using Terraria;
using Terraria.ModLoader;

namespace WiitaMod.Particles.ParticleSystems;

public class ParticleILEdits : ModSystem
{
    public override void OnModLoad()
    {
        On_Main.DrawInfernoRings += DrawParticles;
    }

    private static void DrawParticles(On_Main.orig_DrawInfernoRings orig, Main self)
    {
        ParticleManager.DrawAllParticles(Main.spriteBatch);

        orig(self);
    }
}