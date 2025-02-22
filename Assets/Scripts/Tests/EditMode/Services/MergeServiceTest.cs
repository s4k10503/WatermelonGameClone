using Domain.Services;

using System.Numerics;
using NUnit.Framework;

namespace Tests.EditMode.Services
{
    public class MergeServiceTest
    {
        private MergeService _mergeService;

        [SetUp]
        public void SetUp()
        {
            _mergeService = new MergeService();
        }

        [Test]
        public void GenerateNextItemIndex_ShouldReturnValueWithinExpectedRange()
        {
            // Specify maxItemNo value to be used in the test
            int maxItemNo = 10;
            int expectedMaxIndex = maxItemNo / 2 - 1;

            // Check that the value returned by the method is within the expected range
            int result = _mergeService.GenerateNextItemIndex(maxItemNo);

            // Assert: Check if the result is within the range of 0 to expectedMaxIndex
            Assert.That(result, Is.InRange(0, expectedMaxIndex));
        }

        [Test]
        public void GenerateNextItemIndex_WhenMaxItemNoIsSmall_ShouldReturnZero()
        {
            // Test that if maxItemNo is small, the result is always 0
            int maxItemNo = 2;
            int result = _mergeService.GenerateNextItemIndex(maxItemNo);

            // Assert: Ensure the result is 0
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GenerateNextItemIndex_WhenMaxItemNoIsLarge_ShouldReturnValueWithinExpectedRange()
        {
            // Confirm that the results fall within the correct range even for large maxItemNo
            int maxItemNo = 100;
            int expectedMaxIndex = maxItemNo / 2 - 1;

            int result = _mergeService.GenerateNextItemIndex(maxItemNo);

            // Assert: Check if the result is within the range of 0 to expectedMaxIndex
            Assert.That(result, Is.InRange(0, expectedMaxIndex));
        }

        [Test]
        public void CreateMergeData_WhenItemNumberIsValid_ShouldReturnCorrectMergeData()
        {
            // Arrange
            Vector2 sourcePosition = new(0, 0);
            Vector2 targetPosition = new(2, 2);
            int itemNo = 0;

            // Act
            var mergeData = _mergeService.CreateMergeData(sourcePosition, targetPosition, itemNo);

            // Assert
            Assert.AreEqual(new Vector2(1, 1), mergeData.Position, "Merge position should be the midpoint of source and target positions.");
            Assert.AreEqual(itemNo + 1, mergeData.ItemNo, "MergeData should contain the correct item number.");
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
                _mergeService.CreateMergeData(sourcePosition, targetPosition, itemNo),
                "CreateMergeData should throw DomainException when item number is negative.");
        }

        [TearDown]
        public void TearDown()
        {
            _mergeService = null;
        }
    }
}
