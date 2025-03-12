using System;
using System.Numerics;

namespace UseCase.DTO
{
    public sealed class MergeItemDto
    {
        public Guid Id { get; set; }
        public int ItemNo { get; set; }
        public Vector2 Position { get; set; }
        public float ContactTime { get; set; }
    }

    public sealed class MergeResultDto
    {
        public Vector2 Position { get; set; }
        public int ItemNo { get; set; }
    }
}
