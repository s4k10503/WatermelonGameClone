using UnityEngine;

namespace WatermelonGameClone.Presentation
{
    public sealed class StageView : MonoBehaviour
    {
        [SerializeField] private GameObject[] childObjects;

        private void OnDestroy()
        {
            childObjects = null;
        }

        public void HideStage()
            => SetChildrenActive(false);

        public void ShowStage()
            => SetChildrenActive(true);

        private void SetChildrenActive(bool isActive)
        {
            foreach (GameObject child in childObjects)
            {
                if (child != null)
                {
                    child.SetActive(isActive);
                }
            }
        }
    }
}
