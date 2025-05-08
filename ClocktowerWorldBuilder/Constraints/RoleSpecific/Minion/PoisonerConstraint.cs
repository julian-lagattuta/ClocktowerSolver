using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Condition.RoleSpecific.Townsfolk;
using ClocktowerWorldBuilder.Roles.Minions;

namespace ClocktowerWorldBuilder.Condition.RoleSpecific.Minion;

class SobrietyApply :Constraint
{
    public int TargetId;

    public SobrietyApply(int targetId, int day)
    {
        TargetId = targetId;
        Day = day;
    }

    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new PoisonApply(TargetId, Day);
    }

    public override List<World> PotentialWorlds(World world)
    {
        World worldClone = (World)world.Clone();
        if (!worldClone.ApplySobriety(TargetId, Day))
        {
            return [];
        }
        return [worldClone];
    }


    public int Day;
}
class PoisonApply : Constraint
{
    public int TargetId;

    public PoisonApply(int targetId, int day)
    {
        TargetId = targetId;
        Day = day;
    }

    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new SobrietyApply(TargetId, Day);
    }

    public override List<World> PotentialWorlds(World world)
    {
        World worldClone = (World)world.Clone();
        if (!worldClone.ApplyPoison(TargetId, Day))
        {
            return [];
        }
        return [worldClone];
    }


    public int Day;
}
public class PoisonerConstraint :  SubConstraint
{
    public int TargetId;
    public PoisonerConstraint(int targetId, int day)
    {
        TargetId = targetId;
        Day = day;
        Timestamp date = new Timestamp(day, 0);
        Constraint = new AndConstraint([
            new RoleInPlayConstraint(new RoleTemplate(typeof(Poisoner)), date),
            new PoisonApply(targetId, day)
        ]);
    }

    public int Day;
}