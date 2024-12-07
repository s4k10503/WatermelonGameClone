using UnityEngine;

namespace WatermelonGameClone.Presentation
{
    public sealed class BackgroundPanelView : MonoBehaviour, IBackgroundPanelView
    {
        [SerializeField] Canvas _canvas;

        private void Start()
            => HidePanel();

        private void OnDestroy()
        {
            _canvas = null;
        }

        public void ShowPanel()
            => _canvas.enabled = true;

        public void HidePanel()
            => _canvas.enabled = false;
    }
}
