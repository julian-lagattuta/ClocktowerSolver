using IntervalTree;

namespace ClocktowerWorldBuilder;

public class Util
{
    public static void PrintRole(RoleTemplate role)
    {
        ConsoleColor color;
        if(role.RoleType== RoleType.Minion)
        {
            color = ConsoleColor.Red;
        }else if (role.RoleType== RoleType.Outsider){
            color = ConsoleColor.Cyan;
        }else if (role.RoleType== RoleType.Townsfolk)
        {
            color = ConsoleColor.Blue;
        }else
        {
            color = ConsoleColor.DarkRed;
        }
        Console.ForegroundColor = color;
        Console.Write(role.Name);
        Console.ResetColor();
    }
    public static int Mod(int a, int b)
    {
        if (a < 0)
        {
            return b + (a%b);
        }

        return a % b;
    }
    
    public static IntervalTree<Timestamp,int> SimplifiedIntervalTree(IntervalTree<Timestamp,int> intervalTree,int playerCount)
    {
        
        List<(Timestamp, Timestamp)?> simplifiedImpData = new();
        for(int i = 0;i<playerCount;i++)
        {
            simplifiedImpData.Add(null);
        }
        foreach(var p in intervalTree)
        {
            var data = simplifiedImpData[p.Value];
            if(data is not null)
            {
                (Timestamp, Timestamp) a = data.Value;
                var (start, end) = a;
                Timestamp newFrom = start;
                Timestamp newTo=end;
                if(p.From< start)
                {
                    newFrom = p.From;
                }
                if(p.To>end)
                {
                    newTo = p.To;
                }
                simplifiedImpData[p.Value] = (newFrom,newTo);
            }else{
                simplifiedImpData[p.Value] = (p.From,p.To);
            }
        }

        var newTree = new IntervalTree<Timestamp,int>();
        int j = -1;
        foreach(var a in simplifiedImpData)
        {
            j++;
            if(a is not null)
            {
                newTree.Add(a.Value.Item1,a.Value.Item2,j); 
            }
        }

        return newTree;
    }
    public static List<bool> AlwaysPossible(World world)
    {
        List<bool> result = new();
        for (int i = 0; i < world.ClaimWorld.Day; i++)
        {
            result.Add(true);
        }
        return result;
    }

    public static Alignment OppositeAlignment(Alignment alignment)
    {
        if (alignment == Alignment.Evil)
        {
            return Alignment.Good;
        }

        return Alignment.Evil;
    }

    public static Alignment AlignmentOfRoleType(RoleType roleType)
    {
        switch (roleType)
        {
            case RoleType.Demon:
                return Alignment.Evil;
            case RoleType.Minion:
                return Alignment.Evil;
            default:
                return Alignment.Good;
        }
    }
    public static int PlayerToLeft(int playerId, World world)
    {
        return  Util.Mod(playerId-1,world.Players.Count);
    }
    public static int PlayerToRight(int playerId, World world)
    {
        return  Util.Mod(playerId+1,world.Players.Count);
    }
}