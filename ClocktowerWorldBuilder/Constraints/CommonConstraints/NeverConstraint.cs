using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class NeverConstraint: Constraint

{

    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 0;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new AlwaysConstraint();
    }
    

    public override List<World> PotentialWorlds(World world)
    {
        return [];
    }

}
public class AlwaysConstraint: Constraint
{

    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new NeverConstraint();
    }

    public override List<World> PotentialWorlds(World world)
    {
        return [(World)world.Clone()];
    }

}
