using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;

namespace WatermelonGameClone
{
    public class UIAnimator : IUIAnimator
    {
        private float _moveSpeed = 5f;
        private float _moveHeight = 5f;
        private Vector3 _originalPos;


        public void GetUIPosition(Transform targetTransform)
        {
            _originalPos = targetTransform.localPosition;
        }

        public void HarmonicMotion(Transform targetTransform, HarmonicMotionType animationType)
        {
            float offset = 0f;
            switch (animationType)
            {
                case HarmonicMotionType.Sin:
                    offset = Mathf.Sin(Time.time * _moveSpeed) * _moveHeight;
                    break;
                case HarmonicMotionType.Cos:
                    offset = Mathf.Cos(Time.time * _moveSpeed) * _moveHeight;
                    break;
            }

            targetTransform.localPosition = _originalPos + new Vector3(0, offset, 0);
        }

        public void AnimateScale(GameObject target, Vector3 fromScale, Vector3 toScale, float duration, Ease easeType, bool isUpdate = true)
        {
            target.transform.localScale = fromScale;
            target.transform.DOScale(toScale, duration).SetEase(easeType).SetUpdate(isUpdate);
        }
    }

    public enum HarmonicMotionType
    {
        Sin,
        Cos
    }
}

