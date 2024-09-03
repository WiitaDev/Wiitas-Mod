using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.Placeable;
using WiitaMod.Tiles;

namespace WiitaMod.Projectiles
{
    public class TropicalSandProjectile: ModProjectile
    {
        protected bool falling = true;
        protected int tileType;
        protected int dustType;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tropical Sand");
        }

        public override void SetDefaults()
        {
            Projectile.knockBack = 6f;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            tileType = ModContent.TileType<TropicalSand>();
            dustType = DustID.Sand;
        }

        public override void AI()
        {
            //Change the 5 to determine how much dust will spawn. lower for more, higher for less
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
                Main.dust[dust].velocity.X *= 0.4f;
            }

            Projectile.tileCollide = true;
            Projectile.localAI[1] = 0f;

            if (Projectile.ai[0] == 1f)
            {
                if (!falling)
                {
                    Projectile.ai[1] += 1f;

                    if (Projectile.ai[1] >= 60f)
                    {
                        Projectile.ai[1] = 60f;
                        Projectile.velocity.Y += 0.2f;
                    }
                }
                else
                    Projectile.velocity.Y += 0.41f;
            }
            else if (Projectile.ai[0] == 2f)
            {
                Projectile.velocity.Y += 0.2f;

                if (Projectile.velocity.X < -0.04f)
                    Projectile.velocity.X += 0.04f;
                else if (Projectile.velocity.X > 0.04f)
                    Projectile.velocity.X -= 0.04f;
                else
                    Projectile.velocity.X = 0f;
            }

            Projectile.rotation += 0.1f;

            if (Projectile.velocity.Y > 10f)
                Projectile.velocity.Y = 10f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (falling)
                Projectile.velocity = Collision.AnyCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, true);
            else
                Projectile.velocity = Collision.TileCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, fallThrough, fallThrough, 1);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Point p = Projectile.Center.ToTileCoordinates();
            // If the sand is dying outside the world border, cancel placing sand.
            if (p.X < 0 || p.X >= Main.maxTilesX || p.Y < 0 || p.Y >= Main.maxTilesY)
                return;
            Tile placer = Main.tile[p.X, p.Y];

            // If the sand hit a half brick, but was mostly going downwards (at a lower than 45 degree angle), then stack atop the half brick.
            if (placer.IsHalfBlock && Projectile.velocity.Y > 0f && Math.Abs(Projectile.velocity.Y) > Math.Abs(Projectile.velocity.X))
                placer = Main.tile[p.X, --p.Y];

            bool ValidTileBelow = true;
            bool SlopeTileBelow = false;

            // Attempt to place sand and unslope tile below if available
            // Under no circumstances can falling sand destroy minecart tracks.
            if (!placer.HasTile && placer.TileType != TileID.MinecartTrack)
            {
                if (p.Y + 1 < Main.maxTilesY)
                {
                    Tile under = Main.tile[p.X, p.Y + 1];
                    if (under.HasTile)
                    {
                        if (under.TileType == TileID.MinecartTrack)
                            ValidTileBelow = false;
                        else if (under.IsHalfBlock || under.Slope != 0)
                            SlopeTileBelow = true;
                    }
                }

                if (ValidTileBelow)
                {
                    bool PlacedBlock = WorldGen.PlaceTile(p.X, p.Y, tileType, false, true);
                    WorldGen.SquareTileFrame(p.X, p.Y);

                    if (PlacedBlock && SlopeTileBelow)
                    {
                        WorldGen.SlopeTile(p.X, p.Y + 1);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 14, p.X, p.Y + 1);
                    }
                    if (PlacedBlock && Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 1, p.X, p.Y, tileType);
                }
            }
            // Give the block back if you literally can't place it
            else
                Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.position, Projectile.width, Projectile.height, ModContent.ItemType<TropicalSandItem>());
        }
    }
}