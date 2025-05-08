using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using ClocktowerWorldBuilder.Condition;
using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Condition.RoleSpecific.Townsfolk;
using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;
using ClocktowerWorldBuilder.Roles.Minions;
using ClocktowerWorldBuilder.Roles.Outsiders;
using ClocktowerWorldBuilder.Roles.Townsfolk;
using IntervalTree;
using Newtonsoft.Json;

namespace ClocktowerWorldBuilder;
public class World: ICloneable
{
    public ClaimWorld ClaimWorld;
    public List<Player> Players;
    public int? RedHerring = null;
    public List<int> NotRedHerring = new();
    public PriorityQueue<Constraint,float> Constraints;
    public List<Constraint> DebugAppliedConstraints = new();
    public List<int?> Poisons;
    public List<List<int>> Sobers;
    public World ParentWorld = null;

    public IntervalTree<Timestamp, int> DemonByDay;
    // public TimeDictionary<int?> DemonByDay ;
    public IntervalTree<Timestamp, int> NotDemonByDay;
    // public TimeDictionary<List<int>> NotDemonByDay;
    public bool DemonAdded;
    public int? StartingImp = null;
    public List<Constraint> AfterConstraints = new();
    
    
    ///Confirmations, CounterConfirmations and RolesInPlay are mutually disjoint
    public Dictionary<RoleTemplate, int> RolesInPlay = new Dictionary<RoleTemplate, int>();
    public int InPlayEvil = 0;
    private int _inPlayMinionCount = 0;
    
    
    ///Confirmations, CounterConfirmations and RolesInPlay are mutually disjoint
    public Dictionary<RoleTemplate, Timestamp> Confirmations = new Dictionary<RoleTemplate, Timestamp>();
    public int ConfirmedMinions = 0;
    
    ///Confirmations, CounterConfirmations and RolesInPlay are mutually disjoint
    ///The Timestamp means that they must be dead by that point
    public Dictionary<RoleTemplate, Timestamp> CounterConfirmations = new Dictionary<RoleTemplate,Timestamp>();
    
    // public HashSet<RoleTemplate> OutsidersLeft;
    // private HashSet<RoleTemplate> _confirmedMinions = new HashSet<RoleTemplate>();
    // public HashSet<RoleTemplate>  NotInPlayRoles= new HashSet<RoleTemplate>();
    
    
    public static int WorldCount = 0;
    public static int FinalWorldCount = 0;
    public int? TotalOutsiders=null;
    public int InPlayOutsiders = 0;
    public List<int> NotOusiderCounts = new List<int>();
    public bool VerifyFilledGame()
    {
        if(TotalOutsiders!=InPlayOutsiders)
        {
            return false;
        }
        if(InPlayEvil != ClaimWorld.Script.MinionCount+1)
        {
            return false;
        }
        return true;
    }
    public static List<World> GenerateFilledPermutations(List<World> worlds)
    {
        List<World> currentWorlds = worlds;
        List<World> nextWorlds = [];
        int playerCount = worlds[0].Players.Count;
        for (int playerId = 0; playerId <playerCount; playerId++)
        {
            foreach (var current in currentWorlds)
            {
                World cloned = (World)current.Clone();
                if (cloned.SetAlignment(playerId, Alignment.Good))
                {
                    nextWorlds.Add(cloned);
                }

                cloned = (World)current.Clone();
                if (cloned.SetAlignment(playerId, Alignment.Evil))
                {
                    nextWorlds.Add(cloned);
                }
            }
            currentWorlds = nextWorlds;
            nextWorlds = [];
        }
        currentWorlds = currentWorlds.Where(w=>w.VerifyFilledGame()).ToList();
        // return currentWorlds.Where(w=>w.VerifyFilledGame()).ToList();
        var finalWorlds = new List<World>();
        while(currentWorlds.Count>0)
        {
            foreach(var world in currentWorlds)
            {
                if(world.Confirmations.Count==0)
                {
                    finalWorlds.Add(world);
                }
                foreach(var confirmedMinion in world.Confirmations)
                {
                    for(int i = 0;i<world.Players.Count;i++)
                    {
                        World cloned = (World)world.Clone();
                        if(cloned.SetRole(i,confirmedMinion.Key))
                        {
                            nextWorlds.Add(cloned);
                        }
                    }
                }    
            }
            currentWorlds = nextWorlds;
            nextWorlds = [];
        }
        
        currentWorlds = finalWorlds;
        finalWorlds = [];
        foreach(var world in currentWorlds)
        {
            for(int i = 0;i<world.Players.Count;i++)
            {
                World cloned = (World)world.Clone();
                if(cloned.SetImp(i,Timestamp.GENESIS,world.ClaimWorld.PlayerStatuses[i].DeathDay().Before(),true))
                {
                    finalWorlds.Add(cloned);
                }
            } 
        }
        
        currentWorlds = finalWorlds;
        finalWorlds = [];
        foreach(var world in currentWorlds)
        {
            for(int i = 0;i<world.Players.Count;i++)
            {
                foreach(var minion in world.ClaimWorld.Script.Minions)
                {
                    World cloned = (World)world.Clone();
                    if(cloned.SetRole(i,minion))
                    {
                        finalWorlds.Add(cloned);
                    }
                }
            } 
        }
        return finalWorlds.ToList();
    }
    
    public void SimplifyImpData()
    {
        DemonByDay = Util.SimplifiedIntervalTree(DemonByDay,Players.Count);
        NotDemonByDay = Util.SimplifiedIntervalTree(NotDemonByDay,Players.Count);
    }
    public bool SetOutsider(int playerId)
    {
        if (!Players[playerId].IsOutsider())
        {
            InPlayOutsiders++;
        }
        if (!Players[playerId].SetRoleType(RoleType.Outsider))
        {
            return false;
        }

        if (InPlayOutsiders > TotalOutsiders)
        {
            return false;
        }

        return true;

    }
    public bool SetNotRedHerring(int playerId)
    {
        if (RedHerring == playerId)
        {
            return false;
        }

        if (NotRedHerring.Contains(playerId))
        {
            return true;
        }
        else
        {
            NotRedHerring.Add(playerId);
        }
        return true;
    }
    public bool SetRedHerring(int playerId)
    {
        if (RedHerring == playerId)
        {
            return true;
        }
        if (RedHerring != null)
        {
            return false;
        }

        if (NotRedHerring.Contains(playerId))
        {
            return false;
        }
        RedHerring = playerId;
        return true;
    }

    public List<World> FlushConstraints(bool doAfterConstraints=false)
    {
        List<World> currentWorlds = [this];
        List<World> newWorlds = [];
        List<World> finalWorlds = [];
        int founds = -1;
        while(currentWorlds.Count > 0)
        {
            foreach (var world in currentWorlds)
            {
                if ((!doAfterConstraints && world.Constraints.Count == 0)||(doAfterConstraints && world.AfterConstraints.Count == 0))
                {
                    founds++;
                    finalWorlds.Add(world);
                    continue;
                }

                Constraint constraint;
                if(!doAfterConstraints)
                {
                    
                    constraint = world.Constraints.Dequeue();
                    if (constraint is AfterConstraint)
                    {
                        World cloneWorld = (World)world.Clone();
                        cloneWorld.AfterConstraints.Add(constraint);
                        newWorlds.Add(cloneWorld);
                        continue;
                    }
                }else
                {
                    constraint = world.AfterConstraints[^1];
                    world.AfterConstraints.RemoveAt(world.AfterConstraints.Count - 1);
                }
                var toAdd = constraint.PotentialWorlds(world);
                foreach(var addingWorld in toAdd)
                {
                    addingWorld.ParentWorld = world;
                }
                newWorlds.AddRange(toAdd); 
                
            } 
            currentWorlds = newWorlds;
            newWorlds = [];
        }
        return finalWorlds;
    }
    
    public List<World> CalculateWorldBetter(bool doPrint =true)
    {
        var filledWorlds = GenerateFilledPermutations(FlushConstraints());
        List<World> finalWorlds = [];
        foreach (var world in filledWorlds)
        {
            finalWorlds.AddRange(world.FlushConstraints(true));
        }

        return finalWorlds;
    }

    private bool CheckMinionCount()
    {
        
        if (ConfirmedMinions+ _inPlayMinionCount > ClaimWorld.Script.MinionCount)
        {
            return false;
        }

        return true;
    }
    public bool CounterConfirmRole(RoleTemplate role, Timestamp maximumStatus)
    {
        if (RolesInPlay.TryGetValue(role, out var value))
        {
            Status roleStatus = ClaimWorld.PlayerStatuses[value];
            if (roleStatus.AliveOn(maximumStatus))
            {
                return false;
            }

            return true;

        }
        Timestamp? minimumStatus = Confirmations.GetValueOrDefault(role);
        if (minimumStatus is not null)
        {
            if (maximumStatus <= minimumStatus)
            {
                return false;
            }
            
        }

        if (CounterConfirmations.TryGetValue(role, out var timestamp))
        {
            if (maximumStatus < timestamp)
            {
                CounterConfirmations[role] =maximumStatus;
            }
        }
        else
        {
            CounterConfirmations[role] =  maximumStatus;
            
        }

        return true;


    }

    public int? GetPlayerByRole(RoleTemplate role)
    {
        return Players.FindIndex(p => p.Role == role);
    }
    public bool ConfirmRole(RoleTemplate role, Timestamp minimumStatus)
    {
        if (RolesInPlay.TryGetValue(role, out var value))
        {
            Status roleStatus = ClaimWorld.PlayerStatuses[value];
            if (!roleStatus.AliveOn(minimumStatus))
            {
                return false;
            }

            return true;

        }
        Timestamp? maximumStatus = CounterConfirmations.GetValueOrDefault(role);
        if (maximumStatus is not null)
        {
            if (maximumStatus <= minimumStatus)
            {
                return false;
            }

        }

        if (Confirmations.TryGetValue(role, out var timestamp))
        {
            if (minimumStatus > timestamp)
            {
                Confirmations[role] =  minimumStatus;
            }
        }
        else
        {
            Confirmations[role] =  minimumStatus;
            
            if (role.RoleType == RoleType.Minion)
            {
                ConfirmedMinions++;
            }
        }



        return CheckMinionCount();

    }
    public bool SetAlignment(int playerId, Alignment alignment)
    {

        if (alignment == Alignment.Evil)
        {
            if (Players[playerId].Alignment != Alignment.Evil)
            {
                
                if(InPlayEvil >= ClaimWorld.Script.MinionCount + 1)
                {
                    return false;
                }

                InPlayEvil++;
            } 
        }
        if (!Players[playerId].SetAlignment(alignment))
        {
            return false;
        }

        return true;
    }

    public bool ApplySobriety(int playerId, int day)
    {
        if (Poisons[day - 1]==playerId)
        {
            return false;
        }
        if(!Sobers[day-1].Contains(playerId))
        {
            
            Sobers[day-1].Add(playerId);
        }
            
        return true;
    }
    public bool ApplyPoison(int playerId, int day)
    {
        
        if (Poisons[day - 1] != null && Poisons[day - 1] != playerId)
        {
            return false;
        }

        if (Sobers[day - 1].Contains(playerId))
        {
            return false;
        }
        Poisons[day-1] = playerId;
        return true;
    }

    public bool SetNotImp(int playerId, Timestamp start,Timestamp end)
    {
        foreach(var p in DemonByDay.Query(start,end))
        {
            if(p==playerId)
            {
                return false;
            }
        }
        NotDemonByDay.Add(start,end, playerId);

        return true;
    }
    public bool SetImp(int playerId, Timestamp start,Timestamp end,bool startingImp = false)
    {
        if (!SetAlignment(playerId, Alignment.Evil))
        {
            return false;
        }

        // Timestamp end = ClaimWorld.PlayerStatuses[playerId].DeathDay().Before();
        foreach(var p in NotDemonByDay.Query(start,end))
        {
            if(p== playerId)
            {
                return false;
            }
        }
        foreach(var p in DemonByDay.Query(start,end))
        {
            if(p!=playerId)
            {
                return false;
            }
        }
        DemonByDay.Add(start,end ,playerId);
        if(startingImp && StartingImp != null && StartingImp != playerId)
        {
            return false;
        }
        if(startingImp)
        {
            StartingImp = playerId;
        }
        return true;
    }
    public bool SetRole(int playerId, RoleTemplate role)
    {
        if(StartingImp==playerId)
        {
            return false;
        }
        // Debug.Assert(role.RoleType != RoleType.Demon);
        if (playerId == 2 && role == new RoleTemplate(typeof(Recluse)))
        {
            // Console.WriteLine("epic");
        }
        if (role.RoleType == RoleType.Outsider && !SetOutsider(playerId))
        {
            return false;
        }
        if (RolesInPlay.TryGetValue(role, out var value))
        {   
            return value == playerId;
        }
         

        if (role.RoleType == RoleType.Demon)
        {
            DemonAdded = true;
        }

        if (CounterConfirmations.TryGetValue(role, out var deathDay))
        {
            if (ClaimWorld.PlayerStatuses[playerId].AliveOn(deathDay))
            {
                return false;
            }
        }

        if (Confirmations.TryGetValue(role, out var aliveDay))
        {
            if (!ClaimWorld.PlayerStatuses[playerId].AliveOn(aliveDay))
            {
                return false;
            }

            Confirmations.Remove(role);
            if (role.RoleType == RoleType.Minion)
            {
                ConfirmedMinions--;
            }
        }

        if (Players[playerId].Role== null)
        {
            if (role.RoleType == RoleType.Minion)
            {
                _inPlayMinionCount++;
            }
        }

        if (InPlayOutsiders > TotalOutsiders)
        {
            return false;
        }
        if (!SetAlignment(playerId, role.Alignment))
        {
            return false;
        }

        if (Players[playerId].RoleType != role.RoleType)
        {
            if (role.RoleType == RoleType.Outsider)
            {
                InPlayOutsiders++;
            }
        } 
        if (!Players[playerId].SetRole(role))
        {
            return false;
        }
        RolesInPlay.Add(role, playerId);
        return CheckMinionCount();
    }
    public World(ClaimWorld claimWorld, List<ClaimConstraint> claimConstraints, List<Constraint> testConstraints)
    {
        DemonByDay = new();
        NotDemonByDay = new();
        Constraints = new(); 
        List<Constraint> constraints = new(claimConstraints);
        constraints.AddRange(testConstraints);
        constraints.Add(claimWorld.CalculateDeathConstraints());
        constraints.Add(claimWorld.CalculateAliveConstraints());
        foreach (var constraint in constraints)
        {
            constraint.CalculateProperty(claimWorld);
            Constraints.Enqueue(constraint,constraint.Priority);
        }
        var outsiderConstraints = claimWorld.Script.CalculateOutsiderConstraint();
        outsiderConstraints.CalculateProperty(claimWorld);
        outsiderConstraints.FixPriority(-1);
        Constraints.Enqueue(outsiderConstraints, outsiderConstraints.Priority);
        
        ClaimWorld = claimWorld;
        Players = new List<Player>();
        Poisons = new List<int?>();
        Sobers = new();
        for (int i = 0; i < claimWorld.PlayerStatuses.Count; i++)
        {
            Players.Add(new Player(i));
        }


        for (int i = 0; i < claimWorld.Day; i++)
        {
            Poisons.Add(null);
            Sobers.Add(new List<int>());
        }
    }
    public void Summary()
    {
        SimplifyImpData();
        
        var world = this;
        int lp = 0; 
        foreach (Player p in world.Players)
        {
            lp++;
            if(p.Role == null && StartingImp!= lp-1)
            {
                ConsoleColor color = p.Alignment == Alignment.Evil ? ConsoleColor.Red: ConsoleColor.Blue;
                Console.Write("Player " + lp + " is ");
                Console.ForegroundColor = color;
                Console.WriteLine(p.Alignment);
                Console.ResetColor();
            }else{
                if(StartingImp == lp-1)
                {
                    Console.Write("Player "+lp+" is ");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Imp");
                    Console.ResetColor();
                }else
                {
                    bool wasDemon = false;
                    foreach(var demon in DemonByDay)
                    {
                        if(demon.Value == lp-1)
                        {
                            Console.Write("Player " + lp + " started as ");
                            Util.PrintRole(p.Role);
                            Console.Write(" and got ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("starpassed ");
                            Console.ResetColor();
                            Console.WriteLine("to on day "+ demon.From.Day);
                                
                            wasDemon = true;
                            break;
                        } 
                    }
                    if(!wasDemon)
                    {
                        
                        Console.Write("Player "+lp+" is ");
                        Util.PrintRole(p.Role);
                        Console.WriteLine();
                    }
                }
            }
        }
        if(RolesInPlay.ContainsKey(new RoleTemplate(typeof(FortuneTeller))))
        {
            Console.WriteLine("Red Herring is player "+RedHerring);
        }
        if(RolesInPlay.ContainsKey(new RoleTemplate(typeof(Poisoner))))
        {
            Console.WriteLine("Poisons were "+
                String.Join(", ",Poisons.Select(p=> p == null ? "omitted" : (p+1)+""))
            );
        }
    }
    public void Print()
    {
        SimplifyImpData();
        var world = this;
        int lp = -1; 
        foreach (Player p in world.Players)
        {
            lp++;
            Console.WriteLine(lp+" :"+p.Alignment +" " +p.Role);
        }
                    
        Console.WriteLine("Poisons: "+String.Join(", ",world.Poisons));
        Console.WriteLine("Sobers: "+String.Join(", ",world.Sobers));
        Console.WriteLine("Confirmed: "+String.Join(",",world.Confirmations));
        Console.WriteLine("Counter Confirms"+String.Join(",",world.CounterConfirmations));
        Console.WriteLine("Outsider Count: "+world.TotalOutsiders);
        Console.WriteLine("In Play Minions: "+(world._inPlayMinionCount+world.ConfirmedMinions));
        Console.WriteLine("Imp Data "+ String.Join(",",world.DemonByDay.Select(kv =>$"({kv.From} to {kv.To}, {kv.Value})")));
        Console.WriteLine("Not Imp Data "+ String.Join(",",world.NotDemonByDay.Select(kv =>$"({kv.From} to {kv.To},"+String.Join(",",kv.Value)+")")));
        Console.WriteLine("Red Herring "+ world.RedHerring);
        Console.WriteLine("Not Red Herring "+ String.Join(",",world.NotRedHerring));
        Console.WriteLine("-------------------------------------------------------");
    }
    public void Enqueue(Constraint constraint)
    {
        Debug.Assert(!float.IsNaN(constraint.Priority));
        Constraints.Enqueue(constraint,constraint.Priority);
    }
    public object Clone()
    {
        World clone = (World)MemberwiseClone();
        // clone.OutsidersLeft= new HashSet<RoleTemplate>(OutsidersLeft);
        clone.RolesInPlay = new(RolesInPlay);
        clone.Confirmations = new(Confirmations);
        clone.CounterConfirmations = new(CounterConfirmations);
        clone.Constraints = new PriorityQueue<Constraint, float>();
        foreach(var (e,p) in Constraints.UnorderedItems)
        {
            clone.Constraints.Enqueue(e,p);
        }
        clone.DebugAppliedConstraints = new List<Constraint>(DebugAppliedConstraints);
        clone.NotOusiderCounts = new List<int>(NotOusiderCounts);
        clone.NotRedHerring = new(NotRedHerring);
        clone.DemonByDay = new();
        foreach(var v in DemonByDay)
        {
            clone.DemonByDay.Add(v.From,v.To,v.Value);
        }
        clone.NotDemonByDay = new();
        foreach(var v in NotDemonByDay)
        {
            clone.NotDemonByDay.Add(v.From,v.To,v.Value);
        }
        clone.AfterConstraints = new(AfterConstraints);
        clone.Poisons = new List<int?>(Poisons);
        clone.Sobers = new(Sobers.Select(x=>new List<int>(x)).ToList());
        clone.Players = Players.Select(p =>
        {
            return (Player)p.Clone();
        }
            ).ToList();
        return clone;
    }
}