using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;
using ClocktowerWorldBuilder.Roles.Minions;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class IsImpAtTimestampConstraint(int playerId,Timestamp start, Timestamp? end): Constraint
{
    public int PlayerId = playerId;

    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        
        var realend = claimWorld.PlayerStatuses[playerId].DeathDay().Before();
        if(end != null)
        {
            realend = end;
        }
        return new IsNotImpAtTimestampConstraint(playerId,start,realend);
    }

    public override List<World> PotentialWorlds(World world)
    {
        
        if(!world.ClaimWorld.PlayerStatuses[playerId].AliveOn(start))
        {
            return [];
        }
        var realend = world.ClaimWorld.PlayerStatuses[playerId].DeathDay().Before();
        if(end != null)
        {
            realend = end;
        }
        if(!world.ClaimWorld.PlayerStatuses[playerId].AliveOn(realend))
        {
            return [];
        }
        World cloneWorld  = (World)world.Clone();
        if (!cloneWorld.SetImp(playerId,start,realend))
        {
            return [];
        }
        return [cloneWorld];
    }

}

public class IsNotImpAtTimestampConstraint(int playerId, Timestamp start,Timestamp end): Constraint
{
    public int PlayerId = playerId;

    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new IsImpAtTimestampConstraint(playerId,start,end);
    }

    public override List<World> PotentialWorlds(World world)
    {
        
        World cloneWorld  = (World)world.Clone();
        if(end<start)
        {
            
            return [cloneWorld];
        }
        if (!cloneWorld.SetNotImp(playerId,start,end))
        {
            return [];
        }
        return [cloneWorld];
    }
}