using ClocktowerWorldBuilder.Condition.CommonConstraints;

namespace ClocktowerWorldBuilder.Condition.RoleSpecific.Townsfolk;

public class InvestigatorConstraint(int player1, int player2, RoleTemplate role): TemporalConstraint
{
    public List<(int,Constraint,bool)> ConstraintsByDay =>
    [(1,new OrConstraint([
                new RoleRegisters(player1,1,role),
                new RoleRegisters(player2, 1,role),
            ]
        ),false)
    ];
}