using Bastards.Models;
using Bastards.Settings;
using Bastards.StaticUtils;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;

namespace Bastards.Patches {
    [HarmonyPatch(typeof(Clan), nameof(Clan.SetLeader))]
    internal class SetClanLeaderPatch {
        [HarmonyPostfix]
        private static void Postfix(Hero leader) {
            if (!GlobalSettings<MCMSettings>.Instance.LegitimizeBastardHeirsEnabled) return;

            if (leader == null) return;

            Bastard? bastard = Utils.GetBastardFromHero(leader);
            if (bastard == null) return;

            bastard.Legitimize();
        }
    }
}
