using System;
using System.Numerics;

public class MergeItemEntity : IMergeItemEntity
{
    public Guid Id { get; private set; }
    public int ItemNo { get; private set; }
    public float ContactTime { get; private set; }
    public Vector2 Position { get; set; }
    private readonly float _contactTimeLimit;

    public MergeItemEntity(
        int itemNo,
        float contactTimeLimit)
    {
        Id = Guid.NewGuid();
        ItemNo = itemNo;
        ContactTime = 0f;
        Position = Vector2.Zero;
        _contactTimeLimit = contactTimeLimit;
    }

    public void AddContactTime(float deltaTime)
    {
        ContactTime += deltaTime;
    }

    public void ResetContactTime()
    {
        ContactTime = 0f;
    }

    public bool CanMergeWith(IMergeItemEntity other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        return ItemNo == other.ItemNo;
    }

    public bool CheckGameOver()
    {
        return ContactTime > _contactTimeLimit;
    }
}
