using System;
using System.Numerics;

namespace Domain.Interfaces
{
    public interface IMergeItemEntity
    {
        Guid Id { get; }
        int ItemNo { get; }
        float ContactTime { get; }
        Vector2 Position { get; set; }

        void AddContactTime(float deltaTime);
        void ResetContactTime();
        bool CanMergeWith(IMergeItemEntity other);
        bool CheckGameOver();
    }
}
