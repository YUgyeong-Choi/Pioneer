using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VRUIP
{
    /// <summary>
    /// This is a script that is attached to the buttons that are used to scale a canvas.
    /// </summary>
    public class ScaleUIButton : A_ColorController, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Components")]
        [SerializeField] private Image icon;
        [SerializeField] private RectTransform expander;
        
        private RectTransform _buttonTransform;
        private RectTransform _scalableCanvas;
        private CanvasCorner _corner;
        private Vector3 _originalPosition;
        private Vector2 _originalSize;
        private LineRenderer _lineRenderer;
        private bool _isVR;
        private Vector2 _minSize;
        private Vector2 _maxSize;
        private Vector2 _expanderSize = new(1000, 1000);

        private readonly Dictionary<CanvasCorner, Vector2[]> anchors = new()
        {
            { CanvasCorner.BottomLeft, new[] { Vector2.zero, Vector2.zero } },
            { CanvasCorner.TopLeft, new[] { new Vector2(0,1), new Vector2(0,1) } },
            { CanvasCorner.TopRight, new[] { new Vector2(1,1), new Vector2(1,1) } },
            { CanvasCorner.BottomRight, new[] { new Vector2(1,0), new Vector2(1,0) } }
        };
        
        private readonly Dictionary<CanvasCorner, Vector2> pivots = new()
        {
            { CanvasCorner.BottomLeft, new Vector2(1, 1) },
            { CanvasCorner.TopLeft, new Vector2(1, 0) },
            { CanvasCorner.TopRight, new Vector2(0, 0) },
            { CanvasCorner.BottomRight, new Vector2(0, 1) }
        };

        public void Initialize(RectTransform scalableCanvas, CanvasCorner corner, int cornerIndex, Vector2 minSize, Vector2 maxSize)
        {
            _isVR = VRUIPManager.instance.IsVR;
            _scalableCanvas = scalableCanvas;
            _corner = corner;
            _buttonTransform = GetComponent<RectTransform>();
            var rotate = _corner == CanvasCorner.TopLeft || _corner == CanvasCorner.BottomRight;
            transform.localEulerAngles = rotate ? new Vector3(0, 0, 90) : Vector3.zero;
            _lineRenderer = VRUIPManager.instance.lineRenderer;
            
            // Set min and max size.
            _minSize = minSize;
            _maxSize = maxSize;

            // Set anchors
            _buttonTransform.anchorMin = anchors[_corner][0];
            _buttonTransform.anchorMax = anchors[_corner][1];
            
            // Set positions to corners
            var corners = new Vector3[4];
            scalableCanvas.GetLocalCorners(corners);
            transform.localPosition = corners[cornerIndex];
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _originalPosition = transform.position;
            _originalSize = _scalableCanvas.sizeDelta;
            // Set pivots
            Util.SetPivot(_scalableCanvas, pivots[_corner]);
            expander.sizeDelta = _expanderSize;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            expander.sizeDelta = GetComponent<RectTransform>().sizeDelta;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 pointerEndPosition;
            if (_isVR)
            {
                pointerEndPosition = _lineRenderer.GetPosition(_lineRenderer.positionCount - 1);
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_scalableCanvas, eventData.position, VRUIPManager.instance.mainCamera, out var mousePos);
                pointerEndPosition = _scalableCanvas.TransformPoint(mousePos);
            }
            
            float differenceX = 0;
            float differenceY = 0;
            if (_corner == CanvasCorner.TopLeft)
            {
                differenceX = _originalPosition.x - pointerEndPosition.x;
                differenceY = pointerEndPosition.y - _originalPosition.y;
            }
            else if (_corner == CanvasCorner.BottomRight)
            {
                differenceX = pointerEndPosition.x - _originalPosition.x;
                differenceY = _originalPosition.y - pointerEndPosition.y;
            }
            else if (_corner == CanvasCorner.BottomLeft)
            {
                differenceX = _originalPosition.x - pointerEndPosition.x;
                differenceY = _originalPosition.y - pointerEndPosition.y;
            }
            else if (_corner == CanvasCorner.TopRight)
            {
                differenceX = pointerEndPosition.x - _originalPosition.x;
                differenceY = pointerEndPosition.y - _originalPosition.y;
            }
            
            var difference = new Vector2(differenceX, differenceY);
            var lossyScale = _scalableCanvas.lossyScale;
            var pixelDifference = new Vector3(difference.x / lossyScale.x, difference.y / lossyScale.y);

            var newSize = _originalSize + (Vector2)pixelDifference;
            //var adjustedSize = new Vector2(Mathf.Max(newSize.x, 50), Mathf.Max(newSize.y, 50));
            var adjustedSize = new Vector2(Mathf.Clamp(newSize.x, _minSize.x, _maxSize.x), Mathf.Clamp(newSize.y, _minSize.y, _maxSize.y));
            _scalableCanvas.sizeDelta = adjustedSize;
        }

        public enum CanvasCorner
        {
            BottomLeft,
            TopLeft,
            TopRight,
            BottomRight
        }

        protected override void SetColors(ColorTheme theme)
        {
            icon.color = theme.secondaryColor;
        }
    }
}
