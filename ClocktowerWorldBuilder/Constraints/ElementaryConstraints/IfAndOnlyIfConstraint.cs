using ClocktowerWorldBuilder.Condition;

namespace ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

public class IfAndOnlyIfConstraint: Constraint
{
    public Constraint A;
    public Constraint B;

    public IfAndOnlyIfConstraint(Constraint a, Constraint b)
    {
        A = a;
        B = b;
    }
    public override void CalculateProperty(ClaimWorld claimWorld)
    {
        A.CalculateProperty(claimWorld);
        B.CalculateProperty(claimWorld);
        if(Complement == null)
        {
            Complement = CalculateComplement(claimWorld);
            Complement.Complement = this;
            Complement.CalculateProperty(claimWorld);
        }
        Priority = GetPriority(claimWorld);
    }

    public override float GetPriority(ClaimWorld claimWorld)
    {
        
        return  A.Priority * B.Priority + A.Complement.Priority * B.Complement.Priority;

    }

    public override void FixPriority(float newPriority)
    {
        base.FixPriority(newPriority);
        A.FixPriority(newPriority);
        B.FixPriority(newPriority);
    }
    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        return new OrConstraint([new AndConstraint([A,B.Complement,A.Complement, B])]);
    }

    public override List<World> PotentialWorlds(World world)
    {
        World cloneWorld1 = (World)world.Clone();
        World cloneWorld2 = (World)world.Clone();
        
        cloneWorld1.Enqueue(A);
        cloneWorld1.Enqueue(B);
        cloneWorld2.Enqueue(A.Complement);
        cloneWorld2.Enqueue(B.Complement);

        return [cloneWorld1, cloneWorld2];

    }

}