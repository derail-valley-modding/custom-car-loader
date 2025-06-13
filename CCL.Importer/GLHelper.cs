using UnityEngine;

namespace CCL.Importer
{
    internal static class GLHelper
    {
        private static Material? s_mat;
        public static Material LineMaterial => Extensions.GetCached(ref s_mat,
            () => new Material(Shader.Find("Hidden/Internal-Colored")));

        /// <summary>
        /// Shorthand for <see cref="GL.PushMatrix()"/>, <see cref="GL.LoadPixelMatrix"/>, <see cref="GL.Begin"/>
        /// with <see cref="GL.LINE_STRIP"/> and <see cref="GL.Color"/>.
        /// Also sets the pass to the first of <see cref="LineMaterial"/>.
        /// </summary>
        /// <param name="c">The colour of the line.</param>
        public static void StartPixelLineStrip(Color c)
        {
            GL.PushMatrix();
            LineMaterial.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.LINE_STRIP);
            GL.Color(c);
        }

        /// <summary>
        /// Shorthand for <see cref="GL.PushMatrix()"/>, <see cref="GL.LoadPixelMatrix"/>, <see cref="GL.Begin"/>
        /// with <see cref="GL.LINES"/> and <see cref="GL.Color"/>.
        /// Also sets the pass to the first of <see cref="LineMaterial"/>.
        /// </summary>
        /// <param name="c">The colour of the line.</param>
        public static void StartPixelLines(Color c)
        {
            GL.PushMatrix();
            LineMaterial.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.LINES);
            GL.Color(c);
        }

        /// <summary>
        /// Shorthand for <see cref="GL.End"/> and <see cref="GL.PopMatrix"/>.
        /// </summary>
        public static void EndAndPop()
        {
            GL.End();
            GL.PopMatrix();
        }

        /// <summary>
        /// Coverts a position in <see cref="GUI"/> coordinates to <see cref="GL"/> coordinates.
        /// </summary>
        /// <param name="pos">The position in GUI coordinates.</param>
        /// <param name="z">Optional Z value.</param>
        /// <returns>A <see cref="Vector3"/> transformed from the original position.</returns>
        public static Vector3 GUIToGLPosition(Vector2 pos, float z = 0)
        {
            return new Vector3(pos.x, Screen.height - pos.y, z);
        }

        /// <summary>
        /// Coverts a <see cref="Rect"/> in <see cref="GUI"/> coordinates to <see cref="GL"/> coordinates.
        /// </summary>
        /// <param name="view">The original rectangle in GUI coordinates.</param>
        /// <returns>A <see cref="Rect"/> in GL coordinates.</returns>
        public static Rect GUIToGLView(Rect view)
        {
            return new Rect(view.x, Screen.height - view.y - view.height, view.width, view.height);
        }

        /// <summary>
        /// Draws a <see cref="Rect"/> on the screen.
        /// </summary>
        /// <param name="rect">The rectangle to draw.</param>
        /// <param name="c">The colour of the rectangle.</param>
        public static void DebugRect(Rect rect, Color c)
        {
            StartPixelLineStrip(c);
            GL.Vertex(new Vector2(rect.xMin, rect.yMin));
            GL.Vertex(new Vector2(rect.xMax, rect.yMin));
            GL.Vertex(new Vector2(rect.xMax, rect.yMax));
            GL.Vertex(new Vector2(rect.xMin, rect.yMax));
            GL.Vertex(new Vector2(rect.xMin, rect.yMin));
            EndAndPop();
        }
    }
}
