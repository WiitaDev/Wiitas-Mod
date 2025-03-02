using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WiitaMod.Items.Weapons.Ranger;


namespace WiitaMod.Systems
{
    public class WiitaModPlayerDrawEffects : ModPlayer // Inspired by Calamity Mod public repository
    {
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            // Only draw if player is active and not dead
            if (!Player.dead && Player.active &&
                !Player.frozen && !Player.invis)
            {
                Player drawPlayer = drawInfo.drawPlayer;
                Item heldItem = drawPlayer.HeldItem;

                // Create dictionary of weapons and their backpack textures
                var weaponBackpacks = new Dictionary<int, string>
                {
                    [ModContent.ItemType<PressureWasher>()] = "WiitaMod/Assets/Textures/DrawLayers/PressureWasher_Backpack",
                    [ModContent.ItemType<ThundercorePressureWasher>()] = "WiitaMod/Assets/Textures/DrawLayers/ThundercorePressureWasher_Backpack"
                };

                // Check if held weapon has a backpack
                if (weaponBackpacks.TryGetValue(heldItem.type, out string texturePath))
                {
                    Texture2D backpackTexture = ModContent.Request<Texture2D>(texturePath).Value;

                    float xOffset = 8f;

                    // Position calculations
                    Vector2 position = new Vector2((int)(drawPlayer.position.X - Main.screenPosition.X + (drawPlayer.width / 2) - (xOffset * drawPlayer.direction)) - 4f * drawPlayer.direction, (int)(drawPlayer.position.Y - Main.screenPosition.Y + (drawPlayer.height / 2) + 2f * drawPlayer.gravDir - 8f * drawPlayer.gravDir + drawPlayer.gfxOffY));

                    // Create draw data
                    DrawData backpackDrawData = new DrawData(
                        backpackTexture,
                        position,
                        null,
                        Color.White,
                        Player.bodyRotation,
                        backpackTexture.Size() * 0.5f,
                        1f,
                        Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                        0
                    );

                    // Insert before default player layer
                    drawInfo.DrawDataCache.Insert(0, backpackDrawData);
                }
            }
        }
    }
}