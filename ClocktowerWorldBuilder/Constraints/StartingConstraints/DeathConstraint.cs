using ClocktowerWorldBuilder.Condition.RoleSpecific.Minion;
using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;
using ClocktowerWorldBuilder.Roles.Minions;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class DeathConstraint: SubConstraint
{
    public int PlayerId;
    private void CalculateDeathConstraint(Death playerStatus, int playerId,ClaimWorld claimWorld)
    {
            List<int> alivePlayers = claimWorld.PlayersAliveBefore(playerStatus.Timestamp.Day, playerStatus.Timestamp.Order+1);
            if (!playerStatus.ByDemon)
            {
                if (alivePlayers.Count < 4)
                {
                    Constraint = new IsNotRole(playerId, new RoleTemplate(typeof(Imp)));
                }
                else
                {
                    Constraint = new IfThenConstraint(
                        new IsImpAtTimestampConstraint(playerId, playerStatus.Timestamp.Before(),null),
                        new AndConstraint([
                            new RoleInPlayConstraint(new RoleTemplate(typeof(ScarletWoman)), playerStatus.Timestamp),
                            new ScarletWomanAsDemonAfterConstraint(playerStatus.Timestamp),
                            new OneOfPlayersAreAlignmentConstraint(alivePlayers, Alignment.Evil), //then there is an evil player alive
                        ])
                    );
                }
            }
            else
            {
                Constraint = new AndConstraint([
                    new IfThenConstraint(
                        new IsImpAtTimestampConstraint(playerId, playerStatus.Timestamp.Before(),null),
                        new AndConstraint([
                            new OneOfPlayersAreAlignmentConstraint(alivePlayers, Alignment.Evil), //then there is an evil player alive
                            new IfThenConstraint(
                                new RoleInPlayConstraint(new RoleTemplate(typeof(ScarletWoman)), playerStatus.Timestamp),
                                new ScarletWomanAsDemonAfterConstraint(playerStatus.Timestamp)
                            ),
                            new IfThenConstraint(
                                new RoleNotInPlayConstraint(new(typeof(ScarletWoman)),playerStatus.Timestamp),
                                new EvilPlayerAsDemonAfterConstraint(playerStatus.Timestamp)
                            )
                        ])
                    ),
                ]);
            }

            Constraint = new AndConstraint([
                Constraint,
                new IsNotImpAtTimestampConstraint(playerId,playerStatus.Timestamp,Timestamp.FOREVER)
                ]);
    }
    public DeathConstraint(int playerId, ClaimWorld claimWorld)
    {
        PlayerId = playerId;
        Status playerStatus = claimWorld.PlayerStatuses[playerId];
        switch (playerStatus)
        {
            case Alive alive:
                Constraint = new AlwaysConstraint();
                break;
            case Death death:
                CalculateDeathConstraint(death, playerId,claimWorld);
                break;
        }
    }
}