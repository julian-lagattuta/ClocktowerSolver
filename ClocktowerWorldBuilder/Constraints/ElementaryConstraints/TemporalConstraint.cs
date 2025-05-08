namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public interface TemporalConstraint
{
    public List<(int,Constraint,bool)> ConstraintsByDay { get; }
}