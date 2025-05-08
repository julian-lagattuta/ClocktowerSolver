using System.Runtime.Serialization;

namespace ClocktowerWorldBuilder;
public class Player(int id, RoleTemplate? role) : ICloneable
{
    public int Id = id;
    public RoleTemplate? Role = role;
    public Alignment? Alignment  = null;
    public RoleType? RoleType = null;
    public bool IsNotOutsider = false;
    public HashSet<RoleTemplate>  ImpossibleRoles  = new HashSet<RoleTemplate>();
    public Player(int id) : this(id, null)
    {
    }

    public bool SetNotOutsider()
    {
        if (RoleType == ClocktowerWorldBuilder.RoleType.Outsider)
        {
            return false;
        }

        IsNotOutsider = true;
        return true;
    }
    private bool _SetAlignment(Alignment alignment)
    {
        if (Alignment != null && alignment != Alignment)
        {
            return false;
        }
        Alignment = alignment;
        return true;
    }

    public bool SetAlignment(Alignment alignment)
    {
        return _SetAlignment(alignment);
    }
    public bool AddImpossibleRole(RoleTemplate role)
    {
        if (Role == role)
        {
            return false;
        }
        ImpossibleRoles.Add(role);
        return true;
    }

    private bool _SetRoleType(RoleType roleType)
    {
        if (roleType == ClocktowerWorldBuilder.RoleType.Outsider && IsNotOutsider)
        {
            return false;
        }
        if (RoleType != null && RoleType != roleType)
        {
            return false;
        }

        if (Role != null && Role.RoleType != roleType)
        {
            return false;
        }

         
        RoleType = roleType;
        return true;
    }

    public bool SetRoleType(RoleType roleType)
    {
        if (!_SetAlignment(Util.AlignmentOfRoleType(roleType)))
        {
            return false;
        }
        return _SetRoleType(roleType);
    }
    public bool IsOutsider()
    {
        return RoleType == ClocktowerWorldBuilder.RoleType.Outsider || (Role != null && Role.RoleType == ClocktowerWorldBuilder.RoleType.Outsider);
    }
    private bool _SetRole(RoleTemplate role)
    {
        
        if (Role != role && Role != null)
        {
            return false;
        }

        if (Alignment != null && role.Alignment != Alignment)
        {
            return false;
        }

        if (ImpossibleRoles.Contains(role))
        {
            return false;
        }
        Role = role;
        Alignment = role.Alignment;
        return true;
    }

    public bool SetRole(RoleTemplate role)
    {
        if (!_SetRoleType(role.RoleType))
        {
            return false;
        }
        if (!_SetAlignment(role.Alignment))
        {
            return false;
        }

        return _SetRole(role);
    }

    public object Clone()
    {
        var p2 = (Player) MemberwiseClone();
        p2.ImpossibleRoles = new HashSet<RoleTemplate>(ImpossibleRoles);
        return p2;
    }
};