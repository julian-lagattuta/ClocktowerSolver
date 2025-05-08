using ClocktowerWorldBuilder.Condition.RoleSpecific.Minion;
using ClocktowerWorldBuilder.Roles.Minions;
using ClocktowerWorldBuilder.Roles.Outsiders;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class RoleRegisters: SubConstraint
{
    public RoleRegisters(int playerId, int day, RoleTemplate role)
    {
        if (role.Alignment == Alignment.Good)
        {
            Constraint = new OrConstraint([
                new IsRole(playerId, role),
                new AndConstraint([
                    new IsRole(playerId, new RoleTemplate(typeof(Spy))),
                    new IsNotImpAtTimestampConstraint(playerId,Timestamp.GENESIS, new (day,0))
                ])
            ]);
        }
        else
        {
            if (role.RoleType == RoleType.Demon)
            {
                Constraint = new OrConstraint([
                    // new ORConstraint([
                    new IsImpAtTimestampConstraint(playerId, new(day,0),null),
                    // ])
                    new AndConstraint([
                        new IsRole(playerId, new RoleTemplate(typeof(Recluse))),//TODO NEED TO ADD ISALIVE CONSTRAINT. SAME FOR SPY
                        new SobrietyApply(playerId,day) //not poisoned
                    ])
                ]);
            }
            else
            {
                Constraint = new OrConstraint([
                    new IsRole(playerId, role),
                    new AndConstraint([
                        new IsRole(playerId, new RoleTemplate(typeof(Recluse))),
                        new SobrietyApply(playerId,day) //not poisoned
                    ])
                ]);
            }
        }
        

    }
    
}