using UnityEngine;
using Presentation.DTO;
using DG.Tweening;

namespace Presentation.Interfaces
{
    public interface IUIAnimator
    {
        void GetUIPosition(Transform transform);
        void HarmonicMotion(Transform transform, HarmonicMotionTypeDto motionType);
        void AnimateScale(GameObject gameObject, Vector3 from, Vector3 to, float duration, Ease ease, bool isUpdate = true);
        void AnimateLocalPosition(Transform targetTransform, Vector3 toPosition, float duration, Ease easeType, bool isUpdate = true, System.Action onComplete = null);
    }
}
