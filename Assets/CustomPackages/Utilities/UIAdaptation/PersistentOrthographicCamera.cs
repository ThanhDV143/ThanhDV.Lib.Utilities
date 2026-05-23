using UnityEngine;

namespace ThanhDV.Utilities
{
    public class PersistentOrthographicCamera : MonoBehaviour
    {
        [Space]
        [SerializeField] private bool activateOnAwake = true;
        [SerializeField] private bool autoReadapt = false;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Vector2 referenceResolution = new(1080, 1920);

        private float _baseOrthographicSize = -1f;

        private int _lastScreenWidth;
        private int _lastScreenHeight;

        private void Awake()
        {
            if (activateOnAwake)
            {
                ResizeCamera(out _);
            }
        }

        private void Update()
        {
            if (!autoReadapt) return;
            if (_lastScreenWidth == Screen.width && _lastScreenHeight == Screen.height) return;
            ResizeCamera(out _);
        }

        public void ResizeCamera(out float _orthographicSize)
        {
            if (mainCamera == null)
            {
                Debug.Log("<color=red>Main camera is null!!! Trying to find the main camera...</color>");
                mainCamera = Camera.main;
            }

            if (mainCamera == null)
            {
                Debug.Log("<color=red>Main camera is null!!! Can not find mainCamera!!!</color>");
                _orthographicSize = -1f;
                return;
            }

            // Lazy-cache the baseline so repeated calls compute from the same anchor.
            if (_baseOrthographicSize < 0f)
            {
                _baseOrthographicSize = mainCamera.orthographicSize;
            }

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            float screenRatio = screenWidth / screenHeight;
            float referenceRatio = referenceResolution.x / referenceResolution.y;

            if (screenRatio < referenceRatio)
            {
                float baseHorizontalSize = _baseOrthographicSize * referenceResolution.x / referenceResolution.y;
                _orthographicSize = baseHorizontalSize * screenHeight / screenWidth;
            }
            else
            {
                _orthographicSize = _baseOrthographicSize;
            }

            mainCamera.orthographicSize = _orthographicSize;
            _lastScreenWidth = (int)screenWidth;
            _lastScreenHeight = (int)screenHeight;
        }
    }
}
