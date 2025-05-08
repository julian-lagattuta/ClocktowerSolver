using ClocktowerWorldBuilder.Condition.CommonConstraints;

namespace ClocktowerWorldBuilder.Condition.RoleSpecific.Townsfolk;

public class LibrarianConstraint: TemporalConstraint
{
    public LibrarianConstraint(int player1, int player2, RoleTemplate role)
    {
     ConstraintsByDay =
        [(1,new OrConstraint([
                    new RoleRegisters(player1,1,role),
                    new RoleRegisters(player2, 1,role),
                ]
            ),false)
        ];
    }

    public LibrarianConstraint()
    {
       ConstraintsByDay =[(1, new OutsidersCountConstraint(0),false)];
        
    }

    public List<(int, Constraint,bool)> ConstraintsByDay { get; }
}