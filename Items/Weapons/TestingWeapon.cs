using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Ranger.BassArrows;

namespace WiitaMod.Items.Weapons
{
    public class TestingWeapon : ModItem 
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 50;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TestingWeaponProj>();
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 9f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<TestingWeaponProj>();
            damage = 0;
        }
    }
    public class TestingWeaponProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Fireball}";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Kazimierz Seraphim");     //The English name of the projectile
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 150;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;        //The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;               //The width of projectile hitbox
            Projectile.height = 70;              //The height of projectile hitbox
            Projectile.aiStyle = 1;             //The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;         //Can the projectile deal damage to enemies?
            Projectile.hostile = false;         //Can the projectile deal damage to the player?
            Projectile.penetrate = -1;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.alpha = 0;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f;            //How much light emit around the projectile
            Projectile.ignoreWater = true;          //Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false;          //Can the projectile collide with tiles?
            Projectile.extraUpdates = 10;            //Set to above 0 if you want the projectile to update multiple time in a frame
        }
        float rotationSpeed = 10f;
        public override void AI()
        {
            Projectile.timeLeft = 10;
            Player player = Main.player[Projectile.owner];
            NPC npc = Main.npc[(int)Projectile.ai[2]];
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            //Factors for calculations
            double deg = Projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
            double rad = deg * (Math.PI / 180); //Convert degrees to radians
            double dist = npc.height * 1.5; //Distance away from the npc

            /*Position the player based on where the player is, the Sin/Cos of the angle times the /
            /distance for the desired distance away from the player minus the projectile's width   /
            /and height divided by two so the center of the projectile is at the right place.     */
            Projectile.position.X = npc.Center.X - (int)(Math.Sin(rad) * dist) - npc.height;
            Projectile.position.Y = npc.Center.Y - (int)(Math.Cos(rad) * dist) - npc.height;

            //Increase the counter/angle in degrees by 1 point, you can change the rate here too, but the orbit may look choppy depending on the value
            Projectile.ai[1] += 5f;


            float rotationsPerSecond = rotationSpeed;
            rotationSpeed -= 0.4f;
            bool rotateClockwise = true;
            //The rotation is set here
            Projectile.rotation += (rotateClockwise ? 1 : -1) * MathHelper.ToRadians(rotationsPerSecond * 6f);


        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.RingTrail).Draw(Projectile);

            return false;
        }

    }
}