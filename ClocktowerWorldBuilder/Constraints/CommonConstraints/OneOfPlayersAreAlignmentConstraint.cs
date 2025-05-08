using System.Diagnostics;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class OneOfPlayersAreAlignmentConstraint: SubConstraint
{
    public List<int> PlayerIds;
    public Alignment Alignment;
    public OneOfPlayersAreAlignmentConstraint(List<int> playerIds, Alignment alignment)
    {
        Debug.Assert(playerIds.Count > 0);
        PlayerIds = playerIds;
        Alignment = alignment;
        Constraint = new OrConstraint(playerIds.Select(id => (Constraint) new IsAlignment(id, alignment)).ToList());
    }
}