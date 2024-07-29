using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.Consumables;
using WiitaMod.NPCs.Bosses;
using Terraria.Localization;

namespace WiitaMod.Systems.BossSystems
{
    public class BossChecklistSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            DoBossChecklistIntegration();
        }

        private void DoBossChecklistIntegration()
        {
            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
            {
                return;
            }


            if (bossChecklistMod.Version < new Version(1, 6))
            {
                return;
            }

            // The "LogBoss" method requires many parameters, defined separately below:

            // Your entry key can be used by other developers to submit mod-collaborative data to your entry. It should not be changed once defined
            string internalName = "TropicalCrabBoss";

            LocalizedText spawnInfo = Language.GetOrRegister("Mods.WiitaMod.BossChecklist.CrabBoss.SpawnInfo");

            // Value inferred from boss progression, see the wiki for details
            float weight = 3.7f; // after evil boss

            // Used for tracking checklist progress
            Func<bool> downed = () => DownedBossSystem.downedTropicalCrabBoss;

            // The NPC type of the boss
            int bossType = ModContent.NPCType<CrabBoss>();

            // The item used to summon the boss with (if available)
            int spawnItem = ModContent.ItemType<CrabBossSummonItem>();

            // "collectibles" like relic, trophy, mask, pet
            List<int> collectibles = new List<int>()
            {
                ItemID.PoopBlock,
                ItemID.CultistBossBag,
            };

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["spawnInfo"] = spawnInfo,
                    ["collectibles"] = collectibles,
                    // Other optional arguments as needed are inferred from the wiki
                }
            );

            // Other bosses or additional Mod.Call can be made here.
        }
    }
}