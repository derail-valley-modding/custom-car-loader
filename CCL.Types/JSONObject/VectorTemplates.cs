using UnityEngine;

/*
Copyright (c) 2010-2019 Matt Schoen

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

namespace CCL.Types.Json
{
	public static partial class JSONTemplates
	{

		/*
		 * Vector2
		 */
		public static Vector2 ToVector2(JSONObject obj)
		{
			float x = obj["x"] ? obj["x"].FloatVal : 0;
			float y = obj["y"] ? obj["y"].FloatVal : 0;
			return new Vector2(x, y);
		}
		public static JSONObject FromVector2(Vector2 v)
		{
			JSONObject vdata = JSONObject.NewObject;
			if (v.x != 0) vdata.Add("x", v.x);
			if (v.y != 0) vdata.Add("y", v.y);
			return vdata;
		}
		/*
		 * Vector3
		 */
		public static JSONObject FromVector3(Vector3 v)
		{
			JSONObject vdata = JSONObject.NewObject;
			if (v.x != 0) vdata.Add("x", v.x);
			if (v.y != 0) vdata.Add("y", v.y);
			if (v.z != 0) vdata.Add("z", v.z);
			return vdata;
		}
		public static Vector3 ToVector3(JSONObject obj)
		{
			float x = obj["x"] ? obj["x"].FloatVal : 0;
			float y = obj["y"] ? obj["y"].FloatVal : 0;
			float z = obj["z"] ? obj["z"].FloatVal : 0;
			return new Vector3(x, y, z);
		}
		/*
		 * Vector4
		 */
		public static JSONObject FromVector4(Vector4 v)
		{
			JSONObject vdata = JSONObject.NewObject;
			if (v.x != 0) vdata.Add("x", v.x);
			if (v.y != 0) vdata.Add("y", v.y);
			if (v.z != 0) vdata.Add("z", v.z);
			if (v.w != 0) vdata.Add("w", v.w);
			return vdata;
		}
		public static Vector4 ToVector4(JSONObject obj)
		{
			float x = obj["x"] ? obj["x"].FloatVal : 0;
			float y = obj["y"] ? obj["y"].FloatVal : 0;
			float z = obj["z"] ? obj["z"].FloatVal : 0;
			float w = obj["w"] ? obj["w"].FloatVal : 0;
			return new Vector4(x, y, z, w);
		}
		/*
		 * Matrix4x4
		 */
		public static JSONObject FromMatrix4x4(Matrix4x4 m)
		{
			JSONObject mdata = JSONObject.NewObject;
			if (m.m00 != 0) mdata.Add("m00", m.m00);
			if (m.m01 != 0) mdata.Add("m01", m.m01);
			if (m.m02 != 0) mdata.Add("m02", m.m02);
			if (m.m03 != 0) mdata.Add("m03", m.m03);
			if (m.m10 != 0) mdata.Add("m10", m.m10);
			if (m.m11 != 0) mdata.Add("m11", m.m11);
			if (m.m12 != 0) mdata.Add("m12", m.m12);
			if (m.m13 != 0) mdata.Add("m13", m.m13);
			if (m.m20 != 0) mdata.Add("m20", m.m20);
			if (m.m21 != 0) mdata.Add("m21", m.m21);
			if (m.m22 != 0) mdata.Add("m22", m.m22);
			if (m.m23 != 0) mdata.Add("m23", m.m23);
			if (m.m30 != 0) mdata.Add("m30", m.m30);
			if (m.m31 != 0) mdata.Add("m31", m.m31);
			if (m.m32 != 0) mdata.Add("m32", m.m32);
			if (m.m33 != 0) mdata.Add("m33", m.m33);
			return mdata;
		}
		public static Matrix4x4 ToMatrix4x4(JSONObject obj)
		{
			Matrix4x4 result = new Matrix4x4();
			if (obj["m00"]) result.m00 = obj["m00"].FloatVal;
			if (obj["m01"]) result.m01 = obj["m01"].FloatVal;
			if (obj["m02"]) result.m02 = obj["m02"].FloatVal;
			if (obj["m03"]) result.m03 = obj["m03"].FloatVal;
			if (obj["m10"]) result.m10 = obj["m10"].FloatVal;
			if (obj["m11"]) result.m11 = obj["m11"].FloatVal;
			if (obj["m12"]) result.m12 = obj["m12"].FloatVal;
			if (obj["m13"]) result.m13 = obj["m13"].FloatVal;
			if (obj["m20"]) result.m20 = obj["m20"].FloatVal;
			if (obj["m21"]) result.m21 = obj["m21"].FloatVal;
			if (obj["m22"]) result.m22 = obj["m22"].FloatVal;
			if (obj["m23"]) result.m23 = obj["m23"].FloatVal;
			if (obj["m30"]) result.m30 = obj["m30"].FloatVal;
			if (obj["m31"]) result.m31 = obj["m31"].FloatVal;
			if (obj["m32"]) result.m32 = obj["m32"].FloatVal;
			if (obj["m33"]) result.m33 = obj["m33"].FloatVal;
			return result;
		}
		/*
		 * Quaternion
		 */
		public static JSONObject FromQuaternion(Quaternion q)
		{
			JSONObject qdata = JSONObject.NewObject;
			if (q.w != 0) qdata.Add("w", q.w);
			if (q.x != 0) qdata.Add("x", q.x);
			if (q.y != 0) qdata.Add("y", q.y);
			if (q.z != 0) qdata.Add("z", q.z);
			return qdata;
		}
		public static Quaternion ToQuaternion(JSONObject obj)
		{
			float x = obj["x"] ? obj["x"].FloatVal : 0;
			float y = obj["y"] ? obj["y"].FloatVal : 0;
			float z = obj["z"] ? obj["z"].FloatVal : 0;
			float w = obj["w"] ? obj["w"].FloatVal : 0;
			return new Quaternion(x, y, z, w);
		}
		/*
		 * Color
		 */
		public static JSONObject FromColor(Color c)
		{
			JSONObject cdata = JSONObject.NewObject;
			if (c.r != 0) cdata.Add("r", c.r);
			if (c.g != 0) cdata.Add("g", c.g);
			if (c.b != 0) cdata.Add("b", c.b);
			if (c.a != 0) cdata.Add("a", c.a);
			return cdata;
		}
		public static Color ToColor(JSONObject obj)
		{
			Color c = new Color();
			for (int i = 0; i < obj.Count; i++)
			{
				switch (obj.Keys[i])
				{
					case "r": c.r = obj[i].FloatVal; break;
					case "g": c.g = obj[i].FloatVal; break;
					case "b": c.b = obj[i].FloatVal; break;
					case "a": c.a = obj[i].FloatVal; break;
				}
			}
			return c;
		}
		/*
		 * Layer Mask
		 */
		public static JSONObject FromLayerMask(LayerMask l)
		{
			JSONObject result = JSONObject.NewObject;
			result.Add("value", l.value);
			return result;
		}
		public static LayerMask ToLayerMask(JSONObject obj)
		{
			LayerMask l = new LayerMask { value = (int)obj["value"].NumberVal };
			return l;
		}
		public static JSONObject FromRect(Rect r)
		{
			JSONObject result = JSONObject.NewObject;
			if (r.x != 0) result.Add("x", r.x);
			if (r.y != 0) result.Add("y", r.y);
			if (r.height != 0) result.Add("height", r.height);
			if (r.width != 0) result.Add("width", r.width);
			return result;
		}
		public static Rect ToRect(JSONObject obj)
		{
			Rect r = new Rect();
			for (int i = 0; i < obj.Count; i++)
			{
				switch (obj.Keys[i])
				{
					case "x": r.x = obj[i].FloatVal; break;
					case "y": r.y = obj[i].FloatVal; break;
					case "height": r.height = obj[i].FloatVal; break;
					case "width": r.width = obj[i].FloatVal; break;
				}
			}
			return r;
		}
		public static JSONObject FromRectOffset(RectOffset r)
		{
			JSONObject result = JSONObject.NewObject;
			if (r.bottom != 0) result.Add("bottom", r.bottom);
			if (r.left != 0) result.Add("left", r.left);
			if (r.right != 0) result.Add("right", r.right);
			if (r.top != 0) result.Add("top", r.top);
			return result;
		}
		public static RectOffset ToRectOffset(JSONObject obj)
		{
			RectOffset r = new RectOffset();
			for (int i = 0; i < obj.Count; i++)
			{
				switch (obj.Keys[i])
				{
					case "bottom": r.bottom = (int)obj[i].NumberVal; break;
					case "left": r.left = (int)obj[i].NumberVal; break;
					case "right": r.right = (int)obj[i].NumberVal; break;
					case "top": r.top = (int)obj[i].NumberVal; break;
				}
			}
			return r;
		}
	}
}
