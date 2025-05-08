using ClocktowerWorldBuilder.Condition;

namespace ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

public class IfThenConstraint: SubConstraint
{

    public Constraint IfConstraint;
    public Constraint ThenConstraint;
    public IfThenConstraint(Constraint ifConstraint, Constraint thenConstraint)
    {
        IfConstraint = ifConstraint;
        ThenConstraint = thenConstraint;
    }
    public override void CalculateProperty(ClaimWorld claimWorld)
    {
        Constraint = new OrConstraint([IfConstraint.FetchComplement(claimWorld),ThenConstraint]);
        base.CalculateProperty(claimWorld);
    }
}