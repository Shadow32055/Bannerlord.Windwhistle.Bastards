using Bastards.Models;
using Bastards.Settings;
using Bastards.StaticUtils;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;

namespace Bastards.Patches
{
    [HarmonyPatch(typeof(ConversationHelper), nameof(ConversationHelper.GetHeroRelationToHeroTextShort))]
    internal class EncyclopediaBastardInfoPatch {
        [HarmonyPostfix]
        private static void Postfix(ref string __result, Hero queriedHero, Hero baseHero) {
            if (GlobalSettings<MCMSettings>.Instance.DisableEncyclopediaChanges)
                return;

            if ((queriedHero.Father != null && queriedHero.Father == baseHero) || (queriedHero.Mother != null && queriedHero.Mother == baseHero)) {
                Bastard? bastard = Utils.GetBastardFromHero(queriedHero);
                if (bastard != null)
                    __result = Utils.GetLocalizedString("{=BastardEncyclopediaTitle}Bastard");
            }
        }
    }
}
