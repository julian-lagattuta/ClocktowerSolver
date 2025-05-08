using ClocktowerWorldBuilder.Condition;

namespace ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

public class SpecialComplementConstraint(Constraint complementConstraint, Constraint originalConstraint): Constraint
{
    public override float GetPriority(ClaimWorld claimWorld)
    {
        return complementConstraint.GetPriority(claimWorld);
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        
        return originalConstraint;
    }

    public override List<World> PotentialWorlds(World world)
    {
        return complementConstraint.PotentialWorlds(world);
    }

}