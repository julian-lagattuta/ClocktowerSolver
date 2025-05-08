using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Condition.RoleSpecific.Minion;
using ClocktowerWorldBuilder.Roles.Minions;

namespace ClocktowerWorldBuilder.Condition.RoleSpecific.Townsfolk;

public class SlayerConstraint: TemporalConstraint
{
    public SlayerConstraint(int day, int slayerClaimId,int targetId, bool success, ClaimWorld claimWorld)
    {
        Constraint constraint;
        if (success)
        {
            constraint = new AndConstraint([
                new RoleRegisters(targetId, day, new(typeof(Imp))),
                new SobrietyApply(slayerClaimId, day)
            ]);
        }
        else
        {
            constraint = new RoleRegisters(targetId, day, new(typeof(Imp))).FetchComplement(claimWorld);
        }

        ConstraintsByDay = [(day, constraint,false)];
    }
    public List<(int, Constraint,bool)> ConstraintsByDay { get; }
}