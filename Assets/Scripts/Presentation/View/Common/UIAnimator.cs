using Presentation.Interfaces;
using Presentation.DTO;

using System;
using DG.Tweening;
using UnityEngine;

namespace Presentation.View.Common
{
    public sealed class UIAnimator : IUIAnimator
    {
        private readonly float _moveSpeed = 3f;
        private readonly float _moveHeight = 5f;
        private Vector3 _originalPos;

        public void GetUIPosition(Transform targetTransform)
        {
            _originalPos = targetTransform?.localPosition ?? Vector3.zero;
        }

        public void HarmonicMotion(Transform targetTransform, HarmonicMotionTypeDto animationType)
        {
            if (targetTransform == null) return;

            float offset = 0f;
            switch (animationType)
            {
                case HarmonicMotionTypeDto.Sin:
                    offset = Mathf.Sin(Time.time * _moveSpeed) * _moveHeight;
                    break;
                case HarmonicMotionTypeDto.Cos:
                    offset = Mathf.Cos(Time.time * _moveSpeed) * _moveHeight;
                    break;
            }

            targetTransform.localPosition = _originalPos + new Vector3(0, offset, 0);
        }

        public void AnimateScale(
            GameObject target,
            Vector3 fromScale,
            Vector3 toScale,
            float duration,
            Ease easeType,
            bool isUpdate = true)
        {
            if (target.transform == null) return;

            target.transform.localScale = fromScale;
            target.transform
                .DOScale(toScale, duration)
                .SetEase(easeType)
                .SetUpdate(isUpdate)
                .SetLink(target);
        }

        public void AnimateLocalPosition(
            Transform targetTransform,
            Vector3 toPosition,
            float duration,
            Ease easeType,
            bool isUpdate = true,
            Action onComplete = null)
        {
            if (targetTransform == null) return;

            targetTransform
                .DOLocalMove(toPosition, duration)
                .SetEase(easeType)
                .SetUpdate(isUpdate)
                .SetLink(targetTransform.gameObject)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}

