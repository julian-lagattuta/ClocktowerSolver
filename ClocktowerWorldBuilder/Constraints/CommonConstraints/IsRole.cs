using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;
using ClocktowerWorldBuilder.Roles.Minions;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class IsRole(int playerId, RoleTemplate role): Constraint
{
    public int PlayerId = playerId;
    public RoleTemplate Role = role;
    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new IsNotRole(PlayerId,Role);
    }

    public override List<World> PotentialWorlds(World world)
    {
        
        World cloneWorld = (World)world.Clone();
        if (cloneWorld.SetRole(PlayerId,Role))
        {
            return [cloneWorld];
        }

        return [];
    }
}