using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;
using ClocktowerWorldBuilder.Roles.Minions;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class RoleInPlayConstraint: Constraint
{
    public RoleTemplate Role;
    public Timestamp OnDay;
    public RoleInPlayConstraint(RoleTemplate role, Timestamp onDay)
    {
        Role = role;
        OnDay = onDay;
    }

    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new RoleNotInPlayConstraint(Role,OnDay);
    }

    public override List<World> PotentialWorlds(World world)
    {
        World cloneWorld = (World)world.Clone();
        if (!cloneWorld.ConfirmRole(Role,OnDay))
        {
            return [];
        }

        return [cloneWorld];
    }

}