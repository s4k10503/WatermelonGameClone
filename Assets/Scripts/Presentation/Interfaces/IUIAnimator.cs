using UnityEngine;
using DG.Tweening;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public interface IUIAnimator
    {
        void GetUIPosition(Transform targetTransform);
        void HarmonicMotion(Transform targetTransform, HarmonicMotionType animationType);
        void AnimateScale(GameObject target, Vector3 fromScale, Vector3 toScale, float duration, Ease easeType, bool isUpdate = true);
        void AnimateLocalPosition(Transform targetTransform, Vector3 toPosition, float duration, Ease easeType, bool isUpdate = true, System.Action onComplete = null);
    }
}
