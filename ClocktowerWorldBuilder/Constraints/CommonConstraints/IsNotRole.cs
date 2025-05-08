using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class IsNotRole(int playerId, RoleTemplate role): Constraint
{
    public int PlayerId = playerId;
    public RoleTemplate Role = role;
    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new IsRole(PlayerId, Role);
    }

    public override List<World> PotentialWorlds(World world)
    {
        World worldClone = (World)world.Clone();
        if (worldClone.Players[PlayerId].AddImpossibleRole(Role))
        {
            return [worldClone];
        }

        return [];
    }

}