using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class IsOutsiderConstraint(int playerId): Constraint
{

    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new IsNotOutsiderConstraint(playerId);
    }

    public override List<World> PotentialWorlds(World world)
    {
        World cloneWorld=  (World) world.Clone();
        if (!cloneWorld.SetOutsider(playerId))
        {
            return [];
        }

        return [cloneWorld];
    }
}
public class IsNotOutsiderConstraint(int playerId) :  Constraint
{

    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new IsOutsiderConstraint(playerId);
    }

    public override List<World> PotentialWorlds(World world)
    {
        World cloneWorld=  (World) world.Clone();
        if (!cloneWorld.Players[playerId].SetNotOutsider())
        {
            return [];
        }
        return [cloneWorld];
    }
}