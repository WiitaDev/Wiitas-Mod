using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.NPCs.Bosses;

namespace WiitaMod.Projectiles.Magic
{
    public class CrabBossSpawnEffect : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.FairyQueenHymn}";

        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Palladin's shield");     //The English name of the projectile
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.scale = 1f;
            Projectile.width = 5;               //The width of projectile hitbox
            Projectile.height = 5;              //The height of projectile hitbox
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
        }
        public override void AI()
        {
            Time++;
            Projectile.velocity = new Vector2(0, -1f);

            if (Time == 350)
            {
                int id = NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y - 64, ModContent.NPCType<CrabBoss>(), 1);

                if (id != -1)
                {
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, id);
                    }
                }
            }

            for (int i = 0; i < 20; i++)
            {
                Vector2 circle = Main.rand.NextVector2CircularEdge(8, 8);
                int bigdust = Dust.NewDust(Projectile.position + circle, 5, 5, DustID.WaterCandle, circle.X * 2, circle.Y * 2);
                Main.dust[bigdust].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.LargeBlueTrail).Draw(Projectile);
            return false;
        }

    }
}