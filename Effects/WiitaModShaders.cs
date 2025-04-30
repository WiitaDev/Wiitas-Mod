using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace WiitaMod.Effects
{
    public class WiitaModShaders : ModSystem
    {
        public const string ShaderPrefix = "WiitaMod:";

        internal static Asset<Effect> WaterStreamEffect;
        internal static Asset<Effect> StandardPrimitiveShader;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            LoadShaders();
        }

        private void LoadShaders()
        {
            // Load shaders as assets
            WaterStreamEffect = Mod.Assets.Request<Effect>("Effects/WaterStreamEffect");
            StandardPrimitiveShader = Mod.Assets.Request<Effect>("Effects/StandardPrimitiveShader");

            // Register shaders using the asset references
            RegisterMiscShader(WaterStreamEffect, "TrailPass", "WaterStream");
            RegisterMiscShader(StandardPrimitiveShader, "PrimitivePass", "StandardPrimitiveShader");
        }

        private void RegisterMiscShader(Asset<Effect> effectAsset, string passName, string registrationName)
        {
            GameShaders.Misc[$"{ShaderPrefix}{registrationName}"] =
                new MiscShaderData(effectAsset, passName);
        }

        private void RegisterScreenShader(Asset<Effect> effectAsset, string passName, string registrationName)
        {
            Filters.Scene[$"{ShaderPrefix}{registrationName}"] =
                new Filter(new ScreenShaderData(effectAsset, passName), EffectPriority.Medium);
        }

        public override void Unload()
        {
            WaterStreamEffect = null;
            StandardPrimitiveShader = null;

            // Unregister shaders
            GameShaders.Misc.Remove($"{ShaderPrefix}WaterStream");
            GameShaders.Misc.Remove($"{ShaderPrefix}StandardPrimitiveShader");
        }
    }
}