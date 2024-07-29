using Terraria;
using Terraria.ModLoader;

namespace WiitaMod.Systems.ParticleSystems
{
    public class ParticleUpdate : ModSystem
    {
        public override void PostUpdateEverything()
        {
            if (!Main.dedServ)
            {
                ParticleManager.UpdateAllParticles();
            }
        }
    }
}