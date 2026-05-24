using UnityEngine;

namespace ThanhDV.Utilities
{
    public class UISafeZone : MonoBehaviour
    {
        private bool _isUpdating;

        private void Awake()
        {
            Setup();
        }

        /// <summary>
        /// Unity callback fired whenever this RectTransform's dimensions change
        /// (device rotation, window resize, parent layout change). Used to re-adapt
        /// to the device safe area at runtime without polling per frame.
        /// </summary>
        private void OnRectTransformDimensionsChange()
        {
            if (_isUpdating) return;
            if (!isActiveAndEnabled) return;

            _isUpdating = true;
            Setup();
            _isUpdating = false;
        }

        public void Setup()
        {
            if (!TryGetComponent(out RectTransform rectTransform))
            {
                Debug.Log("<color=red>[UIAdaptation] RectTransform not found!!!</color>");
                return;
            }

            Rect safeZone = Screen.safeArea;
            Vector2 anchorMin = safeZone.position;
            Vector2 anchorMax = anchorMin + safeZone.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
    }
}
