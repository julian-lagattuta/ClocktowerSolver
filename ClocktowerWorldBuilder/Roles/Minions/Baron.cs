using System.Collections.Immutable;

namespace ClocktowerWorldBuilder.Roles.Minions;

public class Baron: Role
{
    public static Alignment Alignment = Alignment.Evil;
    public static RoleType RoleType = RoleType.Minion;
    public static readonly ImmutableArray<int> OutsiderDeltas =[2];
    public static readonly string Name = "Baron";
}