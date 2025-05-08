namespace ClocktowerWorldBuilder;

public abstract class Role
{
    public RoleTemplate GetRoleTemplate()
    {
        return new RoleTemplate(this.GetType());
    }

}