using System;
using UnityEngine;

public interface IMergeItemEntity
{
    Guid Id { get; }
    int ItemNo { get; }
    float ContactTime { get; }
    Vector3 Position { get; set; }

    void AddContactTime(float deltaTime);
    void ResetContactTime();
    bool CanMergeWith(IMergeItemEntity other);
    bool CheckGameOver();
}
