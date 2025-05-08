using System.Diagnostics;
using ClocktowerWorldBuilder.Condition;
using ClocktowerWorldBuilder.Condition.CommonConstraints;

namespace ClocktowerWorldBuilder;

public record ClaimWorld 
{
    // public List<PlayerClaim> PlayerClaims;
    public Script Script;
    public List<Status> PlayerStatuses;
    public List<int> Executees=new List<int>();
    public Dictionary<int, RoleTemplate> KnownRoles = new Dictionary<int, RoleTemplate>();
    public int Day;
    public int Order;
    public Constraint CalculateAliveConstraints()
    {
        var alivePlayers = PlayersAliveBefore(Timestamp.GENESIS.Day, Timestamp.GENESIS.Order);
        return new OrConstraint(
            alivePlayers.Select(i=> (Constraint)new IsImpAtTimestampConstraint(i,new(Day,Order),null)).ToList()
        );
    }
    public List<int> PlayersAliveBefore(int day, int order)
    {
        List<int> aliveIds = new List<int>();
        int i = -1;
        foreach (var status in PlayerStatuses)
        {
            i++;
            if (status.AliveOn(day, order-1))
            {
                aliveIds.Add(i); 
            }
        }

        return aliveIds;
    }

    public Constraint CalculateDeathConstraints()
    {
        List<Constraint> deathConstraints = new();
        for (int i = 0; i < PlayerStatuses.Count; i++)
        {
            var status = PlayerStatuses[i];
            if (status is Death)
            {
                deathConstraints.Add(new DeathConstraint(i, this)); 
            }
        }
        if (deathConstraints.Count > 0)
        {
            return new AndConstraint(deathConstraints);
        }

        return new AlwaysConstraint();
    }
    public ClaimWorld(List<Status> playerStatuses, int day, Script script)
    {
        Debug.Assert(playerStatuses.Count == script.PlayerCount);
        Day = day;
        Script = script;
        PlayerStatuses = playerStatuses;
        Order = 0;
        foreach (var status in PlayerStatuses)
        {
            if(status is Death death)
            {
                if(death.Timestamp.Day == day)
                {
                    Order = Math.Max(death.Timestamp.Order, Order);
                }
            }
        }
        // foreach (var nomination in Nominations)
        // {
        //     if (nomination.Success)
        //     {
        //         Executees.Add(nomination.Target);
        //     }
        //
        //     if (nomination.VirginDeath)
        //     {
        //         //ADD VIRGIN IN FUTURE
        //         // KnownRoles[nomination.Target] = RoleTemplate(Vrig)
        //     }
        // }
    }
}