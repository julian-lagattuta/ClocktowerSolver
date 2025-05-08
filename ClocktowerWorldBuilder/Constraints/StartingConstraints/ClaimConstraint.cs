using ClocktowerWorldBuilder.Condition.RoleSpecific.Minion;
using ClocktowerWorldBuilder.Roles.Outsiders;
using ClocktowerWorldBuilder.Roles.Townsfolk;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class ClaimConstraint: SubConstraint
{
    public TemporalConstraint? ClaimedConstrant;
    public int PlayerId;
    public RoleTemplate RoleClaim;
    public ClaimConstraint(int playerId,  RoleTemplate roleClaim ,TemporalConstraint? claimed)
    {   
        PlayerId = playerId;
        RoleClaim = roleClaim;
        ClaimedConstrant = claimed;
        if (claimed != null)
        {
            List<Constraint> dailyConstraints = new List<Constraint>();
            bool isConfirmed = false;
            foreach(var (day, constraint, confirmed) in claimed.ConstraintsByDay)
            {
                isConfirmed = isConfirmed || confirmed;
                if (confirmed)
                {
                    
                    dailyConstraints.Add(new AndConstraint(
                        [constraint, new SobrietyApply(playerId,day)])); 
                }
                else
                {
                    dailyConstraints.Add(new OrConstraint([
                        new PoisonerConstraint(playerId,day),
                        constraint
                    ]));
                }
            }

            if (isConfirmed)
            {
                Constraint = new AndConstraint([
                    new IsRole(playerId,roleClaim),
                    new AndConstraint(dailyConstraints)
                ]);
            }
            else
            {
                if (roleClaim.RoleType == RoleType.Townsfolk)
                {
                    Constraint = new OrConstraint([
                        new IsRole(playerId, new RoleTemplate(typeof(Drunk))),
                        new AndConstraint([
                            new IsRole(playerId, roleClaim),
                            new AndConstraint(dailyConstraints)
                        ]),
                        new IsAlignment(playerId, Alignment.Evil)
                    ]);
                }
                else
                {
                    Constraint = new OrConstraint([
                        new AndConstraint([
                            new IsRole(playerId, roleClaim),
                            new AndConstraint(dailyConstraints)
                        ]),
                        new IsAlignment(playerId, Alignment.Evil)
                    ]);
                    
                }
            }
        }
        else
        {
            if (roleClaim.RoleType == RoleType.Townsfolk)
            {
                
                Constraint = new OrConstraint([
                    new IsRole(playerId, new RoleTemplate(typeof(Drunk))),
                    new IsRole(playerId, roleClaim),
                    new IsAlignment(playerId, Alignment.Evil)
                ]);
            }
            else
            {
                Constraint = new OrConstraint([
                    new IsRole(playerId, roleClaim),
                    new IsAlignment(playerId, Alignment.Evil)
                ]);
            }
            
        }
    }
}