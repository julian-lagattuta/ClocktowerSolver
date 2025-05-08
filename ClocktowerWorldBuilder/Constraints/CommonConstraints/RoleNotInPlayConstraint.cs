using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class RoleNotInPlayConstraint(RoleTemplate role, Timestamp deadBy) : Constraint
{

    
    public Timestamp DeadBy = deadBy;
    public RoleTemplate Role = role;

    public override float GetPriority(ClaimWorld claimWorld) => 1;

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new RoleInPlayConstraint(Role,DeadBy);
    }

    public override List<World> PotentialWorlds(World world)
    {
        World worldClone = (World)world.Clone();
        if (!worldClone.CounterConfirmRole(Role,DeadBy))
        {
            return [];
        }

        return [worldClone];
    }

}