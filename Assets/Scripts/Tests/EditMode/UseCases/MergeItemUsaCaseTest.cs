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
        private IMergeService _mockMergeService;
        private IGameRuleSettingsRepository _mockGameRuleSettingsRepository;

        [SetUp]
        public void SetUp()
        {
            // Mock creation
            _mockMergeService = Substitute.For<IMergeService>();
            _mockGameRuleSettingsRepository = Substitute.For<IGameRuleSettingsRepository>();

            // Setup mock to return a default value
            _mockGameRuleSettingsRepository.GetContactTimeLimit().Returns(1.0f);

            _mockMergeService.CreateMergeData(
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
                _mockMergeService,
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
            var entity = _mergeItemUseCase.CreateMergeItemEntity(1);
            float initialTime = entity.ContactTime;
            float deltaTime = 2.5f;

            // Act
            _mergeItemUseCase.AddContactTime(entity.Id, deltaTime);

            // Assert
            var resultEntity = _mergeItemUseCase.GetEntityById(entity.Id);
            Assert.AreEqual(initialTime + deltaTime, resultEntity.ContactTime, "ContactTime should be increased by deltaTime.");
        }

        [Test]
        public void ResetContactTime_ShouldSetContactTimeToZero()
        {
            // Arrange
            var entity = new MergeItemEntity(1, 1.0f);
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
            _mockMergeService.GenerateNextItemIndex(Arg.Any<int>()).Returns(5);

            // Act
            _mergeItemUseCase.UpdateNextItemIndex();

            // Assert
            Assert.AreEqual(5, _mergeItemUseCase.NextItemIndex.Value);

            // Confirm that GenerateNextItiteMindex was called by Maxitemno
            _mockMergeService.Received(1).GenerateNextItemIndex(10);
        }

        [Test]
        public void CanMerge_WhenMergeJudgmentServiceReturnsTrue_ShouldReturnTrue()
        {
            // Arrange
            var entity1 = _mergeItemUseCase.CreateMergeItemEntity(1);
            var entity2 = _mergeItemUseCase.CreateMergeItemEntity(1);

            // Act
            bool result = _mergeItemUseCase.CanMerge(entity1.Id, entity2.Id);

            // Assert
            Assert.IsTrue(result, "CanMerge should return true when MergeJudgmentService returns true.");
        }

        [Test]
        public void CanMerge_WhenMergeJudgmentServiceReturnsFalse_ShouldReturnFalse()
        {
            // Arrange
            var entity1 = _mergeItemUseCase.CreateMergeItemEntity(1);
            var entity2 = _mergeItemUseCase.CreateMergeItemEntity(2);

            // Act
            bool result = _mergeItemUseCase.CanMerge(entity1.Id, entity2.Id);

            // Assert
            Assert.IsFalse(result, "CanMerge should return false when MergeJudgmentService returns false.");
        }

        [Test]
        public void CreateMergeData_WhenCalled_ShouldReturnExpectedMergeData()
        {
            // Arrange
            var entity1 = _mergeItemUseCase.CreateMergeItemEntity(1);
            entity1.Position = new Vector3(0, 0, 0);

            var entity2 = _mergeItemUseCase.CreateMergeItemEntity(1);
            entity2.Position = new Vector3(2, 2, 0);

            int expectedItemNo = 1;
            var expectedPosition = new Vector2(1, 1); // Center position

            // Act
            var result = _mergeItemUseCase.CreateMergeData(entity1.Id, entity2.Id);

            // Assert
            Assert.AreEqual(expectedPosition, result.Position, "Merge position should be the midpoint of source and target positions.");
            Assert.AreEqual(expectedItemNo, result.ItemNo, "MergeData should contain the correct item number.");
        }


        [Test]
        public void CheckGameOver_WhenContactTimeExceedsTimeLimit_ShouldReturnTrue()
        {
            // Arrange
            var entity = _mergeItemUseCase.CreateMergeItemEntity(1);
            _mergeItemUseCase.AddContactTime(entity.Id, 1.5f);

            // Act
            bool result = _mergeItemUseCase.CheckGameOver(entity.Id);

            // Assert
            Assert.IsTrue(result, "GameOver should be true when contact time exceeds time limit.");
        }

        [Test]
        public void CheckGameOver_WhenContactTimeIsEqualToTimeLimit_ShouldReturnFalse()
        {
            // Arrange
            var entity = _mergeItemUseCase.CreateMergeItemEntity(1);
            _mergeItemUseCase.AddContactTime(entity.Id, 1.0f);

            // Act
            bool result = _mergeItemUseCase.CheckGameOver(entity.Id);

            // Assert
            Assert.IsFalse(result, "GameOver should be false when contact time is equal to time limit.");
        }

        [Test]
        public void CheckGameOver_WhenContactTimeIsLessThanTimeLimit_ShouldReturnFalse()
        {
            // Arrange
            var entity = _mergeItemUseCase.CreateMergeItemEntity(1);
            _mergeItemUseCase.AddContactTime(entity.Id, 0.5f);

            // Act
            bool result = _mergeItemUseCase.CheckGameOver(entity.Id);

            // Assert
            Assert.IsFalse(result, "GameOver should be false when contact time is less than time limit.");
        }

        [TearDown]
        public void TearDown()
        {
            // Release resources after the test
            _mergeItemUseCase.Dispose();
            _mergeItemUseCase = null;
            _mockMergeService = null;
            _mockGameRuleSettingsRepository = null;
        }
    }
}
