using UnityEngine;

namespace ThanhDV.Utilities
{
    public enum AnchorPresets
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottomCenter,
        BottomRight,

        VertStretchLeft,
        VertStretchRight,
        VertStretchCenter,

        HorStretchTop,
        HorStretchMiddle,
        HorStretchBottom,

        StretchAll
    }

    public enum PivotPresets
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    public static class RectTransformExt
    {
        /// <summary>
        /// Sets the anchor of a <see cref="RectTransform"/> using a preset.
        /// </summary>
        /// <param name="source">The RectTransform to modify.</param>
        /// <param name="align">The anchor preset to apply.</param>
        /// <param name="offsetX">
        /// Optional new <c>anchoredPosition.x</c>. When <c>null</c> (default), the current
        /// anchoredPosition.x is preserved.
        /// </param>
        /// <param name="offsetY">
        /// Optional new <c>anchoredPosition.y</c>. When <c>null</c> (default), the current
        /// anchoredPosition.y is preserved.
        /// </param>
        public static void SetAnchor(this RectTransform source, AnchorPresets align, int? offsetX = null, int? offsetY = null)
        {
            if (offsetX.HasValue || offsetY.HasValue)
            {
                Vector2 current = source.anchoredPosition;
                source.anchoredPosition = new Vector2(
                    offsetX ?? current.x,
                    offsetY ?? current.y);
            }

            switch (align)
            {
                case AnchorPresets.TopLeft:
                    {
                        source.anchorMin = new Vector2(0, 1);
                        source.anchorMax = new Vector2(0, 1);
                        break;
                    }
                case AnchorPresets.TopCenter:
                    {
                        source.anchorMin = new Vector2(0.5f, 1);
                        source.anchorMax = new Vector2(0.5f, 1);
                        break;
                    }
                case AnchorPresets.TopRight:
                    {
                        source.anchorMin = new Vector2(1, 1);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }

                case AnchorPresets.MiddleLeft:
                    {
                        source.anchorMin = new Vector2(0, 0.5f);
                        source.anchorMax = new Vector2(0, 0.5f);
                        break;
                    }
                case AnchorPresets.MiddleCenter:
                    {
                        source.anchorMin = new Vector2(0.5f, 0.5f);
                        source.anchorMax = new Vector2(0.5f, 0.5f);
                        break;
                    }
                case AnchorPresets.MiddleRight:
                    {
                        source.anchorMin = new Vector2(1, 0.5f);
                        source.anchorMax = new Vector2(1, 0.5f);
                        break;
                    }

                case AnchorPresets.BottomLeft:
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(0, 0);
                        break;
                    }
                case AnchorPresets.BottomCenter:
                    {
                        source.anchorMin = new Vector2(0.5f, 0);
                        source.anchorMax = new Vector2(0.5f, 0);
                        break;
                    }
                case AnchorPresets.BottomRight:
                    {
                        source.anchorMin = new Vector2(1, 0);
                        source.anchorMax = new Vector2(1, 0);
                        break;
                    }

                case AnchorPresets.HorStretchTop:
                    {
                        source.anchorMin = new Vector2(0, 1);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }
                case AnchorPresets.HorStretchMiddle:
                    {
                        source.anchorMin = new Vector2(0, 0.5f);
                        source.anchorMax = new Vector2(1, 0.5f);
                        break;
                    }
                case AnchorPresets.HorStretchBottom:
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(1, 0);
                        break;
                    }

                case AnchorPresets.VertStretchLeft:
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(0, 1);
                        break;
                    }
                case AnchorPresets.VertStretchCenter:
                    {
                        source.anchorMin = new Vector2(0.5f, 0);
                        source.anchorMax = new Vector2(0.5f, 1);
                        break;
                    }
                case AnchorPresets.VertStretchRight:
                    {
                        source.anchorMin = new Vector2(1, 0);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }

                case AnchorPresets.StretchAll:
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }
            }
        }

        /// <summary>
        /// Sets the pivot of a <see cref="RectTransform"/> using a preset.
        /// </summary>
        public static void SetPivot(this RectTransform source, PivotPresets preset)
        {

            switch (preset)
            {
                case PivotPresets.TopLeft:
                    {
                        source.pivot = new Vector2(0, 1);
                        break;
                    }
                case PivotPresets.TopCenter:
                    {
                        source.pivot = new Vector2(0.5f, 1);
                        break;
                    }
                case PivotPresets.TopRight:
                    {
                        source.pivot = new Vector2(1, 1);
                        break;
                    }

                case PivotPresets.MiddleLeft:
                    {
                        source.pivot = new Vector2(0, 0.5f);
                        break;
                    }
                case PivotPresets.MiddleCenter:
                    {
                        source.pivot = new Vector2(0.5f, 0.5f);
                        break;
                    }
                case PivotPresets.MiddleRight:
                    {
                        source.pivot = new Vector2(1, 0.5f);
                        break;
                    }

                case PivotPresets.BottomLeft:
                    {
                        source.pivot = new Vector2(0, 0);
                        break;
                    }
                case PivotPresets.BottomCenter:
                    {
                        source.pivot = new Vector2(0.5f, 0);
                        break;
                    }
                case PivotPresets.BottomRight:
                    {
                        source.pivot = new Vector2(1, 0);
                        break;
                    }
            }
        }

        public static void SetOffset(this RectTransform rt, float left, float right, float top, float bottom)
        {
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
        }

        public static void SetOffsetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetOffsetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetOffsetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetOffsetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }
    }
}
