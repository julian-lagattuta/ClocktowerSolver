using System.Diagnostics;
using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;
using ClocktowerWorldBuilder.Roles.Minions;

namespace ClocktowerWorldBuilder.Condition.RoleSpecific.Minion;

public class ScarletWomanAsDemonAfterConstraint(Timestamp when): AfterConstraint
{
    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1; 
    }
    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        //TODO I DONT THINK THIS IS ACCURATE
        return new SpecialComplementConstraint(new AlwaysConstraint(),this);
    }

    public override List<World> PotentialWorlds(World world)
    {
        int? scarletWoman = world.GetPlayerByRole(new(typeof(ScarletWoman)));
        Debug.Assert(scarletWoman.HasValue);
        World cloneWorld = (World)world.Clone();
        if (!cloneWorld.SetImp(scarletWoman.Value, when,world.ClaimWorld.PlayerStatuses[scarletWoman.Value].DeathDay().Before()))
        {
            return [];
        }

        return [cloneWorld];
    }

}