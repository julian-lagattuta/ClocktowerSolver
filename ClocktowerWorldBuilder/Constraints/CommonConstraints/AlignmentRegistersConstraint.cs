using ClocktowerWorldBuilder.Condition.RoleSpecific.Minion;
using ClocktowerWorldBuilder.Roles.Minions;
using ClocktowerWorldBuilder.Roles.Outsiders;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class AlignmentRegistersConstraint: SubConstraint
{
    public int PlayerId;
    public Alignment Alignment;
    public AlignmentRegistersConstraint(int playerId, int day, Alignment alignment)
    {
        PlayerId = playerId;
        Alignment = alignment;
        if (alignment == Alignment.Good)
        {
            Constraint = new OrConstraint([
                new IsAlignment(playerId, Alignment.Good),
                
                new IsRole(playerId, new RoleTemplate(typeof(Spy)))
            ]);
        }
        else
        {
            Constraint = new OrConstraint([
                new IsAlignment(playerId, Alignment.Evil),
                new AndConstraint([
                    new IsRole(playerId, new RoleTemplate(typeof(Recluse))),
                    new SobrietyApply(playerId,day) //not poisoned
                ])
            ]);
        }
        

    }

}