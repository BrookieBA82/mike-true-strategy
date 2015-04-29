using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            baseSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            baseSpec.HandCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            baseSpec.HandCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            baseSpec.HandCardCountsByTypeId[TestSetup.CardTypeLibrary.Id] = 1;

            baseSpec.Play = TestSetup.CardTypeLibrary;

            return baseSpec;
        }

        #endregion

        #region Tests

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryDrawNonAction() {

        }

        [TestCategory("LibraryActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestLibraryActionDiscardedAction() {

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
    }
}