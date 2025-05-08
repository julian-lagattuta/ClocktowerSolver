using System.Diagnostics;
using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;

namespace ClocktowerWorldBuilder.Condition.CommonConstraints;

public class EvilPlayerAsDemonAfterConstraint(Timestamp timestamp): AfterConstraint
{

    public override float GetPriority(ClaimWorld claimWorld)
    {
        return 1; 
    }
    public override Constraint CalculateComplement(ClaimWorld claimWorld)
    {
        //TODO I DONT THINK THIS IS ACCURATE
        return new SpecialComplementConstraint(new AlwaysConstraint(),this);
    }

    public override List<World> PotentialWorlds(World world)
    {
        List<int> aliveEvils = [];
        for(int i =0;i<world.Players.Count;i++)
        {
            if(world.ClaimWorld.PlayerStatuses[i].AliveOn(timestamp) && world.Players[i].Alignment==Alignment.Evil)
            {
                aliveEvils.Add(i); 
            }
        }
        Debug.Assert(aliveEvils.Count!=0);
        List<World> worlds = new List<World>();
        foreach(int player in aliveEvils)
        {
            
            World cloneWorld = (World)world.Clone();
            if (cloneWorld.SetImp(player,timestamp,world.ClaimWorld.PlayerStatuses[player].DeathDay().Before()))
            {
                worlds.Add(cloneWorld);
            }
        }

        return worlds;
    }

}