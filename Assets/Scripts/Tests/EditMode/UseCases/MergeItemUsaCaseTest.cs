using NUnit.Framework;
using NSubstitute;
using UniRx;
using WatermelonGameClone.Domain;
using WatermelonGameClone.UseCase;

namespace WatermelonGameClone.Tests
{
    public class MergeItemUseCaseTests
    {
        private MergeItemUseCase _mergeItemUseCase;
        private IMergeItemIndexService _mockMergeItemIndexService;

        [SetUp]
        public void SetUp()
        {
            // Mock creation
            _mockMergeItemIndexService = Substitute.For<IMergeItemIndexService>();

            // Specify Maxitemno to initialize the test target
            _mergeItemUseCase = new MergeItemUseCase(10, _mockMergeItemIndexService);
        }

        [Test]
        public void Constructor_ShouldInitializeWithMaxItemNo()
        {
            // Check if Maxitemno is set in the constructor
            Assert.AreEqual(10, _mergeItemUseCase.MaxItemNo);
        }

        [Test]
        public void UpdateNextItemIndex_ShouldUpdateNextItemIndexValue()
        {
            // Arrange
            // Mock settings to return 5 when GeneraTenextitemindex is called
            _mockMergeItemIndexService.GenerateNextItemIndex(Arg.Any<int>()).Returns(5);

            // Act
            _mergeItemUseCase.UpdateNextItemIndex();

            // Assert
            Assert.AreEqual(5, _mergeItemUseCase.NextItemIndex.Value);

            // Confirm that GenerateNextItiteMindex was called by Maxitemno
            _mockMergeItemIndexService.Received(1).GenerateNextItemIndex(10);
        }

        [TearDown]
        public void TearDown()
        {
            // Release resources after the test
            _mergeItemUseCase.Dispose();
        }
    }
}
