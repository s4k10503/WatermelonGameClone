using System;
using System.Numerics;

namespace WatermelonGameClone.UseCase
{
    public sealed class MergeItemDTO
    {
        public Guid Id { get; set; }
        public int ItemNo { get; set; }
        public Vector2 Position { get; set; }
        public float ContactTime { get; set; }
    }

    public sealed class MergeResultDTO
    {
        public Vector2 Position { get; set; }
        public int ItemNo { get; set; }
    }
}
