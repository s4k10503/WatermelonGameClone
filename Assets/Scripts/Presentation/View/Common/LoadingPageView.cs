using UnityEngine;

namespace WatermelonGameClone.Presentation
{
    public class LoadingPageView : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

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
