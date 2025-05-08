using System.Runtime.InteropServices;
using ClocktowerWorldBuilder.Condition;
using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Condition.RoleSpecific.Minion;

namespace ClocktowerWorldBuilder;

public class Tester
{
    public ClaimWorld ClaimWorld;
    public int RandPlayer()
    {
        return Random.Shared.Next(0, ClaimWorld.PlayerStatuses.Count);
    }
    public RoleTemplate RandRole()
    {
        return ClaimWorld.Script.Roles[Random.Shared.Next(0, ClaimWorld.Script.Roles.Count)];
    }
    public Alignment RandAlignment()
    {
        
        return Random.Shared.Next(0, 2) == 0 ? Alignment.Evil : Alignment.Good;
    }
    public bool RandBool()
    {
        return Random.Shared.Next(0, 2) == 0;
    }
    public Timestamp RandomTimestamp()
    {
        return new(Random.Shared.Next(1,ClaimWorld.Day+1),Random.Shared.Next(0,3));
    }
    public List<int> RandPlayerList()
    {
        
        var randLength =Math.Min( RandPlayer()+1,3);
        var l = new List<int>();
        for(int i = 0; i < randLength; i++)
        {
            l.Add(RandPlayer());
        }

        return l;
    }
    public Constraint GenerateRandomCommonConstraint()
    {
        var rand_num = Random.Shared.Next(0,9);
        Constraint constraint;
        switch (rand_num)
        {
            case 0:
                constraint = new IsAlignment(Random.Shared.Next(0, ClaimWorld.PlayerStatuses.Count),
                    Random.Shared.Next(0, 2) == 0 ? Alignment.Evil : Alignment.Good);
            break;
            case 1:
                constraint = new IsImpAtTimestampConstraint(RandPlayer(), RandomTimestamp(),RandomTimestamp());
                break;
            case 2:
                constraint = new IsRole(RandPlayer(),RandRole());
                break;
            case 3:
                constraint = new IsOutsiderConstraint(RandPlayer());
                break;
            case 4:
                constraint = new OneOfPlayersAreAlignmentConstraint(RandPlayerList(), RandAlignment());
                break;
            case 5:
                constraint = new OneOfPlayersAreRoleConstraint(RandPlayerList(), RandRole());
                break;
            case 6:
                constraint = new RoleInPlayConstraint(RandRole(),RandomTimestamp());
                break;
            case 7:
                constraint = new PoisonApply(RandPlayer(), RandomTimestamp().Day);
                break;
            case 8:
                constraint = new RoleRegisters(RandPlayer(), RandomTimestamp().Day,RandRole());
                break;
            default:
                throw new Exception("bruhh");
        }
        if(RandBool())
        {
            constraint = constraint.FetchComplement(ClaimWorld);
        }

        return constraint;
    }
    public List<Constraint> RandomCommonConstraints(int size)
    {
        List<Constraint> constraints = new();
        for(int i = 0; i < size; i++)
        {
            constraints.Add(GenerateRandomCommonConstraint()); 
        }

        return constraints;
    }
    public void Test(int size, int checks)
    {
        while(true)
        {
            var randomConstraints = RandomCommonConstraints(size);
            World world = new World(ClaimWorld,[],randomConstraints);
            int initalize_count = -1;
            for(int i =0; i< checks; i++) 
            {
                var generatedWorlds = ((World)world.Clone()).CalculateWorldBetter();
                // var generatedWorlds = ((World)world.Clone()).CalculateWorldBetter();
                if(initalize_count==-1)
                {
                    initalize_count = generatedWorlds.Count;
                }
                if(initalize_count != generatedWorlds.Count)
                {
                    Console.WriteLine("Error");
                }
                generatedWorlds.Clear();
                Random.Shared.Shuffle(CollectionsMarshal.AsSpan(randomConstraints));
                // Console.WriteLine($"Generated worlds: {generatedWorlds.Count}");
            }
            // Console.WriteLine("moving on from "+initalize_count);
        }
        
    }
    public Tester(ClaimWorld claimWorld)
    {
        ClaimWorld = claimWorld;
    }
    public static void TestWorld(ClaimWorld claimWorld,List<ClaimConstraint> claimConstraints, List<Constraint> debugConstraints)
    {
        var initalize_count = -1;
        World w = new World(claimWorld,claimConstraints,debugConstraints);
        w.Constraints.TryPeek(out var firstConstraint, out var firtPriority);
        while(true)
        {
                
            var generatedWorlds = ((World)w.Clone()).CalculateWorldBetter(false);
            // var generatedWorlds = ((World)world.Clone()).CalculateWorldBetter();
            if(initalize_count==-1)
            {
                initalize_count = generatedWorlds.Count;
            }
            if(initalize_count != generatedWorlds.Count)
            {
                Console.WriteLine("Error");
            }
            Console.WriteLine(generatedWorlds.Count);
            generatedWorlds.Clear();
             
            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(claimConstraints));
            w.Constraints.Enqueue(firstConstraint,firtPriority);
        }
    }
}