using Bastards.AddonHelpers;
using Bastards.Settings;
using BetterCore.Utils;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bastards {
    public class Bastards : MBSubModuleBase {
        public static Random Random = new Random();
        public static MCMSettings Settings { get; private set; }
        public static string ModName { get; private set; } = "Homesteads";

        private bool isInitialized = false;
        private bool isLoaded = false;

        //FIRST
        protected override void OnSubModuleLoad() {
            try {
                base.OnSubModuleLoad();

                if (isInitialized)
                    return;

                Harmony h = new("Bannerlord.Windwhistle." + ModName);

                h.PatchAll();
                SetupBastardEvents();

                isInitialized = true;
            } catch (Exception e) {
                NotifyHelper.ReportError(ModName, "OnSubModuleLoad threw exception " + e);
            }
        }

        //SECOND
        protected override void OnBeforeInitialModuleScreenSetAsRoot() {
            try {
                base.OnBeforeInitialModuleScreenSetAsRoot();

                if (isLoaded)
                    return;

                ModName = base.GetType().Assembly.GetName().Name;

                Settings = MCMSettings.Instance ?? throw new NullReferenceException("Settings are null");

                NotifyHelper.ChatMessage(ModName + " Loaded.", MsgType.Good);
                Integrations.BetterHealthLoaded = true;

                isLoaded = true;
            } catch (Exception e) {
                NotifyHelper.ReportError(ModName, "OnBeforeInitialModuleScreenSetAsRoot threw exception " + e);
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter) {
            if (game.GameType is Campaign) {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;

                campaignStarter.AddBehavior(new BastardCampaignBehavior(campaignStarter));
                campaignStarter.AddBehavior(new AIBastardConceptionCampaignBehavior());
            }
        }

        private void SetupBastardEvents() {
            BastardCampaignEvents.AddAction_OnPlayerBastardConceptionAttempt((hero) => ChangeRelationOnConceptionAttempt(hero));
            BastardCampaignEvents.AddAction_OnAIBastardConceptionAttempt((hero1, hero2) => ChangeRelationOnConceptionAttempt(hero1, hero2));
        }

        private void ChangeRelationOnConceptionAttempt(Hero hero1, Hero? hero2 = null) {
            if (hero2 == null)
                ChangeRelationAction.ApplyPlayerRelation(hero1, 2, false);
            else
                ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero1, hero2, 2);
        }
    }
}