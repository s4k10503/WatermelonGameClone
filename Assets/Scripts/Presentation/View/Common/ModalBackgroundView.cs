using UnityEngine;

namespace WatermelonGameClone.Presentation
{
    public sealed class ModalBackgroundView : MonoBehaviour
    {
        [SerializeField] Canvas _canvas;

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
