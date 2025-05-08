using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

namespace ClocktowerWorldBuilder.Condition.RoleSpecific.Townsfolk;

internal class DemonCannotKillMeOnDayConstraint(int playerId, int day) : Constraint
{
    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new SpecialComplementConstraint(new AlwaysConstraint(), this);
    }

    public override List<World> PotentialWorlds(World world)
    {
        
        if(world.ClaimWorld.PlayerStatuses[playerId] is Death death){
            if (death.Timestamp.Day == day && death.ByDemon)
            {
                return [];
            }
        }
        return [(World) world.Clone()];
    }

}
public class SoldierConstraint: TemporalConstraint
{
    public SoldierConstraint(int playerId, ClaimWorld claimWorld)
    {
        ConstraintsByDay = [];
        for (int i = 0; i < claimWorld.Day;i++)
        {
            ConstraintsByDay.Add((i+1, new DemonCannotKillMeOnDayConstraint(playerId,i+1),false)); 
        }
    }  
    public List<(int, Constraint,bool)> ConstraintsByDay { get; }
}