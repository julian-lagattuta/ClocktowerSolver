using System.Diagnostics;
using ClocktowerWorldBuilder.Condition;
using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Roles.Minions;

namespace ClocktowerWorldBuilder;
class HashSetComparer<T> : IEqualityComparer<HashSet<T>>
{
    public bool Equals(HashSet<T>? x, HashSet<T>? y)
    {
        Debug.Assert(x!=null && y!=null);
        return x.SetEquals(y);
    }

    public int GetHashCode(HashSet<T> obj)
    {
        int hash = 0;
        foreach (var item in obj)
        {
            Debug.Assert(item!=null);
            hash ^= item.GetHashCode();
        }
        return hash;
    }
}

public class ListComparer<T> : IEqualityComparer<List<T>>
{
    private readonly IEqualityComparer<T> _elementComparer;

    public ListComparer(IEqualityComparer<T> elementComparer = null)
    {
        _elementComparer = elementComparer ?? EqualityComparer<T>.Default;
    }

    public bool Equals(List<T> x, List<T> y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;
        return x.SequenceEqual(y, _elementComparer);
    }

    public int GetHashCode(List<T> obj)
    {
        if (obj == null) return 0;

        int hash = 17;
        foreach (var item in obj)
        {
            hash = hash * 31 + _elementComparer.GetHashCode(item);
        }
        return hash;
    }
}
public class SolvedGame
{
    public List<World> Worlds;
    public ClaimWorld ClaimWorld;
    internal SolvedGame(List<World> worlds,ClaimWorld claimWorld)
    {
        ClaimWorld = claimWorld;
        Worlds = worlds;
    }
    public SolvedGame Filter(Constraint constraint)
    {
        List<World> worlds =[];
        foreach (var world in Worlds)
        {
            worlds.AddRange(constraint.PotentialWorlds(world)); 
        }
        return new SolvedGame(worlds,ClaimWorld);
    } 
    
    public float ProbabilityIsEvil(int playerId)=>ProbabilityIsAlignment(playerId, Alignment.Evil);
    public float ProbabilityIsGood(int playerId)=>ProbabilityIsAlignment(playerId, Alignment.Good);
    private Dictionary<HashSet<RoleTemplate>,List<World>> GenerateBags()
    {
        
        Dictionary<HashSet<RoleTemplate>,List<World>> bags = new(new HashSetComparer<RoleTemplate>());
        foreach (World world in Worlds)
        {
            // HashSet<RoleTemplate> bag = new HashSet<RoleTemplate>(new HashSetComparer<RoleTemplate>()); 
            HashSet<RoleTemplate> bag = new HashSet<RoleTemplate>();
            foreach(var role in world.RolesInPlay)
            {
                bag.Add(role.Key);
            }
            if(bags.TryGetValue(bag, out var bagWorlds))
            {
                bagWorlds.Add(world); 
            }else{
                bags.Add(bag,[world]); 
            }
        }

        return bags;
    }
    private float Probability(Func<World,bool> condition)
    {
        Dictionary<HashSet<RoleTemplate>, List<World>> bags = GenerateBags();
        float probability = 0;
        foreach(var bag in bags)
        {
            float interiorSum = 0;
            foreach(var world in bag.Value)
            {
                if(condition(world)){
                    interiorSum++;
                } 
            } 
            probability += interiorSum / bag.Value.Count;
        }
        
        return probability/bags.Count;
    }
    public float ProbabilityIsRole(int playerId, RoleTemplate role)
    {
        if (role == new RoleTemplate(typeof(Imp)))
        {
            return Probability(w=>w.StartingImp == playerId);
        }
        return Probability(world => world.Players[playerId].Role == role);
    }
    
    public float ProbabilityIsAlignment(int playerId,Alignment alignment)
    {
        return Probability(world=> world.Players[playerId].Alignment == alignment);
    }
    public float ProbabilityIsCurrentlyImp(int playerId)
    {
        return Probability(world =>
        {
           foreach(var v in world.DemonByDay.Query(Timestamp.FOREVER.Before()))
           {
               return v==playerId;
           }
           throw new Exception("no demon alive");
        });
    }
    public void PrintProbabilities()
    {
        Console.WriteLine("Players\t\tEvil\tCurrent Imp\tStarting Imp"); 
        for(int i =0;i< ClaimWorld.Script.PlayerCount;i++)
        {
            var p1 = Math.Round(100*ProbabilityIsEvil(i));
            var p2 = Math.Round(100*ProbabilityIsCurrentlyImp(i));
            var p3 = Math.Round(100*ProbabilityIsRole(i,new(typeof(Imp))));
            Console.WriteLine($"Player {i+1}: \t"+p1+"%\t"+p2+"%\t\t"+p3+"%");
        }
    }
    public void PrintAllWorlds()
    {
        Console.WriteLine("---------------------------");
        Console.WriteLine("All worlds:");
        Console.WriteLine("-------");
        foreach(var world in Worlds)
        {
            world.Summary(); 
            Console.WriteLine("---------------------------");
        } 
        Console.WriteLine($"Outputted {Worlds.Count} worlds.");
    }
    public Dictionary<List<RoleTemplate>,List<World>> SortByArrangements()
    {
        Dictionary<List<RoleTemplate>,List<World>> arrangements = new(new ListComparer<RoleTemplate>());
        foreach (World world in Worlds)
        {
            // HashSet<RoleTemplate> bag = new HashSet<RoleTemplate>(new HashSetComparer<RoleTemplate>()); 
            List<RoleTemplate> roles = [];
            foreach(var player in world.Players)
            {
                if(player.Role == null)
                {
                    roles.Add(new RoleTemplate(typeof(Imp)));
                }else{
                    roles.Add(player.Role);
                }
            }
            if(arrangements.TryGetValue(roles, out var bagWorlds))
            {
                bagWorlds.Add(world); 
            }else{
                arrangements.Add(roles,[world]); 
            }
        }

        return arrangements;

    }
    public void PrintArrangements()
    {
        var arrangements = SortByArrangements();
        Console.WriteLine("---------------------------");
        Console.WriteLine("Arrangements:");
        Console.WriteLine("-------");
        foreach(var arrangement in arrangements)
        {
            int lp = -1;
            foreach(var role in arrangement.Key)
            {
                lp++;
                Console.Write($"Player {lp} is ");   
                Util.PrintRole(role);
                Console.WriteLine();
            }
            Console.WriteLine("---------------------------");
        }
        Console.WriteLine($"Outputted {arrangements.Count} arrangements.");
    }
}
public class Solver
{
    public World BaseWorld; 
    public ClaimWorld ClaimWorld;
    public Solver(ClaimWorld claimWorld, List<ClaimConstraint> claims,  List<Constraint>? trustedFacts=null)
    {
        if(trustedFacts == null)
        {
            BaseWorld = new World(claimWorld, claims, []);
        }else{
            BaseWorld = new World(claimWorld, claims, trustedFacts);
        }

        ClaimWorld = claimWorld;
    }
    public SolvedGame Solve()
    {
        return new SolvedGame(BaseWorld.CalculateWorldBetter(),ClaimWorld);
    }
    
}