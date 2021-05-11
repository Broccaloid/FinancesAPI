using FinancesApi.Controllers;
using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FinancesApi.Tests
{
    public class FinancialOperationsControllerTests
    {
        private readonly List<FinancialOperation> operations;

        public FinancialOperationsControllerTests()
        {
            operations = new List<FinancialOperation>()
            {
                new FinancialOperation()
                {
                    Id = 0,
                    BalanceChange = -1100.50m,
                    Type = "School",
                    Date = new DateTime(2001, 12, 12)
                },
                new FinancialOperation()
                {
                    Id = 1,
                    BalanceChange = 5100.50m,
                    Type = "Shop",
                    Date = new DateTime(2001, 12, 12)
                },
                new FinancialOperation()
                {
                    Id = 2,
                    BalanceChange = -7100m,
                    Type = "School",
                    Date = new DateTime(2001, 12, 12)
                },
                new FinancialOperation()
                {
                    Id = 3,
                    BalanceChange = 1100.50m,
                    Type = "Gig",
                    Date = new DateTime(2001, 12, 13)
                },
            };
        }

        [Fact]
        public void GetAllOperationsReturnsAllOperations()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FinancialOperationsController>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStatementCalculator = new Mock<IStatementCalculator>();

            mockUnitOfWork.Setup(unit => unit.FinancialOperations.GetAll()).Returns(operations);
            var controller = new FinancialOperationsController(mockUnitOfWork.Object, mockStatementCalculator.Object, mockLogger.Object);

            // Act
            var result = controller.GetAllOperations().Result as OkObjectResult;

            // Assert
            Assert.Equal(operations, result.Value);
        }

        [Fact]
        public void GetAllOperationsReturnsEmptyList()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FinancialOperationsController>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStatementCalculator = new Mock<IStatementCalculator>();

            mockUnitOfWork.Setup(unit => unit.FinancialOperations.GetAll()).Returns(new List<FinancialOperation>());
            var controller = new FinancialOperationsController(mockUnitOfWork.Object, mockStatementCalculator.Object, mockLogger.Object);

            // Act
            var result = controller.GetAllOperations().Result as NotFoundResult;

            // Assert
            Assert.Equal(controller.NotFound().StatusCode, result.StatusCode);
        }

        [Fact]
        public void GetAllIncomesReturnsAllIncomes()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FinancialOperationsController>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStatementCalculator = new Mock<IStatementCalculator>();

            mockUnitOfWork.Setup(unit => unit.FinancialOperations.GetAll()).Returns(operations);
            var controller = new FinancialOperationsController(mockUnitOfWork.Object, mockStatementCalculator.Object, mockLogger.Object);

            // Act
            var result = controller.GetAllIncomes().Result as OkObjectResult;
            var expected = operations.Where(o => o.BalanceChange > 0).ToList();

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void GetAllExpensesReturnsAllExpenses()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FinancialOperationsController>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStatementCalculator = new Mock<IStatementCalculator>();

            mockUnitOfWork.Setup(unit => unit.FinancialOperations.GetAll()).Returns(operations);
            var controller = new FinancialOperationsController(mockUnitOfWork.Object, mockStatementCalculator.Object, mockLogger.Object);

            // Act
            var result = controller.GetAllExpenses().Result as OkObjectResult;
            var expected = operations.Where(o => o.BalanceChange < 0).ToList();

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void GetDailyFinancialStatementReturnsFinancialStatement()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FinancialOperationsController>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStatementCalculator = new Mock<IStatementCalculator>();
            var expected = new FinancialStatement()
            {
                FinancialOperations = operations.Where(o => o.Date == new DateTime(2001, 12, 12)).ToList(),
                TotalExpense = 8200.50m,
                TotalIncome = 5100.50m
            };

            mockStatementCalculator.Setup(o => o.CaluculateStatement(new DateTime(2001, 12, 12), new DateTime(2001, 12, 12))).Returns(expected);
            var controller = new FinancialOperationsController(mockUnitOfWork.Object, mockStatementCalculator.Object, mockLogger.Object);

            // Act
            var result = controller.GetDailyFinancialStatement(new DateTime(2001, 12, 12)).Result as OkObjectResult;

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void GetFinancialStatementForTimePeriodReturnsFinancialStatement()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FinancialOperationsController>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStatementCalculator = new Mock<IStatementCalculator>();
            var expected = new FinancialStatement()
            {
                FinancialOperations = operations,
                TotalExpense = 8200.50m,
                TotalIncome = 5100.50m
            };

            mockStatementCalculator.Setup(o => o.CaluculateStatement(new DateTime(2001, 12, 12), new DateTime(2001, 12, 13))).Returns(expected);
            var controller = new FinancialOperationsController(mockUnitOfWork.Object, mockStatementCalculator.Object, mockLogger.Object);

            // Act
            var result = controller.GetFinancialStatementForTimePeriod(new DateTime(2001, 12, 12), new DateTime(2001, 12, 13)).Result as OkObjectResult;

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void AddListOfOperationsGetsOperationsCallsUnitOfWorkMethods()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FinancialOperationsController>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStatementCalculator = new Mock<IStatementCalculator>();
            mockUnitOfWork.SetupGet(unit => unit.FinancialOperations).Returns(new Mock<IFinancialOperationRepository>().Object);

            var controller = new FinancialOperationsController(mockUnitOfWork.Object, mockStatementCalculator.Object, mockLogger.Object);

            // Act
            controller.AddListOfOperations(operations);

            // Assert
            mockUnitOfWork.Verify(unit => unit.FinancialOperations.AddRange(operations));
            mockUnitOfWork.Verify(unit => unit.Save());
        }

        [Fact]
        public void AddOperationGetsOperationCallsUnitOfWorkMethods()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FinancialOperationsController>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStatementCalculator = new Mock<IStatementCalculator>();
            mockUnitOfWork.SetupGet(unit => unit.FinancialOperations).Returns(new Mock<IFinancialOperationRepository>().Object);

            var controller = new FinancialOperationsController(mockUnitOfWork.Object, mockStatementCalculator.Object, mockLogger.Object);

            // Act
            controller.AddOperation(operations[0]);

            // Assert
            mockUnitOfWork.Verify(unit => unit.FinancialOperations.Add(operations[0]));
            mockUnitOfWork.Verify(unit => unit.Save());
        }

        [Fact]
        public void ChangeOperationGetsOperationCallsUnitOfWorkMethods()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FinancialOperationsController>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStatementCalculator = new Mock<IStatementCalculator>();
            var mockRepository = new Mock<IFinancialOperationRepository>();

            mockRepository.Setup(repo => repo.GetById(operations[0].Id)).Returns(operations[0]);
            mockUnitOfWork.SetupGet(unit => unit.FinancialOperations).Returns(mockRepository.Object);

            var controller = new FinancialOperationsController(mockUnitOfWork.Object, mockStatementCalculator.Object, mockLogger.Object);

            // Act
            controller.ChangeOperation(operations[0]);

            // Assert
            mockRepository.Verify(repo => repo.GetById(operations[0].Id));
            mockUnitOfWork.Verify(unit => unit.Save());
        }

        [Fact]
        public void ChangeListOfOperationsGetsOperationsCallsUnitOfWorkMethods()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FinancialOperationsController>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStatementCalculator = new Mock<IStatementCalculator>();
            var mockRepository = new Mock<IFinancialOperationRepository>();

            foreach (var operation in operations)
            {
                mockRepository.Setup(repo => repo.GetById(operation.Id)).Returns(operation);
            }
            mockUnitOfWork.SetupGet(unit => unit.FinancialOperations).Returns(mockRepository.Object);

            var controller = new FinancialOperationsController(mockUnitOfWork.Object, mockStatementCalculator.Object, mockLogger.Object);

            // Act
            controller.ChangeListOfOperation(operations);

            // Assert
            foreach (var operation in operations)
            {
                mockRepository.Verify(repo => repo.GetById(operation.Id));
            }
            mockUnitOfWork.Verify(unit => unit.Save());
        }

        [Fact]
        public void DeleteListOfOperationsGetsOperationsCallsUnitOfWorkMethods()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FinancialOperationsController>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStatementCalculator = new Mock<IStatementCalculator>();
            var mockRepository = new Mock<IFinancialOperationRepository>();

            mockUnitOfWork.SetupGet(unit => unit.FinancialOperations).Returns(mockRepository.Object);

            var controller = new FinancialOperationsController(mockUnitOfWork.Object, mockStatementCalculator.Object, mockLogger.Object);

            // Act
            controller.DeleteListOfOperations(operations);

            // Assert
            mockRepository.Verify(repo => repo.RemoveRange(operations));
            mockUnitOfWork.Verify(unit => unit.Save());
        }

        [Fact]
        public void DeleteOperationGetsOperationsCallsUnitOfWorkMethods()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FinancialOperationsController>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStatementCalculator = new Mock<IStatementCalculator>();
            var mockRepository = new Mock<IFinancialOperationRepository>();

            mockUnitOfWork.SetupGet(unit => unit.FinancialOperations).Returns(mockRepository.Object);

            var controller = new FinancialOperationsController(mockUnitOfWork.Object, mockStatementCalculator.Object, mockLogger.Object);

            // Act
            controller.DeleteOperation(operations[0]);

            // Assert
            mockRepository.Verify(repo => repo.Remove(operations[0]));
            mockUnitOfWork.Verify(unit => unit.Save());
        }
    }
}
