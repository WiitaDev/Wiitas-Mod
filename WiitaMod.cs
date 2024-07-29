using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems.ParticleSystems;

namespace WiitaMod
{
    public class WiitaMod : Mod
    {
        public override void Load()
        {
            if (!Main.dedServ)
            {
                Ref<Effect> screenRef = new Ref<Effect>(ModContent.Request<Effect>("WiitaMod/Effects/ShockwaveEffect", AssetRequestMode.ImmediateLoad).Value); // The path to the compiled shader file.
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.High);
                Filters.Scene["Shockwave"].Load();

                ParticleManager.RegisterParticles();
            }
        }

        public override void Unload()
        {
            ParticleManager.Unload();
        }

    }
}