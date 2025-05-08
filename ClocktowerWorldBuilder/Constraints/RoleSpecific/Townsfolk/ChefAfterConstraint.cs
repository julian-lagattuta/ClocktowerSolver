using System.Threading.Channels;
using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;
using ClocktowerWorldBuilder.Roles.Townsfolk;

namespace ClocktowerWorldBuilder.Condition.RoleSpecific.Townsfolk;
public class ChefConstraint: TemporalConstraint{
    public ChefConstraint(int chefInfo)
    {
        ConstraintsByDay = [(1, new AlwaysConstraint(),false)];
    }
    public List<(int, Constraint,bool)> ConstraintsByDay { get; }
}
public class ChefAfterConstraint(int chefInfo): AfterConstraint
{
    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new SpecialComplementConstraint(new AlwaysConstraint(),this);
    }
    public override List<World> PotentialWorlds(World world)
    {
        List<(World,int)> currentWorlds = [(world,chefInfo)];
        List<(World,int)> newWorlds = new List<(World,int)>();
        for(int playerId = 0; playerId < world.Players.Count-1; playerId++)
        {
            int playerRight = Util.PlayerToRight(playerId, world);
            foreach (var (current,num) in currentWorlds)
            {
                if(num<0)
                {
                    continue;
                }
                
                if(num >0)
                {
                    World cloneWorld = (World)current.Clone();
                    Constraint pairConstraint = new AndConstraint([
                        new AlignmentRegistersConstraint(playerId,1,Alignment.Evil),
                        new AlignmentRegistersConstraint(playerRight,1,Alignment.Evil),
                    ]);
                    pairConstraint.CalculateProperty(world.ClaimWorld);
                
                    cloneWorld.Enqueue(pairConstraint);

                    var evilWorlds = cloneWorld.FlushConstraints();
                    newWorlds.AddRange(evilWorlds.Select(w=>(w,num-1))); 
                }
                World  goodWorld= (World)current.Clone();
                Constraint noPair= new OrConstraint([
                    new AlignmentRegistersConstraint(playerId,1,Alignment.Good),
                    new AlignmentRegistersConstraint(playerRight,1,Alignment.Good),
                ]);
                noPair.CalculateProperty(world.ClaimWorld);
                
                goodWorld.Enqueue(noPair);

                var goodWorlds = goodWorld.FlushConstraints();
                newWorlds.AddRange(goodWorlds.Select(w=>(w,num))); 
            }

            currentWorlds = newWorlds;
            newWorlds = [];
        }
        return currentWorlds.Select(a=>a.Item1).ToList();    
    }
}