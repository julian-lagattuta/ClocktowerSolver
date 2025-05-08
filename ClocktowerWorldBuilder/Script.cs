using System.Security.AccessControl;
using ClocktowerWorldBuilder.Condition;
using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

namespace ClocktowerWorldBuilder;

public class Script
{
    public List<RoleTemplate> Roles;
    public int PlayerCount;
    public int BaseOutsiderCount;
    public int MinionCount;

    public List<RoleTemplate> Minions = new List<RoleTemplate>();
    public List<RoleTemplate> Outsiders =new List<RoleTemplate>();
    private List<(int,List<RoleTemplate>)> CalculateOutsiderConstraintHelper(int idx,(int,List<RoleTemplate>) outsiders)
    {
        int i = idx;
        while ( i < Roles.Count && Roles[i].OutsiderDeltas.Length ==0)
        {
            i++;
        }
        if (i >= Roles.Count)
        {
            if (outsiders.Item1 >= 0)
            {
                return [outsiders];
            }

            return [];
        }

        List<(int, List<RoleTemplate>)> finalList = new List<(int, List<RoleTemplate>)>();
        RoleTemplate outsiderChanger = Roles[i];
        foreach (int delta in outsiderChanger.OutsiderDeltas)
        {
            var changed =( outsiders.Item1+delta, new List<RoleTemplate>(outsiders.Item2));
            changed.Item2.Add(outsiderChanger);
            finalList.AddRange(CalculateOutsiderConstraintHelper(i+1, changed)); 
        }
        var unchanged =( outsiders.Item1, new List<RoleTemplate>(outsiders.Item2));
        finalList.AddRange(CalculateOutsiderConstraintHelper(i+1, unchanged));
        return finalList;
    }
    public Constraint CalculateOutsiderConstraint()
    {
        List<Constraint> IfThenConstraints = new List<Constraint>();
        List<(int, List<RoleTemplate>)> outsiderPossibilities = CalculateOutsiderConstraintHelper(0, (BaseOutsiderCount,[]));
        Dictionary<int, List<List<RoleTemplate>>> outsiderGroups = new Dictionary<int, List<List<RoleTemplate>>>();
        foreach (var (outsiderCount, inPlayRoles) in outsiderPossibilities)
        {
            if (outsiderGroups.ContainsKey(outsiderCount))
            {
                outsiderGroups[outsiderCount].Add(inPlayRoles);
            }
            else
            {
                outsiderGroups.Add(outsiderCount, [inPlayRoles]);
            }
        }
        foreach(var (outsiderCount, inPlayRoles) in outsiderGroups){
            
            List<Constraint> roleConstraints = [];
            foreach (var roleGroup in inPlayRoles)
            {
                if (roleGroup.Count > 0)
                {
                    roleConstraints.Add(new AndConstraint(roleGroup.Select(role => (Constraint)new RoleInPlayConstraint(role, Timestamp.GENESIS)).ToList()));
                }
            }

            if (roleConstraints.Count > 0)
            {
                IfThenConstraints.Add(
                    new IfAndOnlyIfConstraint(new OutsidersCountConstraint(outsiderCount), new OrConstraint(roleConstraints))
                ); 
            }
            
        }
        return new AndConstraint([new AndConstraint(IfThenConstraints), new OrConstraint(outsiderGroups.Select(kv => (Constraint)new OutsidersCountConstraint(kv.Key)).ToList())]);

    }
    public Script(List<RoleTemplate> roles, int playerCount, int? baseOutsiderCount = null, int? baseMinionCount = null)
    {
        PlayerCount = playerCount;
        Roles = roles;
        foreach (var role in Roles)
        {
            switch (role.RoleType)
            {
                case RoleType.Minion:
                    Minions.Add(role);
                    break;
                case RoleType.Outsider:
                    Outsiders.Add(role);
                    break;
            }
        }
        if (playerCount<10)
        {
            MinionCount = 1;
        }else if (playerCount < 13)
        {
            MinionCount = 2;
        }
        else
        {
            MinionCount = 3;
        }
        switch (PlayerCount)
        {
            case 5:
                BaseOutsiderCount = 0;
                break;
            case 6:
                BaseOutsiderCount = 1;
                break;
            case 7:
                BaseOutsiderCount = 0;
                break;
            case 8:
                BaseOutsiderCount = 1;
                break;
            case 9:
                BaseOutsiderCount = 2;
                break;
            case 10:
                BaseOutsiderCount = 0;
                break;
            case 11:
                BaseOutsiderCount = 1;
                break;
            case 12:
                BaseOutsiderCount = 2;
                break;
            case 13:
                BaseOutsiderCount = 0;
                break;
            case 14:
                BaseOutsiderCount = 1;
                break;
            case 15:
                BaseOutsiderCount = 2;
                break;
            default:
                if (playerCount > 15)
                {
                    BaseOutsiderCount = 2;
                }
                else
                {
                    BaseOutsiderCount = 0;
                }
                break;
        }

        if (baseMinionCount != null)
        {
            MinionCount = baseMinionCount.Value;
        }

        if (baseOutsiderCount != null)
        {
            BaseOutsiderCount = baseOutsiderCount.Value;
        }
    }
}