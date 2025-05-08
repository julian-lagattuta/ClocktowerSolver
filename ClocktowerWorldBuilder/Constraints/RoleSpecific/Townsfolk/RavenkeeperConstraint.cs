using ClocktowerWorldBuilder.Condition.CommonConstraints;

namespace ClocktowerWorldBuilder.Condition.RoleSpecific.Townsfolk;

public class RavenkeeperConstraint: TemporalConstraint
{
    public RavenkeeperConstraint(int day, int playerId, RoleTemplate roleTemplate)
    {
        ConstraintsByDay = [(day, new RoleRegisters(playerId, day,roleTemplate),false)];   
    }

    public List<(int, Constraint,bool)> ConstraintsByDay { get; }
}