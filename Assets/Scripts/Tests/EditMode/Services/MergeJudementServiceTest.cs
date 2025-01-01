using NUnit.Framework;
using UnityEngine;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Tests
{
    public class MergeJudgmentServiceTest
    {
        private MergeJudgmentService _mergeJudgmentService;

        [SetUp]
        public void SetUp()
        {
            _mergeJudgmentService = new MergeJudgmentService();
        }

        [Test]
        public void CanMerge_WhenItemNumbersAreEqual_ShouldReturnTrue()
        {
            // Arrange
            int currentItemNo = 1;
            int targetItemNo = 1;

            // Act
            bool result = _mergeJudgmentService.CanMerge(currentItemNo, targetItemNo);

            // Assert
            Assert.IsTrue(result, "CanMerge should return true when item numbers are equal.");
        }

        [Test]
        public void CanMerge_WhenItemNumbersAreNotEqual_ShouldReturnFalse()
        {
            // Arrange
            int currentItemNo = 1;
            int targetItemNo = 2;

            // Act
            bool result = _mergeJudgmentService.CanMerge(currentItemNo, targetItemNo);

            // Assert
            Assert.IsFalse(result, "CanMerge should return false when item numbers are not equal.");
        }

        [Test]
        public void CanMerge_WhenItemNumbersAreNegative_ShouldThrowDomainException()
        {
            // Arrange
            int currentItemNo = -1;
            int targetItemNo = -1;

            // Act & Assert
            Assert.Throws<DomainException>(() =>
                _mergeJudgmentService.CanMerge(currentItemNo, targetItemNo),
                "CanMerge should throw DomainException when item numbers are negative.");
        }

        [Test]
        public void CreateMergeData_WhenItemNumberIsValid_ShouldReturnCorrectMergeData()
        {
            // Arrange
            Vector2 sourcePosition = new(0, 0);
            Vector2 targetPosition = new(2, 2);
            int itemNo = 1;

            // Act
            var mergeData = _mergeJudgmentService.CreateMergeData(sourcePosition, targetPosition, itemNo);

            // Assert
            Assert.AreEqual(new Vector2(1, 1), mergeData.Position, "Merge position should be the midpoint of source and target positions.");
            Assert.AreEqual(itemNo, mergeData.ItemNo, "MergeData should contain the correct item number.");
        }

        [Test]
        public void CreateMergeData_WhenItemNumberIsNegative_ShouldThrowDomainException()
        {
            // Arrange
            Vector2 sourcePosition = new(0, 0);
            Vector2 targetPosition = new(2, 2);
            int itemNo = -1;

            // Act & Assert
            Assert.Throws<DomainException>(() =>
                _mergeJudgmentService.CreateMergeData(sourcePosition, targetPosition, itemNo),
                "CreateMergeData should throw DomainException when item number is negative.");
        }

        [TearDown]
        public void TearDown()
        {
            _mergeJudgmentService = null;
        }
    }
}
