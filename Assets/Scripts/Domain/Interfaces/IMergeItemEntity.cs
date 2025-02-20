using System;
using System.Numerics;

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
