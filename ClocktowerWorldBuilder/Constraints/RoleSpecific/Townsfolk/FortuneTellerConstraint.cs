using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Roles.Minions;

namespace ClocktowerWorldBuilder.Condition.RoleSpecific.Townsfolk;
public class FortuneTellerConstraint: TemporalConstraint 
{
    public FortuneTellerConstraint(List<(int, int, bool)> info, ClaimWorld claimWorld)
    {
        ConstraintsByDay = new();
        for (int i = 0; i < info.Count; i++)
        {
            int id1 = info[i].Item1;
            int id2 = info[i].Item2;
            bool flagged = info[i].Item3;
            if (!flagged)
            {
                ConstraintsByDay.Add((i+1,new AndConstraint([
                    new NotRedHerringConstraint(id1),
                    new NotRedHerringConstraint(id2),
                    new RoleRegisters(id1, i+1,new(typeof(Imp))).FetchComplement(claimWorld),
                    new RoleRegisters(id2, i+1,new(typeof(Imp))).FetchComplement(claimWorld)
                    ]
                    ),false)); 
            }
            else
            {
                ConstraintsByDay.Add((i+1,new OrConstraint([
                    new RedHerringConstraint(id1),
                    new RedHerringConstraint(id2),
                    new RoleRegisters(id1, i+1,new(typeof(Imp))),
                    new RoleRegisters(id2, i+1,new(typeof(Imp)))
                    ]
                    ),false)); 
            }
        } 
    }
    public List<(int, Constraint,bool)> ConstraintsByDay { get; }
}


public class NotRedHerringConstraint(int playerId) : Constraint
{
    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new RedHerringConstraint(playerId);
    }

    public override List<World> PotentialWorlds(World world)
    {
        World cloneWorld = (World)world.Clone();
        if (!cloneWorld.SetNotRedHerring(playerId))
        {
            return [];
        }

        return [cloneWorld];
    }
}
public class RedHerringConstraint(int playerId) : Constraint
{
    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1;
    }

    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new NotRedHerringConstraint(playerId);
    }

    public override List<World> PotentialWorlds(World world)
    {
        World cloneWorld = (World)world.Clone();
        if (!cloneWorld.SetRedHerring(playerId))
        {
            return [];
        }

        return [cloneWorld];
    }

}
