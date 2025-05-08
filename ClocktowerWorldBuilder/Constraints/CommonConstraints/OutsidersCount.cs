using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class OutsidersCountConstraint(int outsiderCount): Constraint
{
    public int OutsiderCount = outsiderCount;
    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new NotOutsiderCountConstraint(OutsiderCount);
    }

    public override List<World> PotentialWorlds(World world)
    {
        if (world.TotalOutsiders != null || world.NotOusiderCounts.Contains(OutsiderCount))
        {
            if (world.TotalOutsiders != OutsiderCount)
            {
                return [];
            }
        }
        World cloneWorld = (World)world.Clone();
        cloneWorld.TotalOutsiders = OutsiderCount;
        return [cloneWorld];

    }

}
public class NotOutsiderCountConstraint(int notOutsiderCount): Constraint
{
    public int NotOutsiderCount = notOutsiderCount;
    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new OutsidersCountConstraint(NotOutsiderCount);
    }

    public override List<World> PotentialWorlds(World world)
    {
        if (world.TotalOutsiders != null && world.TotalOutsiders == NotOutsiderCount)
        {
            return [];
        }

        World cloneWorld = (World)world.Clone();
        if (world.TotalOutsiders == null && !world.NotOusiderCounts.Contains(NotOutsiderCount))
        {
            cloneWorld.NotOusiderCounts.Add(NotOutsiderCount);
        }

        return [cloneWorld];
    }

}