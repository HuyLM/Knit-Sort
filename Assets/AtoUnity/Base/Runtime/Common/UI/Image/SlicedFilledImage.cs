using UnityEngine;
using UnityEngine.UI;

namespace AtoGame.Base.UI
{
    [AddComponentMenu("UI/Sliced Filled Image", 11)]
    public class SlicedFilledImage : Image
    {
        public enum FillDirection { Right = 0, Left = 1, Up = 2, Down = 3 }

        private static readonly Vector3[] s_Vertices = new Vector3[4];
        private static readonly Vector2[] s_UVs = new Vector2[4];
        private static readonly Vector2[] s_SlicedVertices = new Vector2[4];
        private static readonly Vector2[] s_SlicedUVs = new Vector2[4];

        [SerializeField]
        private FillDirection _fillDirection = FillDirection.Right;
        public FillDirection fillDirection
        {
            get { return _fillDirection; }
            set
            {
                if(_fillDirection != value)
                {
                    _fillDirection = value;
                    SetVerticesDirty();
                }
            }
        }

        [SerializeField]
        private bool _customFillCenter = true;
        public bool customFillCenter
        {
            get { return _customFillCenter; }
            set
            {
                if(_customFillCenter != value)
                {
                    _customFillCenter = value;
                    SetVerticesDirty();
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            type = Type.Simple; // Disable built-in fill system
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if(overrideSprite == null)
            {
                base.OnPopulateMesh(vh);
                return;
            }

            GenerateSlicedFilledSprite(vh);
        }

        private void GenerateSlicedFilledSprite(VertexHelper vh)
        {
            vh.Clear();

            if(fillAmount < 0.001f)
                return;

            Rect rect = GetPixelAdjustedRect();
            Vector4 outer = UnityEngine.Sprites.DataUtility.GetOuterUV(overrideSprite);
            Vector4 padding = UnityEngine.Sprites.DataUtility.GetPadding(overrideSprite);

            if(!hasBorder)
            {
                Vector2 size = overrideSprite.rect.size;
                int spriteW = Mathf.RoundToInt(size.x);
                int spriteH = Mathf.RoundToInt(size.y);

                Vector4 vertices = new Vector4(
                    rect.x + rect.width * (padding.x / spriteW),
                    rect.y + rect.height * (padding.y / spriteH),
                    rect.x + rect.width * ((spriteW - padding.z) / spriteW),
                    rect.y + rect.height * ((spriteH - padding.w) / spriteH));

                GenerateFilledSprite(vh, vertices, outer, fillAmount);
                return;
            }

            Vector4 inner = UnityEngine.Sprites.DataUtility.GetInnerUV(overrideSprite);
            Vector4 border = GetAdjustedBorders(overrideSprite.border / pixelsPerUnit, rect);
            padding = padding / pixelsPerUnit;

            s_SlicedVertices[0] = new Vector2(padding.x, padding.y);
            s_SlicedVertices[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);
            s_SlicedVertices[1].x = border.x;
            s_SlicedVertices[1].y = border.y;
            s_SlicedVertices[2].x = rect.width - border.z;
            s_SlicedVertices[2].y = rect.height - border.w;

            for(int i = 0; i < 4; ++i)
            {
                s_SlicedVertices[i].x += rect.x;
                s_SlicedVertices[i].y += rect.y;
            }

            s_SlicedUVs[0] = new Vector2(outer.x, outer.y);
            s_SlicedUVs[1] = new Vector2(inner.x, inner.y);
            s_SlicedUVs[2] = new Vector2(inner.z, inner.w);
            s_SlicedUVs[3] = new Vector2(outer.z, outer.w);

            float rectStartPos;
            float inverseTotalSize;
            if(_fillDirection == FillDirection.Left || _fillDirection == FillDirection.Right)
            {
                rectStartPos = s_SlicedVertices[0].x;
                float totalSize = s_SlicedVertices[3].x - s_SlicedVertices[0].x;
                inverseTotalSize = totalSize > 0f ? 1f / totalSize : 1f;
            }
            else
            {
                rectStartPos = s_SlicedVertices[0].y;
                float totalSize = s_SlicedVertices[3].y - s_SlicedVertices[0].y;
                inverseTotalSize = totalSize > 0f ? 1f / totalSize : 1f;
            }

            for(int x = 0; x < 3; x++)
            {
                int x2 = x + 1;

                for(int y = 0; y < 3; y++)
                {
                    if(!_customFillCenter && x == 1 && y == 1)
                        continue;

                    int y2 = y + 1;

                    float sliceStart, sliceEnd;
                    switch(_fillDirection)
                    {
                        case FillDirection.Right:
                            sliceStart = (s_SlicedVertices[x].x - rectStartPos) * inverseTotalSize;
                            sliceEnd = (s_SlicedVertices[x2].x - rectStartPos) * inverseTotalSize;
                            break;
                        case FillDirection.Up:
                            sliceStart = (s_SlicedVertices[y].y - rectStartPos) * inverseTotalSize;
                            sliceEnd = (s_SlicedVertices[y2].y - rectStartPos) * inverseTotalSize;
                            break;
                        case FillDirection.Left:
                            sliceStart = 1f - (s_SlicedVertices[x2].x - rectStartPos) * inverseTotalSize;
                            sliceEnd = 1f - (s_SlicedVertices[x].x - rectStartPos) * inverseTotalSize;
                            break;
                        case FillDirection.Down:
                            sliceStart = 1f - (s_SlicedVertices[y2].y - rectStartPos) * inverseTotalSize;
                            sliceEnd = 1f - (s_SlicedVertices[y].y - rectStartPos) * inverseTotalSize;
                            break;
                        default:
                            sliceStart = sliceEnd = 0f;
                            break;
                    }

                    if(sliceStart >= fillAmount)
                        continue;

                    Vector4 vertices = new Vector4(
                        s_SlicedVertices[x].x,
                        s_SlicedVertices[y].y,
                        s_SlicedVertices[x2].x,
                        s_SlicedVertices[y2].y);

                    Vector4 uvs = new Vector4(
                        s_SlicedUVs[x].x,
                        s_SlicedUVs[y].y,
                        s_SlicedUVs[x2].x,
                        s_SlicedUVs[y2].y);

                    float sliceFill = (fillAmount - sliceStart) / (sliceEnd - sliceStart);
                    GenerateFilledSprite(vh, vertices, uvs, sliceFill);
                }
            }
        }

        private void GenerateFilledSprite(VertexHelper vh, Vector4 vertices, Vector4 uvs, float fill)
        {
            if(fill < 0.001f)
                return;

            float uvLeft = uvs.x;
            float uvBottom = uvs.y;
            float uvRight = uvs.z;
            float uvTop = uvs.w;

            if(fill < 1f)
            {
                if(_fillDirection == FillDirection.Left || _fillDirection == FillDirection.Right)
                {
                    if(_fillDirection == FillDirection.Left)
                    {
                        vertices.x = vertices.z - (vertices.z - vertices.x) * fill;
                        uvLeft = uvRight - (uvRight - uvLeft) * fill;
                    }
                    else
                    {
                        vertices.z = vertices.x + (vertices.z - vertices.x) * fill;
                        uvRight = uvLeft + (uvRight - uvLeft) * fill;
                    }
                }
                else
                {
                    if(_fillDirection == FillDirection.Down)
                    {
                        vertices.y = vertices.w - (vertices.w - vertices.y) * fill;
                        uvBottom = uvTop - (uvTop - uvBottom) * fill;
                    }
                    else
                    {
                        vertices.w = vertices.y + (vertices.w - vertices.y) * fill;
                        uvTop = uvBottom + (uvTop - uvBottom) * fill;
                    }
                }
            }

            s_Vertices[0] = new Vector3(vertices.x, vertices.y);
            s_Vertices[1] = new Vector3(vertices.x, vertices.w);
            s_Vertices[2] = new Vector3(vertices.z, vertices.w);
            s_Vertices[3] = new Vector3(vertices.z, vertices.y);

            s_UVs[0] = new Vector2(uvLeft, uvBottom);
            s_UVs[1] = new Vector2(uvLeft, uvTop);
            s_UVs[2] = new Vector2(uvRight, uvTop);
            s_UVs[3] = new Vector2(uvRight, uvBottom);

            int startIndex = vh.currentVertCount;
            for(int i = 0; i < 4; i++)
                vh.AddVert(s_Vertices[i], color, s_UVs[i]);

            vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        private Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
        {
            Rect originalRect = rectTransform.rect;

            for(int axis = 0; axis <= 1; axis++)
            {
                if(originalRect.size[axis] != 0)
                {
                    float scale = adjustedRect.size[axis] / originalRect.size[axis];
                    border[axis] *= scale;
                    border[axis + 2] *= scale;
                }

                float combined = border[axis] + border[axis + 2];
                if(adjustedRect.size[axis] < combined && combined != 0)
                {
                    float scale = adjustedRect.size[axis] / combined;
                    border[axis] *= scale;
                    border[axis + 2] *= scale;
                }
            }

            return border;
        }
    }
}