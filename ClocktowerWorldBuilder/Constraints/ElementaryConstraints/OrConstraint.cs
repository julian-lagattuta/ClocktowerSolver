using System.Diagnostics;

namespace ClocktowerWorldBuilder.Condition;

public class OrConstraint: Constraint
{
    public List<Constraint> Constraints;

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
    public OrConstraint(List<Constraint> constraints)
    {
        Debug.Assert(constraints.Count>0);
        Constraints = constraints;
    }
    public override float GetPriority(ClaimWorld claimWorld)
    {
        
        List<float> complements = Constraints.Take(Constraints.Count-1).Select(n => n.Complement.Priority).ToList();
        
        List<float>  priorities= Constraints.Take(Constraints.Count-1).Select(n => n.Priority).ToList();
        
        float totalPriority = 0;  
        
        for(int i = 0;i<Constraints.Count;i++)
        {

            var product = 1f;
            foreach (var  compPriority in complements.Take(i))
            {
                product *= compPriority;
            }

            product *= Constraints[i].Priority; 
            totalPriority += product;
        }
        return totalPriority;
    }

    public override void FixPriority(float newPriority)
    {
        base.FixPriority(newPriority);
        foreach (Constraint constraint in Constraints)
        {
            constraint.FixPriority(newPriority);
        }
    }
    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new AndConstraint(Constraints.Select(c => c.Complement).ToList());
    }
    public override Constraint FetchComplement(ClaimWorld claimWorld)
    {
        return new AndConstraint(Constraints.Select(c => c.FetchComplement(claimWorld)).ToList());
    }


    public override List<World> PotentialWorlds(World world)
    {
        
        List<World> worlds = new List<World>();
        List<Constraint> complements = Constraints.Select(n => n.Complement).ToList();
        int i = -1;
        foreach (Constraint constraint in Constraints)
        {
            i++;
            World worldClone = (World) world.Clone();
            // List<Constraint> falseConstraints = complements.GetRange(0, i);
            foreach(var falseConstraint in complements.Take(i))
            {
                worldClone.Constraints.Enqueue(falseConstraint, falseConstraint.Priority);
            }
            worldClone.Constraints.Enqueue(constraint,constraint.Priority);
            worldClone.DebugAppliedConstraints.AddRange(complements.Take(i));
            worldClone.DebugAppliedConstraints.Add(constraint);
            worlds.Add(worldClone); 
        }
        return worlds;
    }

}