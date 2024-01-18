using Bastards.Models;
using HarmonyLib;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace Bastards.Patches {
    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "DailyTickHero")]
    internal class PregnancyDailyTickHeroPatch {
        [HarmonyPrefix]
        private static bool Prefix(Hero hero) {
            if (!hero.IsPregnant)
                return true;

            Bastard bastard;

            try {
                bastard = BastardCampaignBehavior.Instance.Bastards.First(x => x.hero == null && x.mother == hero);
            }
            catch (Exception) {
                return true;
            }

            bastard.Tick();
            return false;
        }
    }
}
