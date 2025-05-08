using ClocktowerWorldBuilder.Condition.CommonConstraints;

namespace ClocktowerWorldBuilder.Condition.RoleSpecific.Townsfolk;

public class UndertakerConstraint:  TemporalConstraint
{
    public UndertakerConstraint(int day, int lastExecutedPlayer, RoleTemplate role)
    {
        ConstraintsByDay = [(day, new RoleRegisters(lastExecutedPlayer,day, role),false)]; 
    } 
    public List<(int, Constraint,bool)> ConstraintsByDay { get; }
}