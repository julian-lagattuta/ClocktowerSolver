using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class IsAlignment(int playerId, Alignment alignment) : Constraint
{
    public int PlayerId = playerId;
    public Alignment Alignment = alignment;



    public override float GetPriority(ClaimWorld claimWorld) => 1;

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return  new IsAlignment(PlayerId, Util.OppositeAlignment(Alignment));
    }

    public override List<World> PotentialWorlds(World world)
    {
        World cloneWorld = (World)world.Clone();
        if (cloneWorld.SetAlignment(playerId, alignment))
        {
            return [cloneWorld];
        }

        return [];
    }
}