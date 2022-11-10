﻿using BastardChildren.Models;
using BastardChildren.StaticUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace BastardChildren {
    public class BastardCampaignBehavior : CampaignBehaviorBase {
        private CampaignGameStarter game;

        public BastardCampaignBehavior(CampaignGameStarter game) {
            this.game = game;

            AddBaseModDialogs();
            AddBecomeBastardDialogs();
            AddCrueltyDialogs();
            AddLegitimizeBastardDialogs();
        }

        public override void RegisterEvents() {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action( () => {
                foreach (Bastard bastard in SubModule.Bastards.ToList())
                    bastard.Tick();
            } ));

            CampaignEvents.BeforeHeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>( (hero1, hero2, killActionDetail, someBool) => {
                if (hero1 != null) {
                    Bastard? bastard = Utils.GetBastardFromHero(hero1);
                    if (bastard != null)
                        SubModule.Bastards.Remove(bastard);
                }
            } ));
        }

        public override void SyncData(IDataStore dataStore) {
            dataStore.SyncData("Bastards", ref SubModule.Bastards);
        }

        private void AddBaseModDialogs() {
            // hero_main_options = self explanatory
            // lord_pretalk = make them ask "anything else?"
            // close_window = EXIT // seems to cause attack bug when done on map, so avoid.
            // lord_talk_speak_diplomacy_2 = "There is something I'd like to discuss."

            // BASE MOD DIALOG

            // Bastard event engage
            game.AddPlayerLine("BastardsEventStart", "hero_main_options", "BastardsEventStartOutput",
                "I was thinking that you and I...",
                () => Conversation.BastardConceptionAllowed(), null, 100, null, null);

            // WHEN other hero has already been asked
            game.AddDialogLine("BastardsEventStartQuestionedAlreadyAsked", "BastardsEventStartOutput", "lord_pretalk",
                "I'm not in the mood for this.[rf:negative, rb:negative]",
                () => Conversation.BastardConceptionAlreadyAskedCooldown(), null, 110, null);
            // WHEN Other hero is interested
            game.AddDialogLine("BastardsEventStartQuestionedInterested", "BastardsEventStartOutput", "BastardsEventConfirmBastard",
                "...? You and I...?[rf:unsure, rb:unsure]",
                () => Conversation.BastardConceptionAccepted(), null, 100, null);
            // WHEN NOT interested
            game.AddDialogLine("BastardsEventStartQuestionedNotInterested", "BastardsEventStartOutput", "lord_pretalk",
                "...? I don't know what picture you're trying to paint here, but this conversation is boring me.[rf:very_negative_ag, rb:negative]",
                null, null, 90, null);

            // mad game
            game.AddPlayerLine("BastardsEventConfirmationYes", "BastardsEventConfirmBastard", "BastardsEventMakeBastard",
                "Will you ride with me for a while?",
                null, null, 100, null);
            game.AddPlayerLine("BastardsEventConfirmationNo", "BastardsEventConfirmBastard", "lord_pretalk",
                "Uh, nevermind.",
                null, null, 90, null);

            // OOOOOOO THREE POINT SHOT
            game.AddDialogLine("BastardsEventConfirmationReceived", "BastardsEventMakeBastard", "lord_pretalk",
                "Yes.. I think I would like that very much.[rf:positive, rb:positive]",
                null, () => Conversation.ConceiveBastard(), 100, null);
        }

        private void AddCrueltyDialogs() {
            game.AddPlayerLine("BastardsCrueltyEventStart", "hero_main_options", "BastardsCrueltyEventStartOutput",
                "Remove your clothing.",
                () => {
                    return SubModule.Config.GetValueBool("enableCruelty") &&
                    Hero.OneToOneConversationHero.IsPrisoner &&
                    Hero.OneToOneConversationHero.PartyBelongedToAsPrisoner.LeaderHero == Hero.MainHero;
                }, null, 100, null, null);

            game.AddDialogLine("BastardsCreultyEventConfirmation", "BastardsCrueltyEventStartOutput", "BastardsCruelyEventConfirm",
                "Not again..please.[rf:unsure]",
                () => Conversation.CrueltyReceivedCooldown(), null, 100, null);
            game.AddDialogLine("BastardsCreultyEventConfirmationTraumatized", "BastardsCrueltyEventStartOutput", "BastardsCruelyEventConfirm",
                "I'm sorry..? No! I won't submit to that![rf:unsure, rb:very_negative]",
                null, null, 90, null);

            game.AddPlayerLine("BastardsCrueltyEventConfirmationYes", "BastardsCruelyEventConfirm", "BastardsCrueltyEventMakeBastard",
                "I don't think you understand; I wasn't asking. Guards, hold them down.",
                null, null, 100, null);
            game.AddPlayerLine("BastardsCrueltyEventConfirmationNo", "BastardsCruelyEventConfirm", "lord_pretalk",
                "Fine. Next time I may not be so generous.",
                null, null, 90, null);

            game.AddDialogLine("BastardsCrueltyEventConfirmationReceived", "BastardsCrueltyEventMakeBastard", "close_window",
                "NO! GET AWAY![rf:very_negative, rb:very_negative]",
                null, () => Conversation.ConceiveBastard(true), 100, null);
        }

        private void AddBecomeBastardDialogs() {
            game.AddPlayerLine("BastardsBecomeHerosBastard", "lord_talk_speak_diplomacy_2", "BastardsBecomeHerosBastardOutput",
                "I believe I'm your bastard child.",
            null, null, 100, null, null);

            game.AddDialogLine("BastardsBecomeHerosBastardConfirmationStart", "BastardsBecomeHerosBastardOutput", "BastardsBecomeHerosBastardConfirmation",
                "Eh, are you sure about that...?[rf:unsure, rb:unsure]",
            null, null, 100, null);

            game.AddPlayerLine("BastardsBecomeHerosBastardConfirmationOptionYes", "BastardsBecomeHerosBastardConfirmation", "BastardsBecomeHerosBastardConfirmationYes",
                "Yea, just look at me and try to deny it.",
            null, null, 100, null, null);
            game.AddPlayerLine("BastardsBecomeHerosBastardConfirmationOptionNo", "BastardsBecomeHerosBastardConfirmation", "BastardsBecomeHerosBastardConfirmationNo",
                "Perhaps you are right. Apologies, been drinking too much lately.",
            null, null, 90, null, null);

            game.AddDialogLine("BastardsBecomeHerosBastardConfirmationEndYes", "BastardsBecomeHerosBastardConfirmationYes", "lord_pretalk",
                "Yea, I suppose so! You seem to have my features well enough![rf:very_positive_ag, rb:very_positive]",
            null, () => Conversation.BecomeBastardOfHero(), 100, null);
            game.AddDialogLine("BastardsBecomeHerosBastardConfirmationEndNo", "BastardsBecomeHerosBastardConfirmationNo", "lord_pretalk",
                "Ehm...[rf:negative_ag]",
            null, null, 100, null);
        }

        private void AddLegitimizeBastardDialogs() {
            game.AddPlayerLine("BastardsLegitimizeByPlayerStart", "hero_main_options", "BastardsLegitimizeByPlayerStartOutput",
                "I believe it's time you were made a true member to your family.",
            () => Conversation.PlayerCanLegitimizeBastard(), null, 100, null, null);

            game.AddDialogLine("BastardsLegitimizeByPlayerConfirmationStart", "BastardsLegitimizeByPlayerStartOutput", "BastardsLegitimizeByPlayerConfirmation",
                "That would be an honor! However, are you sure that would be wise at the moment? It could cost you quite a lot of influence among the others.[rf:very_positive, rb:very_positive]",
            null, null, 100, null);

            game.AddPlayerLine("BastardsLegitimizeByPlayerConfirmationOptionYes", "BastardsLegitimizeByPlayerConfirmation", "BastardsLegitimizeByPlayerConfirmationYes",
                "Let it be so.",
            null, null, 100, (out TextObject explain) => Conversation.HasInfluenceToLegitimize(out explain), null);
            game.AddPlayerLine("BastardsLegitimizeByPlayerConfirmationOptionNo", "BastardsLegitimizeByPlayerConfirmation", "BastardsLegitimizeByPlayerConfirmationNo",
                "Perhaps you are right. The time isn't right.",
            null, null, 90, null, null);

            game.AddDialogLine("BastardsLegitimizeByPlayerConfirmationEndYes", "BastardsLegitimizeByPlayerConfirmationYes", "lord_pretalk",
                "This is a great honor you bestow upon me! I will do my best to live up to your expectations.[rf:very_positive, rb:very_positive]",
            null, () => Conversation.LegitimizeBastard(), 100, null);

            game.AddDialogLine("BastardsLegitimizeByPlayerConfirmationEndNo", "BastardsLegitimizeByPlayerConfirmationNo", "lord_pretalk",
                "Alright... I will try to live up to your expectations.[rf:negative, rb:unsure]",
            null, null, 100, null);
        }
    }
    public class CustomSaveDefiner : SaveableTypeDefiner {
        public CustomSaveDefiner() : base(823997416) { }

        protected override void DefineClassTypes() {
            AddClassDefinition(typeof(Bastard), 1);
        }

        protected override void DefineContainerDefinitions() {
            ConstructContainerDefinition(typeof(List<Bastard>));
        }
    }
}
