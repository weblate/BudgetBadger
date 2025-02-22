﻿using BudgetBadger.Core.DataAccess;
using BudgetBadger.Core.LocalizedResources;
using BudgetBadger.Logic;
using BudgetBadger.Models;
using BudgetBadger.UnitTests.TestModels;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetBadger.UnitTests.Logic
{
    public class TransactionLogicUnitTests
    {
        IResourceContainer resourceContainer { get; set; }
        IPayeeDataAccess payeeDataAccess { get; set; }
        IEnvelopeDataAccess envelopeDataAccess { get; set; }
        ITransactionDataAccess transactionDataAccess { get; set; }
        IAccountDataAccess accountDataAccess { get; set; }
        TransactionLogic transactionLogic { get; set; }

        [SetUp]
        public void Setup()
        {
            accountDataAccess = A.Fake<IAccountDataAccess>();
            transactionDataAccess = A.Fake<ITransactionDataAccess>();
            envelopeDataAccess = A.Fake<IEnvelopeDataAccess>();
            payeeDataAccess = A.Fake<IPayeeDataAccess>();
            resourceContainer = A.Fake<IResourceContainer>();
            transactionLogic = new TransactionLogic(transactionDataAccess, accountDataAccess, payeeDataAccess, envelopeDataAccess, resourceContainer);
        }

        [Test]
        public async Task SoftDeleteTransaction_ActiveTransaction_Successful()
        {
            // arrange
            var activeTransaction = TestTransactions.ActiveTransaction.DeepCopy();

            A.CallTo(() => transactionDataAccess.ReadTransactionAsync(activeTransaction.Id)).Returns(activeTransaction);

            // act
            var result = await transactionLogic.SoftDeleteTransactionAsync(activeTransaction.Id);

            // assert
            Assert.IsTrue(result.Success);
        }

        [Test]
        public async Task SoftDeleteTransaction_ActiveTransaction_UpdatesTransaction()
        {
            // arrange
            var activeTransaction = TestTransactions.ActiveTransaction.DeepCopy();

            A.CallTo(() => transactionDataAccess.ReadTransactionAsync(activeTransaction.Id)).Returns(activeTransaction);

            // act
            var result = await transactionLogic.SoftDeleteTransactionAsync(activeTransaction.Id);

            // assert
            A.CallTo(() => transactionDataAccess.UpdateTransactionAsync(A<Transaction>.Ignored)).MustHaveHappened();
        }

        [Test]
        public async Task SoftDeleteTransaction_DeletedTransaction_Unsuccessful()
        {
            // arrange
            var deletedTransaction = TestTransactions.DeletedTransaction.DeepCopy();

            A.CallTo(() => transactionDataAccess.ReadTransactionAsync(deletedTransaction.Id)).Returns(deletedTransaction);

            // act
            var result = await transactionLogic.SoftDeleteTransactionAsync(deletedTransaction.Id);

            // assert
            Assert.IsFalse(result.Success);
        }

        [Test]
        public async Task SoftDeleteTransaction_NewTransaction_Unsuccessful()
        {
            // arrange
            var newTransaction = TestTransactions.NewTransaction.DeepCopy();

            A.CallTo(() => transactionDataAccess.ReadTransactionAsync(newTransaction.Id)).Returns(newTransaction);

            // act
            var result = await transactionLogic.SoftDeleteTransactionAsync(newTransaction.Id);

            // assert
            Assert.IsFalse(result.Success);
        }

        [Test]
        public async Task SoftDeleteTransaction_SplitTransactionTwoSplits_Successful()
        {
            // arrange
            var splitTransactions = TestTransactions.GetSplitTransactions(2);
            var splitTransactionToDelete = splitTransactions.FirstOrDefault();

            A.CallTo(() => transactionDataAccess.ReadTransactionAsync(splitTransactionToDelete.Id)).Returns(splitTransactionToDelete);
            A.CallTo(() => transactionDataAccess.ReadSplitTransactionsAsync(splitTransactionToDelete.SplitId.Value)).Returns(splitTransactions);

            // act
            var result = await transactionLogic.SoftDeleteTransactionAsync(splitTransactionToDelete.Id);

            //assert
            Assert.IsTrue(result.Success);
        }

        [Test]
        public async Task SoftDeleteTransaction_SplitTransactionTwoSplits_RemovesSplitIdFromTransaction()
        {
            // arrange
            var splitTransactions = TestTransactions.GetSplitTransactions(2);
            var splitTransactionToDelete = splitTransactions.FirstOrDefault();
            var remainingTransaction = splitTransactions.FirstOrDefault(s => s.Id != splitTransactionToDelete.Id);

            A.CallTo(() => transactionDataAccess.ReadTransactionAsync(splitTransactionToDelete.Id)).Returns(splitTransactionToDelete);
            A.CallTo(() => transactionDataAccess.ReadSplitTransactionsAsync(splitTransactionToDelete.SplitId.Value)).Returns(splitTransactions);

            // act
            var result = await transactionLogic.SoftDeleteTransactionAsync(splitTransactionToDelete.Id);

            //assert
            A.CallTo(() => transactionDataAccess.UpdateTransactionAsync(A<Transaction>.That.Matches(t => t.Id == splitTransactionToDelete.Id && !t.SplitId.HasValue))).MustHaveHappened();
        }

        [Test]
        public async Task SoftDeleteTransaction_SplitTransactionTwoSplits_RemovesSplitIdFromRemainingTransaction()
        {
            // arrange
            var splitTransactions = TestTransactions.GetSplitTransactions(2);
            var splitTransactionToDelete = splitTransactions.FirstOrDefault();
            var remainingTransaction = splitTransactions.FirstOrDefault(s => s.Id != splitTransactionToDelete.Id);

            A.CallTo(() => transactionDataAccess.ReadTransactionAsync(splitTransactionToDelete.Id)).Returns(splitTransactionToDelete);
            A.CallTo(() => transactionDataAccess.ReadSplitTransactionsAsync(splitTransactionToDelete.SplitId.Value)).Returns(splitTransactions);

            // act
            var result = await transactionLogic.SoftDeleteTransactionAsync(splitTransactionToDelete.Id);

            //assert
            A.CallTo(() => transactionDataAccess.UpdateTransactionAsync(A<Transaction>.That.Matches(t => t.Id == remainingTransaction.Id && !t.SplitId.HasValue))).MustHaveHappened();
        }

        [Test]
        public async Task SoftDeleteTransaction_SplitTransactionMoreThanTwoSplits_Successful([Random(3, 100, 5)] int splits)
        {
            // arrange
            var splitTransactions = TestTransactions.GetSplitTransactions(splits);
            var splitTransactionToDelete = splitTransactions.FirstOrDefault();

            A.CallTo(() => transactionDataAccess.ReadTransactionAsync(splitTransactionToDelete.Id)).Returns(splitTransactionToDelete);
            A.CallTo(() => transactionDataAccess.ReadSplitTransactionsAsync(splitTransactionToDelete.SplitId.Value)).Returns(splitTransactions);

            // act
            var result = await transactionLogic.SoftDeleteTransactionAsync(splitTransactionToDelete.Id);

            //assert
            Assert.IsTrue(result.Success);
        }

        [Test]
        public async Task SoftDeleteTransaction_SplitTransactionMoreThanTwoSplits_LeavesRemainingSplitTransactions([Random(3, 100, 5)] int splits)
        {
            // arrange
            var splitTransactions = TestTransactions.GetSplitTransactions(splits);
            var splitTransactionToDelete = splitTransactions.FirstOrDefault();
            var remainingTransaction = splitTransactions.FirstOrDefault(s => s.Id != splitTransactionToDelete.Id);

            A.CallTo(() => transactionDataAccess.ReadTransactionAsync(splitTransactionToDelete.Id)).Returns(splitTransactionToDelete);
            A.CallTo(() => transactionDataAccess.ReadSplitTransactionsAsync(splitTransactionToDelete.SplitId.Value)).Returns(splitTransactions);

            // act
            var result = await transactionLogic.SoftDeleteTransactionAsync(splitTransactionToDelete.Id);

            //assert
            A.CallTo(() => transactionDataAccess.UpdateTransactionAsync(A<Transaction>.That.Matches(t => t.Id == remainingTransaction.Id))).MustNotHaveHappened();
        }
    }
}
