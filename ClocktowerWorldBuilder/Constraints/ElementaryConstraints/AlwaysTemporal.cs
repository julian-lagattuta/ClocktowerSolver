using ClocktowerWorldBuilder.Condition;
using ClocktowerWorldBuilder.Condition.CommonConstraints;

namespace ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

public class AlwaysTemporal: TemporalConstraint
{
    public List<(int, Constraint, bool)> ConstraintsByDay { get; } = [(1,new AlwaysConstraint(),false)];
}