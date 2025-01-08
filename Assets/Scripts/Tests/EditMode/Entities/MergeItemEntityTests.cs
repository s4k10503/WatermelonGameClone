using NUnit.Framework;
using System;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Tests
{
    public class MergeItemEntityTests
    {
        private MergeItemEntity _mergeItemEntity;

        [SetUp]
        public void SetUp()
        {
            int itemNo = 1;
            float contactTimeLimit = 1.0f;
            _mergeItemEntity = new MergeItemEntity(itemNo, contactTimeLimit);
        }

        [Test]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Assert
            Assert.IsNotNull(_mergeItemEntity.Id, "Id should not be null.");
            Assert.AreEqual(1, _mergeItemEntity.ItemNo, "ItemNo should be initialized correctly.");
            Assert.AreEqual(0f, _mergeItemEntity.ContactTime, "ContactTime should be initialized to 0.");
        }

        [Test]
        public void AddContactTime_ShouldIncreaseContactTime()
        {
            // Arrange
            float deltaTime = 2.5f;

            // Act
            _mergeItemEntity.AddContactTime(deltaTime);

            // Assert
            Assert.AreEqual(2.5f, _mergeItemEntity.ContactTime, "ContactTime should be increased by deltaTime.");
        }

        [Test]
        public void ResetContactTime_ShouldSetContactTimeToZero()
        {
            // Arrange
            _mergeItemEntity.AddContactTime(1.5f);

            // Act
            _mergeItemEntity.ResetContactTime();

            // Assert
            Assert.AreEqual(0f, _mergeItemEntity.ContactTime, "ContactTime should be reset to 0.");
        }

        [Test]
        public void CanMerge_WhenItemNumbersAreEqual_ShouldReturnTrue()
        {
            // Arrange
            var otherEntity = new MergeItemEntity(1, 1.0f);

            // Act
            bool result = _mergeItemEntity.CanMergeWith(otherEntity);

            // Assert
            Assert.IsTrue(result, "CanMerge should return true when item numbers are equal.");
        }

        [Test]
        public void CanMerge_WhenItemNumbersAreNotEqual_ShouldReturnFalse()
        {
            // Arrange
            var otherEntity = new MergeItemEntity(2, 1.0f);

            // Act
            bool result = _mergeItemEntity.CanMergeWith(otherEntity);

            // Assert
            Assert.IsFalse(result, "CanMerge should return false when item numbers are not equal.");
        }

        [Test]
        public void CheckGameOver_WhenContactTimeExceedsTimeLimit_ShouldReturnTrue()
        {
            // Arrange
            _mergeItemEntity.AddContactTime(1.5f);

            // Act
            bool result = _mergeItemEntity.CheckGameOver();

            // Assert
            Assert.IsTrue(result, "GameOver should be true when contact time exceeds time limit.");
        }

        [Test]
        public void CheckGameOver_WhenContactTimeIsEqualToTimeLimit_ShouldReturnFalse()
        {
            // Arrange
            _mergeItemEntity.AddContactTime(1.0f);

            // Act
            bool result = _mergeItemEntity.CheckGameOver();

            // Assert
            Assert.IsFalse(result, "GameOver should be false when contact time is equal to time limit.");
        }

        [Test]
        public void CheckGameOver_WhenContactTimeIsLessThanTimeLimit_ShouldReturnFalse()
        {
            // Arrange
            _mergeItemEntity.AddContactTime(0.5f);

            // Act
            bool result = _mergeItemEntity.CheckGameOver();

            // Assert
            Assert.IsFalse(result, "GameOver should be false when contact time is less than time limit.");
        }

        [TearDown]
        public void TearDown()
        {
            _mergeItemEntity = null;
        }
    }
}
