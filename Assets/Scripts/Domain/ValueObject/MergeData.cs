using System.Numerics;

namespace Domain.ValueObject
{
    public sealed class MergeData
    {
        public Vector2 Position;
        public readonly int ItemNo;

        public MergeData(Vector2 position, int itemNo)
        {
            Position = position;
            ItemNo = itemNo;
        }
    }
}
