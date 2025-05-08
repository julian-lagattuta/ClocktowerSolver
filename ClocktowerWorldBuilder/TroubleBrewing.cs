using ClocktowerWorldBuilder.Roles.Minions;
using ClocktowerWorldBuilder.Roles.Outsiders;
using ClocktowerWorldBuilder.Roles.Townsfolk;

namespace ClocktowerWorldBuilder;

public class TroubleBrewing: Script
{
    public static List<RoleTemplate> TBRoles = [
        new RoleTemplate(typeof(Washerwoman)),
        new RoleTemplate(typeof(Librarian)),
        new RoleTemplate(typeof(Investigator)),
        new RoleTemplate(typeof(Empath)),
        new RoleTemplate(typeof(FortuneTeller)),
        new RoleTemplate(typeof(Undertaker)),
        new RoleTemplate(typeof(Monk)),
        new RoleTemplate(typeof(Ravenkeeper)),
        new RoleTemplate(typeof(Virgin)),
        new RoleTemplate(typeof(Slayer)),
        new RoleTemplate(typeof(Soldier)),
        new RoleTemplate(typeof(Mayor)),

        new RoleTemplate(typeof(Butler)),
        new RoleTemplate(typeof(Drunk)),       
        new RoleTemplate(typeof(Saint)),
        new RoleTemplate(typeof(Recluse)),    

        // Minions
        new RoleTemplate(typeof(Poisoner)),  
        new RoleTemplate(typeof(Spy)),        
        new RoleTemplate(typeof(ScarletWoman)),
        new RoleTemplate(typeof(Baron)),     

        // Demon
        new RoleTemplate(typeof(Imp))       
    ];
    public TroubleBrewing(int playerCount) : base(TBRoles, playerCount)
    {
    }
}