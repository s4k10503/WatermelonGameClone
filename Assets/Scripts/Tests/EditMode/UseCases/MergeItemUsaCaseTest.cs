using NUnit.Framework;
using NSubstitute;
using WatermelonGameClone.Domain;
using WatermelonGameClone.UseCase;

namespace WatermelonGameClone.Tests
{
    public class MergeItemUseCaseTests
    {
        private MergeItemUseCase _mergeItemUseCase;
        private IMergeItemIndexService _mockMergeItemIndexService;
        private IGameOverJudgmentService _mockGameOverJudgmentService;
        private IGameRuleSettingsRepository _mockGameRuleSettingsRepository;

        [SetUp]
        public void SetUp()
        {
            // Mock creation
            _mockMergeItemIndexService = Substitute.For<IMergeItemIndexService>();
            _mockGameOverJudgmentService = Substitute.For<IGameOverJudgmentService>();
            _mockGameRuleSettingsRepository = Substitute.For<IGameRuleSettingsRepository>();
            
            // Setup mock to return a default value
            _mockGameRuleSettingsRepository.GetContactTimeLimit().Returns(1.0f);

            // Specify Maxitemno to initialize the test target
            _mergeItemUseCase = new MergeItemUseCase(
                10, _mockMergeItemIndexService, _mockGameOverJudgmentService, _mockGameRuleSettingsRepository);
        }

        [Test]
        public void Constructor_ShouldInitializeWithMaxItemNo()
        {
            // Assert that MaxItemNo is set correctly
            Assert.AreEqual(10, _mergeItemUseCase.MaxItemNo);

            // Assert that the time limit is fetched correctly from the repository
            _mockGameRuleSettingsRepository.Received(1).GetContactTimeLimit();
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

        [Test]
        public void CheckGameOver_WhenContactTimeExceedsTimeLimit_ShouldReturnTrue()
        {
            // Arrange
            // Use the ContactTimelimit acquired in the rules
            float contactTime = 1.5f; // Time exceeding the upper limit
            float contactTimeLimit = _mockGameRuleSettingsRepository.GetContactTimeLimit();
            _mockGameOverJudgmentService.CheckGameOver(contactTime, contactTimeLimit).Returns(true);

            // Act
            bool result = _mergeItemUseCase.CheckGameOver(contactTime);

            // Assert
            Assert.IsTrue(result, "GameOver should be true when contact time exceeds time limit.");
        }

        [Test]
        public void CheckGameOver_WhenContactTimeIsEqualToTimeLimit_ShouldReturnFalse()
        {
            // Arrange
            float contactTime = 1.0f; // Equal to time limit
            float contactTimeLimit = _mockGameRuleSettingsRepository.GetContactTimeLimit();
            _mockGameOverJudgmentService.CheckGameOver(contactTime, contactTimeLimit).Returns(false);

            // Act
            bool result = _mergeItemUseCase.CheckGameOver(contactTime);

            // Assert
            Assert.IsFalse(result, "GameOver should be false when contact time is equal to time limit.");
        }

        [Test]
        public void CheckGameOver_WhenContactTimeIsLessThanTimeLimit_ShouldReturnFalse()
        {
            // Arrange
            float contactTime = 0.5f; // Less than time limit
            float contactTimeLimit = _mockGameRuleSettingsRepository.GetContactTimeLimit();
            _mockGameOverJudgmentService.CheckGameOver(contactTime, contactTimeLimit).Returns(false);

            // Act
            bool result = _mergeItemUseCase.CheckGameOver(contactTime);

            // Assert
            Assert.IsFalse(result, "GameOver should be false when contact time is less than time limit.");
        }

        [TearDown]
        public void TearDown()
        {
            // Release resources after the test
            _mergeItemUseCase.Dispose();
            _mergeItemUseCase = null;
        }
    }
}
