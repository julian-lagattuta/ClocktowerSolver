namespace ClocktowerWorldBuilder.Condition;

public abstract class Constraint
{
    public float Priority { get; set; }
    public Constraint? Complement { get; set; }
    public virtual void CalculateProperty(ClaimWorld claimWorld)
    {
        if(Complement == null)
        {
            Complement = CalculateComplement(claimWorld);
            Complement.Complement = this;
            Complement.CalculateProperty(claimWorld);
        }

        Priority = GetPriority(claimWorld);
    }
    /// <summary>
    /// THIS IS TO SET SOMETHING TO BE DONE FIRST SUCH AS OUTSIDER COUNT
    /// </summary>
    /// <param name="newPriority"></param>
    public virtual void FixPriority(float newPriority)
    {
        Priority = newPriority;
        Complement.Priority = newPriority;
    }
    public virtual Constraint FetchComplement(ClaimWorld claimWorld)
    {
        return CalculateComplement(claimWorld);
    }
    /// <summary>
    /// This function is meant to be called ONLY by CalculateProperty. 
    /// </summary>
    /// <param name="claimWorld"></param>
    /// <returns></returns>
    public abstract float GetPriority(ClaimWorld claimWorld);
    /// <summary>
    /// This function is meant to be called ONLY by CalculateProperty. If you intend to quickly grab the complement, use FetchComplement.
    /// </summary>
    /// <param name="claimWorld"></param>
    /// <returns></returns>
    public abstract Constraint CalculateComplement(ClaimWorld claimWorld);
    public abstract List<World> PotentialWorlds(World world);
    // public abstract Constraint Complement(ClaimWorld claimWorld);
    
}