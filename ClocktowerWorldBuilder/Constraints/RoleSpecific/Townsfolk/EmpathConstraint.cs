using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

namespace ClocktowerWorldBuilder.Condition.RoleSpecific.Townsfolk;

public class EmpathConstraint: TemporalConstraint
{
    public List<int> Info;
    public int PlayerId;
    private ClaimWorld claimWorld;
    public EmpathConstraint(int playerId, List<int> info, ClaimWorld claimWorld)
    {
        PlayerId = playerId;
        Info = info;
        this.claimWorld = claimWorld;
        var constraintsByDay = new List<(int,Constraint,bool)>();
        for (int i = 0; i < Info.Count; i++)
        {
            constraintsByDay.Add((i+1,CalculateConstraint(i),false));
        }
        ConstraintsByDay = constraintsByDay;
        return;
    }
    private Constraint CalculateConstraint(int dayMinusOne)
    {
        List<Constraint> totalConstraints = new List<Constraint>();
        int leftPlayerId = PlayerId - 1;
        int rightPlayerId = PlayerId + 1;
        leftPlayerId = Util.Mod(leftPlayerId, claimWorld.Script.PlayerCount);
        rightPlayerId= Util.Mod(rightPlayerId, claimWorld.Script.PlayerCount);
        var leftPlayer =claimWorld.PlayerStatuses[leftPlayerId]; 
        var rightPlayer =claimWorld.PlayerStatuses[rightPlayerId];
        while (!leftPlayer.AliveOn(dayMinusOne,0))
        {
            leftPlayerId += -1;
            leftPlayerId = Util.Mod(leftPlayerId, claimWorld.Script.PlayerCount);
            leftPlayer = claimWorld.PlayerStatuses[(leftPlayerId)]; 
        }
        while (!rightPlayer.AliveOn(dayMinusOne+1,0))
        {
            rightPlayerId+= -1;
            rightPlayerId= Util.Mod(rightPlayerId, claimWorld.Script.PlayerCount);
            rightPlayer= claimWorld.PlayerStatuses[(leftPlayerId)]; 
        }

        switch (Info[dayMinusOne])
        {
            case 0:
                totalConstraints.Add(new AlignmentRegistersConstraint(leftPlayerId, dayMinusOne,Alignment.Good)); 
                totalConstraints.Add(new AlignmentRegistersConstraint(rightPlayerId, dayMinusOne,Alignment.Good));
                break;
            case 1:
                totalConstraints.Add(new OrConstraint([
                    new AndConstraint([
                        new AlignmentRegistersConstraint(leftPlayerId, dayMinusOne+1,Alignment.Evil),
                        new AlignmentRegistersConstraint(rightPlayerId, dayMinusOne+1,Alignment.Good),
                    ]),
                    new AndConstraint([
                        new AlignmentRegistersConstraint(leftPlayerId, dayMinusOne+1,Alignment.Good),
                        new AlignmentRegistersConstraint(rightPlayerId, dayMinusOne+1,Alignment.Evil)
                    ])
                ]));
                break;
            case 2:
                totalConstraints.Add(new AlignmentRegistersConstraint(leftPlayerId, dayMinusOne,Alignment.Evil));
                totalConstraints.Add(new AlignmentRegistersConstraint(rightPlayerId, dayMinusOne,Alignment.Evil));
                break;
            default:
                totalConstraints.Add(new NeverConstraint());
                break;
        } 

        return new AndConstraint(totalConstraints);
        // return new SpecialComplementConstraint(new AlwaysConstraint(),new ANDConstraint(totalConstraints));
    }

    public List<(int, Constraint,bool)> ConstraintsByDay { get; }
}