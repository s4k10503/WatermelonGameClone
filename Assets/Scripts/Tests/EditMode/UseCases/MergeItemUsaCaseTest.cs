using NUnit.Framework;
using NSubstitute;
using WatermelonGameClone.Domain;
using WatermelonGameClone.UseCase;
using UnityEngine;

namespace WatermelonGameClone.Tests
{
    public class MergeItemUseCaseTests
    {
        private MergeItemUseCase _mergeItemUseCase;
        private IMergeItemIndexService _mockMergeItemIndexService;
        private IMergeJudgmentService _mockMergeJudgmentService;
        private IGameOverJudgmentService _mockGameOverJudgmentService;
        private IGameRuleSettingsRepository _mockGameRuleSettingsRepository;

        [SetUp]
        public void SetUp()
        {
            // Mock creation
            _mockMergeItemIndexService = Substitute.For<IMergeItemIndexService>();
            _mockMergeJudgmentService = Substitute.For<IMergeJudgmentService>();
            _mockGameOverJudgmentService = Substitute.For<IGameOverJudgmentService>();
            _mockGameRuleSettingsRepository = Substitute.For<IGameRuleSettingsRepository>();

            // Setup mock to return a default value
            _mockGameRuleSettingsRepository.GetContactTimeLimit().Returns(1.0f);

            _mockMergeJudgmentService.CreateMergeData(
                Arg.Any<Vector2>(),
                Arg.Any<Vector2>(),
                Arg.Any<int>()
            ).Returns(callInfo =>
            {
                Vector2 source = callInfo.ArgAt<Vector2>(0);
                Vector2 target = callInfo.ArgAt<Vector2>(1);
                int itemNo = callInfo.ArgAt<int>(2);

                Vector2 mergePosition = (source + target) / 2;
                return new MergeData(mergePosition, itemNo);
            });

            // Specify Maxitemno to initialize the test target
            _mergeItemUseCase = new MergeItemUseCase(
                10,
                _mockMergeItemIndexService,
                _mockMergeJudgmentService,
                _mockGameOverJudgmentService,
                _mockGameRuleSettingsRepository);
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
        public void AddContactTime_ShouldIncreaseContactTimeCorrectly()
        {
            // Arrange
            MergeItemEntity entity = new MergeItemEntity(1);
            float initialTime = entity.ContactTime;
            float deltaTime = 2.5f;

            // Act
            entity.AddContactTime(deltaTime);

            // Assert
            Assert.AreEqual(initialTime + deltaTime, entity.ContactTime, "ContactTime should be increased by deltaTime.");
        }

        [Test]
        public void ResetContactTime_ShouldSetContactTimeToZero()
        {
            // Arrange
            MergeItemEntity entity = new MergeItemEntity(1);
            entity.AddContactTime(1.5f);

            // Act
            entity.ResetContactTime();

            // Assert
            Assert.AreEqual(0f, entity.ContactTime, "ContactTime should be reset to zero.");
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
        public void CanMerge_WhenMergeJudgmentServiceReturnsTrue_ShouldReturnTrue()
        {
            // Arrange
            int currentItemNo = 1;
            int targetItemNo = 1;
            _mockMergeJudgmentService.CanMerge(currentItemNo, targetItemNo).Returns(true);

            // Act
            bool result = _mergeItemUseCase.CanMerge(currentItemNo, targetItemNo);

            // Assert
            Assert.IsTrue(result, "CanMerge should return true when MergeJudgmentService returns true.");
        }

        [Test]
        public void CanMerge_WhenMergeJudgmentServiceReturnsFalse_ShouldReturnFalse()
        {
            // Arrange
            int currentItemNo = 1;
            int targetItemNo = 2;
            _mockMergeJudgmentService.CanMerge(currentItemNo, targetItemNo).Returns(false);

            // Act
            bool result = _mergeItemUseCase.CanMerge(currentItemNo, targetItemNo);

            // Assert
            Assert.IsFalse(result, "CanMerge should return false when MergeJudgmentService returns false.");
        }

        [Test]
        public void CreateMergeData_WhenCalled_ShouldReturnExpectedMergeData()
        {
            // Arrange
            Vector2 sourcePosition = new(0, 0);
            Vector2 targetPosition = new(2, 2);
            int itemNo = 1;
            var expectedMergeData = new MergeData(new Vector2(1, 1), itemNo);

            // Act
            var result = _mergeItemUseCase.CreateMergeData(sourcePosition, targetPosition, itemNo);

            // Assert
            Assert.AreEqual(expectedMergeData.Position, result.Position, "Merge position should be the midpoint of source and target positions.");
            Assert.AreEqual(expectedMergeData.ItemNo, result.ItemNo, "MergeData should contain the correct item number.");
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
