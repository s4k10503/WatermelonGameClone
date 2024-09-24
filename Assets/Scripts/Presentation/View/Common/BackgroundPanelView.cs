using UnityEngine;

namespace WatermelonGameClone.Presentation
{
    public sealed class BackgroundPanelView : MonoBehaviour, IBackgroundPanelView
    {
        [SerializeField] Canvas _canvas;

        void Start()
            => HidePanel();

        public void ShowPanel()
            => _canvas.enabled = true;

        public void HidePanel()
            => _canvas.enabled = false;
    }
}
