using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using WiitaMod.Items.Consumable;
using WiitaMod.Items.Placeable;
using WiitaMod.NPCs.Bosses;
using WiitaMod.Projectiles.Magic;

namespace WiitaMod.Tiles
{
    public class CrabBossAltar : ModTile
    {
        public override string Texture => $"Terraria/Images/Tiles_{TileID.DefendersForge}";
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(10, 75, 245), name);

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.GeneralPlacementTiles[Type] = false;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;

            MinPick = 110;
        }

        public override bool CanExplode(int i, int j) => false;

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<CrabBossSummonItem>();
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX / 18;
            int top = j - tile.TileFrameY / 18;

            Player player = Main.LocalPlayer;
            int requiredItemType = ModContent.ItemType<CrabBossSummonItem>();
            int type = ModContent.NPCType<CrabBoss>();

            if (IsBossAlive(type) || Helper.CountProjectiles(ModContent.ProjectileType<CrabBossSpawnEffect>()) > 0)
                return true;

            if (player.HasItem(requiredItemType))
            {
                // Consume the item
                Main.LocalPlayer.ConsumeItem(requiredItemType, true);

                SoundEngine.PlaySound(SoundID.Item145, player.position);

                Vector2 spawnPosition = new Vector2(left + 1, top).ToWorldCoordinates();

                //spawn the effect projectile (that later spawns the boss)
                Projectile.NewProjectile(new EntitySource_WorldEvent(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<CrabBossSpawnEffect>(), 0, 0f, Main.myPlayer, 0f);
            }
            else
            {
                Main.NewText("You need a pixelated shit turd of a crab to activate the altar.", 255, 0, 0);
            }

            return true;
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        private bool IsBossAlive(int bossType)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.type == bossType)
                {
                    return true;
                }
            }
            return false;
        }
    }
}