using NUnit.Framework;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Tests
{
    public class MergeItemIndexServiceTest
    {
        private MergeItemIndexService _mergeItemIndexService;

        [SetUp]
        public void SetUp()
        {
            _mergeItemIndexService = new MergeItemIndexService();
        }

        [Test]
        public void GenerateNextItemIndex_ShouldReturnValueWithinExpectedRange()
        {
            // Specify maxSphereNo value to be used in the test
            int maxSphereNo = 10;
            int expectedMaxIndex = maxSphereNo / 2 - 1;

            // Check that the value returned by the method is within the expected range
            int result = _mergeItemIndexService.GenerateNextItemIndex(maxSphereNo);

            // Assert: Check if the result is within the range of 0 to expectedMaxIndex
            Assert.That(result, Is.InRange(0, expectedMaxIndex));
        }

        [Test]
        public void GenerateNextItemIndex_WhenMaxSphereNoIsSmall_ShouldReturnZero()
        {
            // Test that if maxSphereNo is small, the result is always 0
            int maxSphereNo = 2;
            int result = _mergeItemIndexService.GenerateNextItemIndex(maxSphereNo);

            // Assert: Ensure the result is 0
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GenerateNextItemIndex_WhenMaxSphereNoIsLarge_ShouldReturnValueWithinExpectedRange()
        {
            // Confirm that the results fall within the correct range even for large maxSphereNo
            int maxSphereNo = 100;
            int expectedMaxIndex = maxSphereNo / 2 - 1;

            int result = _mergeItemIndexService.GenerateNextItemIndex(maxSphereNo);

            // Assert: Check if the result is within the range of 0 to expectedMaxIndex
            Assert.That(result, Is.InRange(0, expectedMaxIndex));
        }

        [TearDown]
        public void TearDown()
        {
            _mergeItemIndexService = null;
        }
    }
}
