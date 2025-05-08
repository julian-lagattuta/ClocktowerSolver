using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

namespace ClocktowerWorldBuilder.Condition.GeneralAfterConstraints;

public class OutsiderCountAfterConstraint: AfterConstraint
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