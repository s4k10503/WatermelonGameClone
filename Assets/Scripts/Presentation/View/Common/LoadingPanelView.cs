using UnityEngine;

namespace WatermelonGameClone.Presentation
{
    public class LoadingPanelView : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        private void Start()
            => HidePanel();

        private void OnDestroy()
        {
            _canvas = null;
        }

        public void ShowPanel()
            => _canvas.enabled = true;
        private void HidePanel()
            => _canvas.enabled = false;
    }
}
