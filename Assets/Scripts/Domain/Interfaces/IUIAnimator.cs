using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;

namespace WatermelonGameClone
{
    public interface IUIAnimator
    {
        void GetUIPosition(Transform targetTransform);
        void HarmonicMotion(Transform targetTransform, HarmonicMotionType animationType);
        void AnimateScale(GameObject target, Vector3 fromScale, Vector3 toScale, float duration, Ease easeType, bool isUpdate = true);
    }
}
