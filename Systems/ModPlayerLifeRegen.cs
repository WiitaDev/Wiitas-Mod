using Terraria;
using Terraria.ModLoader;

namespace WiitaMod.Systems
{
    public partial class WiitaModPlayer : ModPlayer
    {
        public override void UpdateBadLifeRegen()
        {
            float totalNegativeLifeRegen = 0;

            void ApplyDoTDebuff(bool hasDebuff, int negativeLifeRegenToApply, bool immuneCondition = false)
            {
                if (!hasDebuff || immuneCondition)
                    return;

                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                totalNegativeLifeRegen += negativeLifeRegenToApply;
            }

            ApplyDoTDebuff(ProstheticDebuff, Main.rand.Next(12, 16));


            Player.lifeRegen -= (int)totalNegativeLifeRegen;
        }
    }
}
