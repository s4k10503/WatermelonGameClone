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
            _mergeItemEntity = new MergeItemEntity(1);
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

        [TearDown]
        public void TearDown()
        {
            _mergeItemEntity = null;
        }
    }
}
