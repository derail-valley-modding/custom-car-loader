using UnityEngine;

namespace CCL.Types
{
    public static class GUIHelper
    {
        private static Texture2D? s_tex;
        private static Texture2D Tex
        {
            get
            {
                if (s_tex == null)
                {
                    s_tex = new Texture2D(2, 2);

                    s_tex.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
                    s_tex.Apply();
                }

                return s_tex;
            }
        }

        public static void DrawLine(Vector2 start, Vector2 end, float width)
        {
            Vector2 d = end - start;
            float a = Mathf.Rad2Deg * Mathf.Atan2(d.y, d.x);
            GUIUtility.RotateAroundPivot(a, start);
            DrawRectangle(new Rect(start.x, start.y - (width * 0.5f), d.magnitude, width));
            GUIUtility.RotateAroundPivot(-a, start);
        }

        public static void DrawPixel(Vector2 position)
        {
            DrawRectangle(new Rect(position.x, position.y, 1, 1));
        }

        public static void DrawPixel(Vector2 position, float size)
        {
            DrawRectangle(new Rect(position.x, position.y, size, size));
        }

        public static void DrawPixel(Vector2 position, Vector2 scale)
        {
            DrawRectangle(new Rect(position.x, position.y, scale.x, scale.y));
        }

        private static void DrawRectangle(Rect rect)
        {
            GUI.DrawTexture(rect, Tex, ScaleMode.StretchToFill, true, 0, GUI.color, 0, 0);
        }

        public static void DrawTexture(Rect rect, Texture2D tex, Color c)
        {
            GUI.DrawTexture(rect, tex, ScaleMode.StretchToFill, true, 0, c, 0, 0);
        }
    }
}
