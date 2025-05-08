using System.Collections.Immutable;
using System.Diagnostics;

namespace ClocktowerWorldBuilder;

public record RoleTemplate
{
    public Type UnderlyingType;
    public RoleType RoleType;
    public Alignment Alignment;
    public string Name;

    public readonly ImmutableArray<int> OutsiderDeltas;
    //USE THIS FOR EVIL CHARACTERS WHO DO NOT HAVE CLAIMS
    //EVIL CHARACTERS MUST HAVE A DEFAULT CONSTRUCTOR
    public RoleTemplate(Type underlyingType)
    {
        UnderlyingType = underlyingType;
        var v = underlyingType.GetField("RoleType").GetValue(null);
        Debug.Assert(v != null);
        RoleType = (RoleType)v;
        var al  = underlyingType.GetField("Alignment").GetValue(null);
        Debug.Assert(al != null);
        Alignment = (Alignment)al;
        
        var outsiders = underlyingType.GetField("OutsiderDeltas");
        if (outsiders == null)
        {
            OutsiderDeltas = [];
        }
        else
        {
            var outsiderDeltas = outsiders.GetValue(null);
            Debug.Assert(outsiderDeltas != null);
            OutsiderDeltas = (ImmutableArray<int>)outsiderDeltas;
        }
        
        UnderlyingType = underlyingType;
        var v5 = underlyingType.GetField("Name").GetValue(null);
        Debug.Assert(v5 != null);
        Name = (string) v5 ;
    }
}