using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class LibraryTests {

        #region Test Setup

        [TestInitialize]
        public void LibraryTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Base Specifications

        private static ActionTestSpecification GetBaseSpecification() {
            ActionTestSpecification baseSpec = new ActionTestSpecification();

            baseSpec.GameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            baseSpec.GameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            baseSpec.GameCardCountsByTypeId[TestSetup.CardTypeLibrary.Id] = 10;
            baseSpec.GameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            baseSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            baseSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            baseSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeLibrary.Id] = 1;
            baseSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 4;

            baseSpec.HandCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            baseSpec.HandCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            baseSpec.HandCardCountsByTypeId[TestSetup.CardTypeLibrary.Id] = 1;

            baseSpec.Play = TestSetup.CardTypeLibrary;
            baseSpec.ActionDescription = "after a library is played";

            return baseSpec;
        }

        #endregion

        #region Tests

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryDrawNonAction() {
            ActionTestSpecification customSpec = new ActionTestSpecification();

            customSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 5;
            customSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 0;

            ActionTestSpecification testSpec = ActionTestSpecification.ApplyOverrides(LibraryTests.GetBaseSpecification(), customSpec);

            Game game = TestSetup.GenerateStartingGame
              (2, testSpec.GameCardCountsByTypeId, testSpec.PlayerCardCountsByTypeId, testSpec.HandCardCountsByTypeId);

            Card libraryCard = TestUtilities.SetUpCardToPlay(game, testSpec.Play);

            // Play through card selection and action resoultion to fill the hand with estates, which cannot be discarded.
            game.PlayPhase();
            game.PlayPhase();
            
            /*
            TestUtilities.ConfirmCardPlayed(
                game,
                libraryCard,
                testSpec.ActionDescription
            );
            TestUtilities.ConfirmCardCount(
                game.State.CurrentPlayer.Hand, 
                7, 
                TestUtilities.CARD_LOCATION_HAND, 
                testSpec.ActionDescription
            );
            TestUtilities.ConfirmCardTypeCount(
                game.State.CurrentPlayer.Hand, 
                TestSetup.CardTypeEstate, 
                4, 
                TestUtilities.CARD_LOCATION_HAND, 
                testSpec.ActionDescription
            );
            TestUtilities.ConfirmCardCount(
                game.State.CurrentPlayer.Discard, 
                0, 
                TestUtilities.CARD_LOCATION_DISCARD, 
                testSpec.ActionDescription
            );
            */
        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryActionDiscardedAction() {
            ActionTestSpecification testSpec = LibraryTests.GetBaseSpecification();

            Game game = TestSetup.GenerateStartingGame
              (2, testSpec.GameCardCountsByTypeId, testSpec.PlayerCardCountsByTypeId, testSpec.HandCardCountsByTypeId);

            Card libraryCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, testSpec.Play);

            // Fill the hand with estates, which cannot be discarded.
            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = new ScriptedPlaySelectionStrategy(libraryCard);
            game.PlayPhase();
            
            /*
            TestUtilities.ConfirmCardPlayed(
                game,
                libraryCard,
                testSpec.ActionDescription
            );
            TestUtilities.ConfirmCardCount(
                game.State.CurrentPlayer.Hand, 
                7, 
                TestUtilities.CARD_LOCATION_HAND, 
                testSpec.ActionDescription
            );
            TestUtilities.ConfirmCardTypeCount(
                game.State.CurrentPlayer.Hand, 
                TestSetup.CardTypeEstate, 
                4, 
                TestUtilities.CARD_LOCATION_HAND, 
                testSpec.ActionDescription
            );
            TestUtilities.ConfirmCardCount(
                game.State.CurrentPlayer.Discard, 
                0, 
                TestUtilities.CARD_LOCATION_DISCARD, 
                testSpec.ActionDescription
            );
             */
        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryActionKeptAction() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryOutOfCardsToDrawAction() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryHandAlreadyTooFullToDrawAction() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryEmptyDrawTarget() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryInvalidDrawTarget() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryDuplicateDrawTarget() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryMultipleDrawTarget() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryEmptyChoiceTarget() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryInvalidChoiceTarget() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryDuplicateChoiceTarget() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryMultipleChoiceTarget() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryEmptyDiscardTarget() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryInvalidDiscardTarget() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryDuplicateDiscardTarget() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryMultipleDiscardTarget() {

        }

        #endregion

        #region Utility Methods

        private static ITargetable FindTargetActionForChoice(Game game, Type choiceType) {
            IAction choiceAction = game.State.NextPendingAction;
            Debug.Assert(choiceAction is BaseActionTargetAction, "Next action is not a choice between actions.");

            BaseActionTargetAction action = choiceAction as BaseActionTargetAction;
            foreach (ITargetable candidateAction in action.GetAllValidIndividualTargets(game)) {
                if (choiceType.Equals(candidateAction.GetType())) {
                    return candidateAction;
                }
            }

            return null;
        }

        #endregion
    }
}