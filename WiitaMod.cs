using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Prim;

namespace WiitaMod
{
    public class WiitaMod : Mod
    {

        public static BasicEffect basicEffect;
        public static Effect TestEffect;
        public static Effect StarfirePrims;
        public static Effect PrimitiveTextureMap;

        public static PrimTrailManager primitives;

        public override void Load()
        {
            PrimitiveTextureMap = ModContent.Request<Effect>("WiitaMod/Effects/PrimitiveTextureMap", AssetRequestMode.ImmediateLoad).Value;

            if (Main.netMode != NetmodeID.Server)
            {
                Ref<Effect> screenRef = new Ref<Effect>(ModContent.Request<Effect>("WiitaMod/Effects/ShockwaveEffect", AssetRequestMode.ImmediateLoad).Value); // The path to the compiled shader file.
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.High);
                Filters.Scene["Shockwave"].Load();

            }

            int width = Main.graphics.GraphicsDevice.Viewport.Width;
            int height = Main.graphics.GraphicsDevice.Viewport.Height;
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
            Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);

            Main.QueueMainThreadAction(() =>
            {
                basicEffect = new BasicEffect(Main.graphics.GraphicsDevice)
                {
                    VertexColorEnabled = true,
                    View = view,
                    Projection = projection
                };
            });

            primitives = new PrimTrailManager();
            primitives.LoadContent(Main.graphics.GraphicsDevice);
        }


        public override void Unload()
        {
            TestEffect = null;
            PrimitiveTextureMap = null;

            primitives = null;
        }

        [System.Obsolete]
        public override void AddRecipeGroups()/* tModPorter Note: Removed. Use ModSystem.AddRecipeGroups */
        {
            RecipeGroup group = new RecipeGroup(() => Lang.misc[37] + " Mythril Bar", new int[]
            {
                382,
                1191
            });
            RecipeGroup.RegisterGroup("MythrilOrOrichalcium", group);
        }

    }
}