using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

namespace ClocktowerWorldBuilder.Condition;

public abstract class SubConstraint: Constraint
{
    public Constraint Constraint = new EmptyConstraint();
    public SubConstraint()
    {
    }

    public override List<World> PotentialWorlds(World world)
    {
        return Constraint.PotentialWorlds(world);
    }

    public override void CalculateProperty(ClaimWorld claimWorld)
    {
        Constraint.CalculateProperty(claimWorld);
        Complement = Constraint.Complement;
        Priority =  Constraint.Priority;
    }

    public override void FixPriority(float newPriority)
    {
        base.FixPriority(newPriority);
        Constraint.FixPriority(newPriority);
    }

    public override Constraint FetchComplement(ClaimWorld claimWorld)
    {
        return Constraint.FetchComplement(claimWorld);
    }
    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return Constraint.CalculateComplement(claimWorld);
    }
    public override float GetPriority(ClaimWorld claimWorld)
    {
        return Constraint.GetPriority(claimWorld);
    }
}