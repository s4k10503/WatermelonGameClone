using System;

public class MergeItemEntity : IMergeItemEntity
{
    public Guid Id { get; private set; }
    public int ItemNo { get; private set; }
    public float ContactTime { get; private set; }

    public MergeItemEntity(int itemNo)
    {
        Id = Guid.NewGuid();
        ItemNo = itemNo;
        ContactTime = 0f;
    }

    public void AddContactTime(float deltaTime)
    {
        ContactTime += deltaTime;
    }

    public void ResetContactTime()
    {
        ContactTime = 0f;
    }
}
