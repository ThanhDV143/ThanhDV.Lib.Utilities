using UnityEngine;
using UnityEngine.UI;

namespace ThanhDV.Utilities
{
    [RequireComponent(typeof(CanvasScaler))]
    public class UIScaler : MonoBehaviour
    {
        [Space]
        [SerializeField] private CanvasScaler scaler;

        [Space]
        [SerializeField] private Vector2 referenceResolution = new Vector2(1080f, 1920f);

        private bool _isUpdating;

        private void Awake()
        {
            if (TryGetCanvasScaler())
            {
                Prepare();
                Setup();
            }
        }

        /// <summary>
        /// Unity callback fired whenever this RectTransform's dimensions change
        /// (device rotation, window resize, parent layout change). Used to re-adapt
        /// the canvas at runtime without polling per frame.
        /// </summary>
        private void OnRectTransformDimensionsChange()
        {
            if (_isUpdating) return;
            if (!isActiveAndEnabled) return;
            if (!TryGetCanvasScaler()) return;

            _isUpdating = true;
            Setup();
            _isUpdating = false;
        }

        private void Setup()
        {
            if (scaler == null)
            {
                Debug.Log("<color=red>[UIAdaptation] CanvasScaler not found!!!</color>");
                return;
            }

            float referenceRatio = referenceResolution.x / referenceResolution.y;
            float screenRatio = (float)Screen.width / Screen.height;

            scaler.matchWidthOrHeight = (screenRatio > referenceRatio) ? 1f : 0f;
        }

        private void Prepare()
        {
            if (scaler == null)
            {
                Debug.Log("<color=red>[UIAdaptation] CanvasScaler not found!!!</color>");
                return;
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.referenceResolution = referenceResolution;
        }

        private bool TryGetCanvasScaler()
        {
            if (scaler != null) return true;

            TryGetComponent(out scaler);

            return scaler != null;
        }
    }
}
