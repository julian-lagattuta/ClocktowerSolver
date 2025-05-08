using System.Diagnostics;
using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

namespace ClocktowerWorldBuilder.Condition;

public class AndConstraint:  Constraint
{
    public List<Constraint> Constraints;
    public AndConstraint(List<Constraint> constraints)
    {
        Debug.Assert(constraints.Count>0);
        Constraints = constraints;
    }

    public override void CalculateProperty(ClaimWorld claimWorld)
    {
        foreach(var constraint in Constraints)
        {
            constraint.CalculateProperty(claimWorld);
        }
        if(Complement == null)
        {
            Complement = CalculateComplement(claimWorld);
            Complement.Complement = this;
            Complement.CalculateProperty(claimWorld);
        }
        Priority = GetPriority(claimWorld);
    }

   /// <summary>
   /// This function is meant to be called ONLY by CalculateProperty
   /// </summary>
   /// <param name="claimWorld"></param>
   /// <returns></returns>
    public override float GetPriority(ClaimWorld claimWorld)
    {
        
        var priority = 1.0f;
        foreach (Constraint constraint in Constraints)
        {
            priority *= constraint.Priority;
        }

        return priority;
    }

    public override void FixPriority(float newPriority)
    {
        base.FixPriority(newPriority);
        foreach (Constraint constraint in Constraints)
        {
            constraint.FixPriority(newPriority);
        }
    }

    public override Constraint FetchComplement(ClaimWorld claimWorld)
    {
        return new OrConstraint(Constraints.Select(c => c.FetchComplement(claimWorld)).ToList());
    }
    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new OrConstraint(Constraints.Select(c => c.Complement).ToList());
    }

    public override List<World> PotentialWorlds(World world)
    {
        World w = (World) world.Clone();
        foreach (var constraint in Constraints)
        {
            w.Constraints.Enqueue(constraint,constraint.Priority);
        }
        w.DebugAppliedConstraints.AddRange(Constraints.AsEnumerable());
        return [w];
    }

}