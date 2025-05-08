using ClocktowerWorldBuilder.Condition;

namespace ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

public class EmptyConstraint: Constraint
{

    public override float GetPriority(ClaimWorld claimWorld)
    {
        throw new NotImplementedException();
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        throw new NotImplementedException();
    }

    public override List<World> PotentialWorlds(World world)
    {
        throw new NotImplementedException();
    }

}