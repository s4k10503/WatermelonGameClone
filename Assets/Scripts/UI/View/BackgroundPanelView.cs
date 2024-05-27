using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace WatermelonGameClone
{
    public class BackgroundPanelView : MonoBehaviour, IBackgroundPanelView
    {
        // Objects
        private Transform _canvasTransform;
        private RectTransform _canvasRectTransform;
        private RectTransform _panelRectTransform;

        [Inject]
        public void Construct([Inject(Id = "CanvasTransform")] Transform canvasTransform)
        {
            _canvasTransform = canvasTransform;
            _canvasRectTransform = _canvasTransform.GetComponent<RectTransform>();
            _panelRectTransform = GetComponent<RectTransform>();
            _panelRectTransform.sizeDelta = _canvasRectTransform.sizeDelta;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
        }

        public void ShowPanel()
        {
            gameObject.SetActive(true);
        }

        public void HidePanel()
        {
            gameObject.SetActive(false);
        }
    }
}
