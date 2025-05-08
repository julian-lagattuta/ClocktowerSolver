using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Condition.RoleSpecific.Minion;

namespace ClocktowerWorldBuilder.Condition.RoleSpecific.Townsfolk;

public class VirginConstraint: TemporalConstraint
{
    public VirginConstraint(int firstNominationDay, int virginClaimerId, int nomineer, bool success)
    {
        Constraint constraint;
        if (success)
        {
            constraint  = new AndConstraint([
                            new IsNotOutsiderConstraint(nomineer),
                            new AlignmentRegistersConstraint(nomineer,firstNominationDay, Alignment.Good)
            ]);
        }
        else
        {
            constraint = new OrConstraint([
                new IsOutsiderConstraint(nomineer),
                new AlignmentRegistersConstraint(nomineer, firstNominationDay, Alignment.Evil)
            ]);
        }

        ConstraintsByDay = [(firstNominationDay, constraint,success)];
    }
    public List<(int, Constraint,bool)> ConstraintsByDay { get; }
}