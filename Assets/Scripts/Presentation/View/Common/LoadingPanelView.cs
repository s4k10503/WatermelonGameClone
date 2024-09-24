using UnityEngine;

namespace WatermelonGameClone.Presentation
{
    public class LoadingPanelView : MonoBehaviour
    {
        [SerializeField] Canvas _canvas;

        private void Start()
            => HidePanel();
        public void ShowPanel()
            => _canvas.enabled = true;
        private void HidePanel()
            => _canvas.enabled = false;
    }
}