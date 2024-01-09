using System;
using UnityEngine;
using UnityEngine.UI;
#if XR_ITK
using UnityEngine.XR.Interaction.Toolkit.UI;
#endif

namespace VRUIP
{
    /// <summary>
    /// This class is an abstract class that will setup any canvas for this package to work with the VR rig's camera.
    /// </summary>
    public abstract class A_Canvas : A_ColorController
    {
        // COMPONENTS
        [SerializeField] private Canvas canvas;
        
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private bool _isTransitioning;
        private Camera _camera;

        public bool IsSetup { get; private set; } = false;
        protected RectTransform RectTransform => _rectTransform;
        public Canvas Canvas => canvas;
        public Camera Camera => _camera;

        protected override void Start()
        {
            base.Start();
            if (!IsSetup) Setup();
        }

        /// <summary>
        /// Setup this canvas.
        /// </summary>
        public void Setup()
        {
            IsSetup = true;
            canvas.worldCamera = _camera = VRUIPManager.instance.mainCamera;
            _rectTransform = canvas.GetComponent<RectTransform>();
            SetupCanvasForFramework();
            SetupCanvasGroup();
        }

        /// <summary>
        /// Setup this canvas based on the user's selected target framework.
        /// </summary>
        private void SetupCanvasForFramework()
        {
            #if XR_ITK
            if (canvas.TryGetComponent<GraphicRaycaster>(out var graphicRaycaster))
            {
                Destroy(graphicRaycaster);
            }
            if (canvas.GetComponent<TrackedDeviceGraphicRaycaster>() != null) return;
            canvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
            #elif OCULUS_INTEGRATION
            if (canvas.TryGetComponent<GraphicRaycaster>(out var graphicRaycaster))
            {
                Destroy(graphicRaycaster);
            }
            if (canvas.GetComponent<OVRRaycaster>() != null) return;
            var ovrRaycaster = canvas.gameObject.AddComponent<OVRRaycaster>();
            ovrRaycaster.pointer = VRUIPManager.instance.laserPointer.gameObject;
            #else
            if (canvas.GetComponent<GraphicRaycaster>() != null) return;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            #endif
        }

        /// <summary>
        /// Setup the canvas group for this canvas.
        /// </summary>
        private void SetupCanvasGroup()
        {
            if (_canvasGroup != null) return;
            _canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            }
        }

        /// <summary>
        /// Fade out this canvas and do something when fading finishes.
        /// </summary>
        public void FadeOutCanvas(Action callback = null)
        {
            if (_isTransitioning) return;
            if (_canvasGroup.gameObject.activeInHierarchy == false) return;
            _isTransitioning = true;
            StartCoroutine(_canvasGroup.TransitionAlpha(0, 4, () =>
            {
                callback?.Invoke();
                _isTransitioning = false;
                gameObject.SetActive(false);
            }));
        }

        /// <summary>
        /// Fade in this canvas and do something when fading finishes.
        /// </summary>
        public void FadeInCanvas(Action callback = null)
        {
            if (_isTransitioning) return;
            _isTransitioning = true;
            gameObject.SetActive(true);
            StartCoroutine(_canvasGroup.TransitionAlpha(1, 4, () =>
            {
                callback?.Invoke();
                _isTransitioning = false;
            }));
        }

        /// <summary>
        /// Set alpha of canvas group.
        /// </summary>
        public void SetAlpha(float alpha)
        {
            if (!IsSetup) SetupCanvasGroup();
            _canvasGroup.alpha = alpha;
        }
    }
}
