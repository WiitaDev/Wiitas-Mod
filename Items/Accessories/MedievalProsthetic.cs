using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;

namespace WiitaMod.Items.Accessories
{
	public class MedievalProsthetic : ModItem
	{
		public override void SetStaticDefaults()
		{                                        
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.accessory = true;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.LightRed;
		}

		bool flag;
		int damageTimer = 0;
		public override void UpdateEquip(Player player)
		{
			player.statLifeMax2 -= player.statLifeMax2 / 20;

			if (player.velocity.Y != 0 && player.wings <= 0 && !player.mount.Active)  // not on the ground
			{
				if (flag)
				{
					player.wingTime *= 2;
					flag = false;
				}
                player.runAcceleration *= 1.5f;
                player.maxRunSpeed *= 1.3f;
				damageTimer = 0;
			}
			else if (!flag && player.velocity.Y == 0) //on the ground
			{
				flag = true;
			}
			
			if(damageTimer <= 0) {
				if (player.velocity.Y == 0 && player.velocity.X != 0 && !player.mount.Active) // on the ground and moving
				{
					if(player.whoAmI == Main.myPlayer)
						player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " tripped on a rock and died."), Main.rand.Next(1, 4), 0, armorPenetration: 9999, dodgeable: false, knockback: 0, cooldownCounter: 20);
				}
				damageTimer = 10;
			}
			else 
			{
				if(!Main.gamePaused)
				damageTimer--;
			}
		}

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Wood, 15);
			recipe.AddRecipeGroup("PrehardTier2", 3);
			recipe.AddIngredient(ItemID.SoulofFlight, 5);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}