using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;

namespace WiitaMod.Helpers
{
    public static partial class WiitaUtils
    {

        // Cached for efficiency purposes.
        internal static readonly FieldInfo UImageFieldMisc0 = typeof(MiscShaderData).GetField("_uImage0", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly FieldInfo UImageFieldMisc1 = typeof(MiscShaderData).GetField("_uImage1", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly FieldInfo UImageFieldArmor = typeof(ArmorShaderData).GetField("_uImage", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Manually sets the texture of a <see cref="MiscShaderData"/> instance, since vanilla's implementation only supports strings that access vanilla textures.
        /// </summary>
        /// <param name="shader">The shader to bind the texture to.</param>
        /// <param name="texture">The texture to bind.</param>
        public static MiscShaderData SetShaderTexture(this MiscShaderData shader, Asset<Texture2D> texture, int index = 1)
        {
            switch (index)
            {
                case 0:
                    UImageFieldMisc0.SetValue(shader, texture);
                    break;
                case 1:
                    UImageFieldMisc1.SetValue(shader, texture);
                    break;
            }
            return shader;
        }

        /// <summary>
        /// Sets the current render target to the provided one.
        /// </summary>
        /// <param name="target">The render target to swap to</param>
        /// <param name="flushColor">The color to clear the screen with. Transparent by default</param>
        public static void SwapTo(this RenderTarget2D target, Color? flushColor = null)
        {
            // If we are in the menu, a server, or any of these are null, return.
            if (Main.gameMenu || Main.dedServ || target is null || Main.instance.GraphicsDevice is null || Main.spriteBatch is null)
                return;

            Main.instance.GraphicsDevice.SetRenderTarget(target);
            Main.instance.GraphicsDevice.Clear(flushColor ?? Color.Transparent);
        }
    }
}
