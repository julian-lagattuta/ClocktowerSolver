using System.Diagnostics;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class OneOfPlayersAreRoleConstraint: SubConstraint
{
    public List<int> PlayerIds;
    public RoleTemplate Role;
    public OneOfPlayersAreRoleConstraint(List<int> playerIds, RoleTemplate role)
    {
        Debug.Assert(playerIds.Count > 0);
        PlayerIds = playerIds;
        Role = role;
        Constraint = new OrConstraint(
            playerIds.Select(id => (Constraint) new IsRole(id, Role)).ToList()
        );
    }
}