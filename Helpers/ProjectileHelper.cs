using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace WiitaMod.Helpers;

public static class ProjectileHelper
{
    public static void Explode(int index, int Width, int Height, bool gore = true, bool dust = true) // thanks spirit mod!!
    {
        Projectile projectile = Main.projectile[index];

        if (!projectile.active)
            return;

        projectile.tileCollide = false;
        projectile.alpha = 255;
        Vector2 center = projectile.Center;
        projectile.width = Width;
        projectile.height = Height;
        projectile.Center = center;
        projectile.penetrate = -1;

        projectile.Damage();


        if (dust)
        {
            for (int i = 0; i < 30; i++)
            {
                int num = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                Main.dust[num].velocity *= 1.4f;
            }

            for (int j = 0; j < 20; j++)
            {
                int num2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 7f;
                num2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[num2].velocity *= 3f;
            }
        }

        if (gore) {
            for (int k = 0; k < 8; k++)
            {
                float scaleFactor = 0.4f;
                if (k > 3)
                    scaleFactor = 0.8f;

                Gore smoke = Main.gore[Gore.NewGore(projectile.GetSource_Death("Explosion"), projectile.position + Main.rand.NextVector2Square(0, projectile.width), default, Main.rand.Next(61, 64), 1f)];
                smoke.velocity *= scaleFactor;

                float negateX = k > 3 ? -1 : 1;
                float negateY = k == 2 || k == 3 || k > 5 ? -1 : 1;

                smoke.velocity.X += 1f * negateX;
                smoke.velocity.Y += 1f * negateY;
            }
        }
    }
}
