using System;

public interface IMergeItemEntity
{
    Guid Id { get; }
    int ItemNo { get; }
    float ContactTime { get; }
    void AddContactTime(float deltaTime);
    void ResetContactTime();
}
