#nullable disable
#define PRETTY		//Comment out when you no longer need to read JSON to disable pretty Print system-wide
//Using doubles will cause errors in VectorTemplates.cs; Unity speaks floats
#define USEFLOAT	//Use floats for numbers instead of doubles	(enable if you're getting too many significant digits in string output)
//#define POOLING	//Currently using a build setting for this one (also it's experimental)

#if UNITY_2 || UNITY_3 || UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
using UnityEngine;
using Debug = UnityEngine.Debug;
#endif
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System;
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
	public partial class JSONObject : IEnumerable
	{
#if POOLING
	const int MAX_POOL_SIZE = 10000;
	public static Queue<JSONObject> releaseQueue = new Queue<JSONObject>();
#endif

		const int MAX_DEPTH = 100;
		const string INFINITY = "\"INFINITY\"";
		const string NEGINFINITY = "\"NEGINFINITY\"";
		const string NaN = "\"NaN\"";
		const string NEWLINE = "\r\n";
		public static readonly char[] WHITESPACE = { ' ', '\r', '\n', '\t', '\uFEFF', '\u0009' };
		public enum JsonNodeType { NULL, STRING, NUMBER, OBJECT, ARRAY, BOOL, BAKED }
		public bool IsContainer => (NodeType == JsonNodeType.ARRAY || NodeType == JsonNodeType.OBJECT);
		public JsonNodeType NodeType = JsonNodeType.NULL;
		public int Count
		{
			get
			{
				return (Children == null) ? -1 : Children.Count;
			}
		}
		public List<JSONObject> Children;
		public List<string> Keys;
		public string StringVal;

#if USEFLOAT
		public float NumberVal;
		public float FloatVal
		{
			get
			{
				return NumberVal;
			}
		}
#else
	public double NumberVal;
	public float FloatVal {
		get {
			return (float)NumberVal;
		}
	}
#endif

		public bool UseInt;
		public long IntVal;
		public bool BoolVal;
		public delegate void AddJSONContents(JSONObject self);

		public static JSONObject Null { get { return Create(JsonNodeType.NULL); } }   //an empty, null object
		public static JSONObject NewObject { get { return Create(JsonNodeType.OBJECT); } }        //an empty object
		public static JSONObject NewArray { get { return Create(JsonNodeType.ARRAY); } }     //an empty array

		public JSONObject(JsonNodeType t)
		{
			NodeType = t;
			switch (t)
			{
				case JsonNodeType.ARRAY:
					Children = new List<JSONObject>();
					break;
				case JsonNodeType.OBJECT:
					Children = new List<JSONObject>();
					Keys = new List<string>();
					break;
			}
		}
		public JSONObject(bool b)
		{
			NodeType = JsonNodeType.BOOL;
			this.BoolVal = b;
		}
#if USEFLOAT
		public JSONObject(float f)
		{
			NodeType = JsonNodeType.NUMBER;
			NumberVal = f;
		}
#else
	public JSONObject(double d) {
		type = Type.NUMBER;
		n = d;
	}
#endif
		public JSONObject(int i)
		{
			NodeType = JsonNodeType.NUMBER;
			this.IntVal = i;
			UseInt = true;
			NumberVal = i;
		}
		public JSONObject(long l)
		{
			NodeType = JsonNodeType.NUMBER;
			IntVal = l;
			UseInt = true;
			NumberVal = l;
		}
		public JSONObject(Dictionary<string, string> dic)
		{
			NodeType = JsonNodeType.OBJECT;
			Keys = new List<string>();
			Children = new List<JSONObject>();
			//Not sure if it's worth removing the foreach here
			foreach (KeyValuePair<string, string> kvp in dic)
			{
				Keys.Add(kvp.Key);
				Children.Add(CreateStringObject(kvp.Value));
			}
		}
		public JSONObject(Dictionary<string, JSONObject> dic)
		{
			NodeType = JsonNodeType.OBJECT;
			Keys = new List<string>();
			Children = new List<JSONObject>();
			//Not sure if it's worth removing the foreach here
			foreach (KeyValuePair<string, JSONObject> kvp in dic)
			{
				Keys.Add(kvp.Key);
				Children.Add(kvp.Value);
			}
		}
		public JSONObject(AddJSONContents content)
		{
			content.Invoke(this);
		}
		public JSONObject(JSONObject[] objs)
		{
			NodeType = JsonNodeType.ARRAY;
			Children = new List<JSONObject>(objs);
		}
		//Convenience function for creating a JSONObject containing a string.  This is not part of the constructor so that malformed JSON data doesn't just turn into a string object
		public static JSONObject StringObject(string val) { return CreateStringObject(val); }
		public void Absorb(JSONObject obj)
		{
			Children.AddRange(obj.Children);
			Keys.AddRange(obj.Keys);
			StringVal = obj.StringVal;
			NumberVal = obj.NumberVal;
			UseInt = obj.UseInt;
			IntVal = obj.IntVal;
			BoolVal = obj.BoolVal;
			NodeType = obj.NodeType;
		}
		public static JSONObject Create()
		{
#if POOLING
		JSONObject result = null;
		while(result == null && releaseQueue.Count > 0) {
			result = releaseQueue.Dequeue();
#if DEV
			//The following cases should NEVER HAPPEN (but they do...)
			if(result == null)
				Debug.WriteLine("wtf " + releaseQueue.Count);
			else if(result.list != null)
				Debug.WriteLine("wtflist " + result.list.Count);
#endif
		}
		if(result != null)
			return result;
#endif
			return new JSONObject();
		}
		public static JSONObject Create(JsonNodeType t)
		{
			JSONObject obj = Create();
			obj.NodeType = t;
			switch (t)
			{
				case JsonNodeType.ARRAY:
					obj.Children = new List<JSONObject>();
					break;
				case JsonNodeType.OBJECT:
					obj.Children = new List<JSONObject>();
					obj.Keys = new List<string>();
					break;
			}
			return obj;
		}
		public static JSONObject Create(bool val)
		{
			JSONObject obj = Create();
			obj.NodeType = JsonNodeType.BOOL;
			obj.BoolVal = val;
			return obj;
		}
		public static JSONObject Create(float val)
		{
			JSONObject obj = Create();
			obj.NodeType = JsonNodeType.NUMBER;
			obj.NumberVal = val;
			return obj;
		}
		public static JSONObject Create(int val)
		{
			JSONObject obj = Create();
			obj.NodeType = JsonNodeType.NUMBER;
			obj.NumberVal = val;
			obj.UseInt = true;
			obj.IntVal = val;
			return obj;
		}
		public static JSONObject Create(long val)
		{
			JSONObject obj = Create();
			obj.NodeType = JsonNodeType.NUMBER;
			obj.NumberVal = val;
			obj.UseInt = true;
			obj.IntVal = val;
			return obj;
		}
		public static JSONObject CreateStringObject(string val)
		{
			JSONObject obj = Create();
			obj.NodeType = JsonNodeType.STRING;
			obj.StringVal = val;
			return obj;
		}
		public static JSONObject CreateBakedObject(string val)
		{
			JSONObject bakedObject = Create();
			bakedObject.NodeType = JsonNodeType.BAKED;
			bakedObject.StringVal = val;
			return bakedObject;
		}
		/// <summary>
		/// Create a JSONObject by parsing string data
		/// </summary>
		/// <param name="val">The string to be parsed</param>
		/// <param name="maxDepth">The maximum depth for the parser to search.  Set this to to 1 for the first level,
		/// 2 for the first 2 levels, etc.  It defaults to -2 because -1 is the depth value that is parsed (see below)</param>
		/// <param name="storeExcessLevels">Whether to store levels beyond maxDepth in baked JSONObjects</param>
		/// <param name="strict">Whether to be strict in the parsing. For example, non-strict parsing will successfully
		/// parse "a string" into a string-type </param>
		/// <returns></returns>
		public static JSONObject Create(string val, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
		{
			JSONObject obj = Create();
			obj.Parse(val, maxDepth, storeExcessLevels, strict);
			return obj;
		}
		public static JSONObject Create(AddJSONContents content)
		{
			JSONObject obj = Create();
			content.Invoke(obj);
			return obj;
		}

		public static T FromJson<T>(string val)
			where T : class
		{
			return Create(val, -2, false, false).ToObject<T>();
		}

		public static T FromJson<T>(string val, Func<T> fallbackValue)
			where T : class
		{
			if (string.IsNullOrWhiteSpace(val))
			{
				return fallbackValue();
			}

            return Create(val, -2, false, false).ToObject<T>();
        }

		/// <remarks>Returns a new instance of <see cref="{T}"/> in case <paramref name="val"/> is <see langword="null"/> or empty.</remarks>
		public static void FromJson<T>(string val, ref T result)
			where T : class, new()
        {
            result = string.IsNullOrWhiteSpace(val) ? new T() : Create(val, -2, false, false).ToObject<T>();
        }

        public JSONObject() { }
		#region PARSE
		public JSONObject(string str, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
		{   //create a new JSONObject from a string (this will also create any children, and parse the whole string)
			Parse(str, maxDepth, storeExcessLevels, strict);
		}
		void Parse(string str, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
		{
			if (!string.IsNullOrEmpty(str))
			{
				str = str.Trim(WHITESPACE);
				if (strict)
				{
					if (str[0] != '[' && str[0] != '{')
					{
						NodeType = JsonNodeType.NULL;
#if UNITY_2 || UNITY_3 || UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
					Debug.LogWarning
#else
						Debug.WriteLine
#endif
							("Improper (strict) JSON formatting.  First character must be [ or {");
						return;
					}
				}
				if (str.Length > 0)
				{
#if UNITY_WP8 || UNITY_WSA
				if (str == "true") {
					type = Type.BOOL;
					b = true;
				} else if (str == "false") {
					type = Type.BOOL;
					b = false;
				} else if (str == "null") {
					type = Type.NULL;
#else
					if (string.Compare(str, "true", true) == 0)
					{
						NodeType = JsonNodeType.BOOL;
						BoolVal = true;
					}
					else if (string.Compare(str, "false", true) == 0)
					{
						NodeType = JsonNodeType.BOOL;
						BoolVal = false;
					}
					else if (string.Compare(str, "null", true) == 0)
					{
						NodeType = JsonNodeType.NULL;
#endif
#if USEFLOAT
					}
					else if (str == INFINITY)
					{
						NodeType = JsonNodeType.NUMBER;
						NumberVal = float.PositiveInfinity;
					}
					else if (str == NEGINFINITY)
					{
						NodeType = JsonNodeType.NUMBER;
						NumberVal = float.NegativeInfinity;
					}
					else if (str == NaN)
					{
						NodeType = JsonNodeType.NUMBER;
						NumberVal = float.NaN;
#else
				} else if(str == INFINITY) {
					type = Type.NUMBER;
					n = double.PositiveInfinity;
				} else if(str == NEGINFINITY) {
					type = Type.NUMBER;
					n = double.NegativeInfinity;
				} else if(str == NaN) {
					type = Type.NUMBER;
					n = double.NaN;
#endif
					}
					else if (str[0] == '"')
					{
						NodeType = JsonNodeType.STRING;
						this.StringVal = str.Substring(1, str.Length - 2);
					}
					else
					{
						int tokenTmp = 1;
						/*
						 * Checking for the following formatting (www.json.org)
						 * object - {"field1":value,"field2":value}
						 * array - [value,value,value]
						 * value - string	- "string"
						 *		 - number	- 0.0
						 *		 - bool		- true -or- false
						 *		 - null		- null
						 */
						int offset = 0;
						switch (str[offset])
						{
							case '{':
								NodeType = JsonNodeType.OBJECT;
								Keys = new List<string>();
								Children = new List<JSONObject>();
								break;
							case '[':
								NodeType = JsonNodeType.ARRAY;
								Children = new List<JSONObject>();
								break;
							default:
								try
								{
#if USEFLOAT
									NumberVal = System.Convert.ToSingle(str, CultureInfo.InvariantCulture);
#else
								n = System.Convert.ToDouble(str, CultureInfo.InvariantCulture);		 
#endif
									if (!str.Contains("."))
									{
										IntVal = System.Convert.ToInt64(str, CultureInfo.InvariantCulture);
										UseInt = true;
									}
									NodeType = JsonNodeType.NUMBER;
								}
								catch (System.FormatException)
								{
									NodeType = JsonNodeType.NULL;
#if UNITY_2 || UNITY_3 || UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
								Debug.LogWarning
#else
									Debug.WriteLine
#endif
									("improper JSON formatting:" + str);
								}
								return;
						}
						string propName = "";
						bool openQuote = false;
						bool inProp = false;
						int depth = 0;
						while (++offset < str.Length)
						{
							if (System.Array.IndexOf(WHITESPACE, str[offset]) > -1)
								continue;
							if (str[offset] == '\\')
							{
								offset += 1;
								continue;
							}
							if (str[offset] == '"')
							{
								if (openQuote)
								{
									if (!inProp && depth == 0 && NodeType == JsonNodeType.OBJECT)
										propName = str.Substring(tokenTmp + 1, offset - tokenTmp - 1);
									openQuote = false;
								}
								else
								{
									if (depth == 0 && NodeType == JsonNodeType.OBJECT)
										tokenTmp = offset;
									openQuote = true;
								}
							}
							if (openQuote)
								continue;
							if (NodeType == JsonNodeType.OBJECT && depth == 0)
							{
								if (str[offset] == ':')
								{
									tokenTmp = offset + 1;
									inProp = true;
								}
							}

							if (str[offset] == '[' || str[offset] == '{')
							{
								depth++;
							}
							else if (str[offset] == ']' || str[offset] == '}')
							{
								depth--;
							}
							//if  (encounter a ',' at top level)  || a closing ]/}
							if ((str[offset] == ',' && depth == 0) || depth < 0)
							{
								inProp = false;
								string inner = str.Substring(tokenTmp, offset - tokenTmp).Trim(WHITESPACE);
								if (inner.Length > 0)
								{
									if (NodeType == JsonNodeType.OBJECT)
										Keys.Add(propName);
									if (maxDepth != -1)                                                         //maxDepth of -1 is the end of the line
										Children.Add(Create(inner, (maxDepth < -1) ? -2 : maxDepth - 1, storeExcessLevels));
									else if (storeExcessLevels)
										Children.Add(CreateBakedObject(inner));

								}
								tokenTmp = offset + 1;
							}
						}
					}
				}
				else NodeType = JsonNodeType.NULL;
			}
			else NodeType = JsonNodeType.NULL;  //If the string is missing, this is a null
									//Profiler.EndSample();
		}
		#endregion
		public bool IsNumber { get { return NodeType == JsonNodeType.NUMBER; } }
		public bool IsNull { get { return NodeType == JsonNodeType.NULL; } }
		public bool IsString { get { return NodeType == JsonNodeType.STRING; } }
		public bool IsBool { get { return NodeType == JsonNodeType.BOOL; } }
		public bool IsArray { get { return NodeType == JsonNodeType.ARRAY; } }
		public bool IsObject { get { return NodeType == JsonNodeType.OBJECT || NodeType == JsonNodeType.BAKED; } }

        #region List Object Initializers
        public void Add(bool val)
		{
			Add(Create(val));
		}
		public void Add(float val)
		{
			Add(Create(val));
		}
		public void Add(long val)
		{
			Add(Create(val));
		}
		public void Add(int val)
		{
			Add(Create(val));
		}
		public void Add(string str)
		{
			Add(CreateStringObject(str));
		}
		public void Add(AddJSONContents content)
		{
			Add(Create(content));
		}
		public void Add(JSONObject obj)
		{
			if (obj)
			{       //Don't do anything if the object is null
				if (NodeType != JsonNodeType.ARRAY)
				{
					NodeType = JsonNodeType.ARRAY;      //Congratulations, son, you're an ARRAY now
					Children ??= new List<JSONObject>();
				}
				Children.Add(obj);
			}
		}
        #endregion

        #region Dictionary Object Initializers
        public void Add(string name, bool val)
		{
			Add(name, Create(val));
		}
		public void Add(string name, float val)
		{
			Add(name, Create(val));
		}
		public void Add(string name, int val)
		{
			Add(name, Create(val));
		}
		public void Add(string name, long val)
		{
			Add(name, Create(val));
		}
		public void Add(string name, AddJSONContents content)
		{
			Add(name, Create(content));
		}
		public void Add(string name, string val)
		{
			Add(name, CreateStringObject(val));
		}
		public void Add(string name, JSONObject obj)
		{
			if (obj)
			{       //Don't do anything if the object is null
				if (NodeType != JsonNodeType.OBJECT)
				{
					Keys ??= new List<string>();

					if (NodeType == JsonNodeType.ARRAY)
					{
						for (int i = 0; i < Children.Count; i++)
						{
							Keys.Add(i.ToString(CultureInfo.InvariantCulture));
						}
					}
					else
					{
						Children ??= new List<JSONObject>();
					}
					NodeType = JsonNodeType.OBJECT;     //Congratulations, son, you're an OBJECT now
				}
				Keys.Add(name);
				Children.Add(obj);
			}
		}
        #endregion

        public void SetField(string name, string val) { SetField(name, CreateStringObject(val)); }
		public void SetField(string name, bool val) { SetField(name, Create(val)); }
		public void SetField(string name, float val) { SetField(name, Create(val)); }
		public void SetField(string name, long val) { SetField(name, Create(val)); }
		public void SetField(string name, int val) { SetField(name, Create(val)); }
		public void SetField(string name, JSONObject obj)
		{
			if (HasField(name))
			{
				Children.Remove(this[name]);
				Keys.Remove(name);
			}
			Add(name, obj);
		}
		public void RemoveField(string name)
		{
			if (Keys.IndexOf(name) > -1)
			{
				Children.RemoveAt(Keys.IndexOf(name));
				Keys.Remove(name);
			}
		}
		public delegate void FieldNotFound(string name);
		public delegate void GetFieldResponse(JSONObject obj);
		public bool GetField(out bool field, string name, bool fallback)
		{
			field = fallback;
			return GetField(ref field, name);
		}
		public bool GetField(ref bool field, string name, FieldNotFound fail = null)
		{
			if (NodeType == JsonNodeType.OBJECT)
			{
				int index = Keys.IndexOf(name);
				if (index >= 0)
				{
					field = Children[index].BoolVal;
					return true;
				}
			}
            fail?.Invoke(name);
            return false;
		}
#if USEFLOAT
		public bool GetField(out float field, string name, float fallback)
		{
#else
	public bool GetField(out double field, string name, double fallback) {
#endif
			field = fallback;
			return GetField(ref field, name);
		}
#if USEFLOAT
		public bool GetField(ref float field, string name, FieldNotFound fail = null)
		{
#else
	public bool GetField(ref double field, string name, FieldNotFound fail = null) {
#endif
			if (NodeType == JsonNodeType.OBJECT)
			{
				int index = Keys.IndexOf(name);
				if (index >= 0)
				{
					field = Children[index].NumberVal;
					return true;
				}
			}
            fail?.Invoke(name);
            return false;
		}
		public bool GetField(out int field, string name, int fallback)
		{
			field = fallback;
			return GetField(ref field, name);
		}
		public bool GetField(ref int field, string name, FieldNotFound fail = null)
		{
			if (IsObject)
			{
				int index = Keys.IndexOf(name);
				if (index >= 0)
				{
					field = (int)Children[index].IntVal;
					return true;
				}
			}
            fail?.Invoke(name);
            return false;
		}
		public bool GetField(out long field, string name, long fallback)
		{
			field = fallback;
			return GetField(ref field, name);
		}
		public bool GetField(ref long field, string name, FieldNotFound fail = null)
		{
			if (IsObject)
			{
				int index = Keys.IndexOf(name);
				if (index >= 0)
				{
					field = (long)Children[index].IntVal;
					return true;
				}
			}
            fail?.Invoke(name);
            return false;
		}
		public bool GetField(out uint field, string name, uint fallback)
		{
			field = fallback;
			return GetField(ref field, name);
		}
		public bool GetField(ref uint field, string name, FieldNotFound fail = null)
		{
			if (IsObject)
			{
				int index = Keys.IndexOf(name);
				if (index >= 0)
				{
					field = (uint)Children[index].IntVal;
					return true;
				}
			}
            fail?.Invoke(name);
            return false;
		}
		public bool GetField(out string field, string name, string fallback)
		{
			field = fallback;
			return GetField(ref field, name);
		}
		public bool GetField(ref string field, string name, FieldNotFound fail = null)
		{
			if (IsObject)
			{
				int index = Keys.IndexOf(name);
				if (index >= 0)
				{
					field = Children[index].StringVal;
					return true;
				}
			}
            fail?.Invoke(name);
            return false;
		}
		public void GetField(string name, GetFieldResponse response, FieldNotFound fail = null)
		{
			if (response != null && IsObject)
			{
				int index = Keys.IndexOf(name);
				if (index >= 0)
				{
					response.Invoke(Children[index]);
					return;
				}
			}
            fail?.Invoke(name);
        }
		public JSONObject GetField(string name)
		{
			if (IsObject)
				for (int i = 0; i < Keys.Count; i++)
					if (Keys[i] == name)
						return Children[i];
			return null;
		}
		public bool HasFields(string[] names)
		{
			if (!IsObject)
				return false;
			for (int i = 0; i < names.Length; i++)
				if (!Keys.Contains(names[i]))
					return false;
			return true;
		}
		public bool HasField(string name)
		{
			if (!IsObject)
				return false;
			for (int i = 0; i < Keys.Count; i++)
				if (Keys[i] == name)
					return true;
			return false;
		}
		public void Clear()
		{
			NodeType = JsonNodeType.NULL;
			Children?.Clear();
			Keys?.Clear();
			StringVal = "";
			NumberVal = 0;
			BoolVal = false;
		}
		/// <summary>
		/// Copy a JSONObject. This could probably work better
		/// </summary>
		/// <returns></returns>
		public JSONObject Copy()
		{
			return Create(Print());
		}
		/*
		 * The Merge function is experimental. Use at your own risk.
		 */
		public void Merge(JSONObject obj)
		{
			MergeRecur(this, obj);
		}
		/// <summary>
		/// Merge object right into left recursively
		/// </summary>
		/// <param name="left">The left (base) object</param>
		/// <param name="right">The right (new) object</param>
		static void MergeRecur(JSONObject left, JSONObject right)
		{
			if (left.NodeType == JsonNodeType.NULL)
				left.Absorb(right);
			else if (left.NodeType == JsonNodeType.OBJECT && right.NodeType == JsonNodeType.OBJECT)
			{
				for (int i = 0; i < right.Children.Count; i++)
				{
					string key = right.Keys[i];
					if (right[i].IsContainer)
					{
						if (left.HasField(key))
							MergeRecur(left[key], right[i]);
						else
							left.Add(key, right[i]);
					}
					else
					{
						if (left.HasField(key))
							left.SetField(key, right[i]);
						else
							left.Add(key, right[i]);
					}
				}
			}
			else if (left.NodeType == JsonNodeType.ARRAY && right.NodeType == JsonNodeType.ARRAY)
			{
				if (right.Count > left.Count)
				{
#if UNITY_2 || UNITY_3 || UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
				Debug.LogError
#else
					Debug.WriteLine
#endif
					("Cannot merge arrays when right object has more elements");
					return;
				}
				for (int i = 0; i < right.Children.Count; i++)
				{
					if (left[i].NodeType == right[i].NodeType)
					{           //Only overwrite with the same type
						if (left[i].IsContainer)
							MergeRecur(left[i], right[i]);
						else
						{
							left[i] = right[i];
						}
					}
				}
			}
		}
		public void Bake()
		{
			if (NodeType != JsonNodeType.BAKED)
			{
				StringVal = Print();
				NodeType = JsonNodeType.BAKED;
			}
		}
		public IEnumerable BakeAsync()
		{
			if (NodeType != JsonNodeType.BAKED)
			{
				foreach (string s in PrintAsync())
				{
					if (s == null)
						yield return s;
					else
					{
						StringVal = s;
					}
				}
				NodeType = JsonNodeType.BAKED;
			}
		}

		public string Print(bool pretty = false)
		{
			StringBuilder builder = new StringBuilder();
			Stringify(0, builder, pretty);
			return builder.ToString();
		}
		public IEnumerable<string> PrintAsync(bool pretty = false)
		{
			StringBuilder builder = new StringBuilder();
			printWatch.Reset();
			printWatch.Start();
			foreach (IEnumerable _ in StringifyAsync(0, builder, pretty))
			{
				yield return null;
			}
			yield return builder.ToString();
		}

		#region STRINGIFY
		const float maxFrameTime = 0.008f;
		static readonly Stopwatch printWatch = new Stopwatch();
		IEnumerable StringifyAsync(int depth, StringBuilder builder, bool pretty = false)
		{   //Convert the JSONObject into a string
			//Profiler.BeginSample("JSONprint");
			if (depth++ > MAX_DEPTH)
			{
#if UNITY_2 || UNITY_3 || UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
			Debug.Log
#else
				Debug.WriteLine
#endif
				("reached max depth!");
				yield break;
			}
			if (printWatch.Elapsed.TotalSeconds > maxFrameTime)
			{
				printWatch.Reset();
				yield return null;
				printWatch.Start();
			}
			switch (NodeType)
			{
				case JsonNodeType.BAKED:
					builder.Append(StringVal);
					break;
				case JsonNodeType.STRING:
					builder.AppendFormat("\"{0}\"", StringVal);
					break;
				case JsonNodeType.NUMBER:
					if (UseInt)
					{
						builder.Append(IntVal.ToString(CultureInfo.InvariantCulture));
					}
					else
					{
#if USEFLOAT
						if (float.IsInfinity(NumberVal))
							builder.Append(INFINITY);
						else if (float.IsNegativeInfinity(NumberVal))
							builder.Append(NEGINFINITY);
						else if (float.IsNaN(NumberVal))
							builder.Append(NaN);
#else
				if(double.IsInfinity(n))
					builder.Append(INFINITY);
				else if(double.IsNegativeInfinity(n))
					builder.Append(NEGINFINITY);
				else if(double.IsNaN(n))
					builder.Append(NaN);
#endif
						else
							builder.Append(NumberVal.ToString(CultureInfo.InvariantCulture));
					}
					break;
				case JsonNodeType.OBJECT:
					builder.Append("{");
					if (Children.Count > 0)
					{
#if (PRETTY)        //for a bit more readability, comment the define above to disable system-wide                                                                                  
						if (pretty)
							builder.Append(NEWLINE);
#endif
						for (int i = 0; i < Children.Count; i++)
						{
							string key = Keys[i];
							JSONObject obj = Children[i];
							if (obj)
							{
#if (PRETTY)
								if (pretty)
									for (int j = 0; j < depth; j++)
										builder.Append("\t"); //for a bit more readability
#endif
								builder.AppendFormat("\"{0}\":", key);
								foreach (IEnumerable e in obj.StringifyAsync(depth, builder, pretty))
									yield return e;
								builder.Append(",");
#if (PRETTY)
								if (pretty)
									builder.Append(NEWLINE);
#endif
							}
						}
#if (PRETTY)
						if (pretty)
							builder.Length -= 2;
						else
#endif
							builder.Length--;
					}
#if (PRETTY)
					if (pretty && Children.Count > 0)
					{
						builder.Append(NEWLINE);
						for (int j = 0; j < depth - 1; j++)
							builder.Append("\t"); //for a bit more readability
					}
#endif
					builder.Append("}");
					break;
				case JsonNodeType.ARRAY:
					builder.Append("[");
					if (Children.Count > 0)
					{
#if (PRETTY)
						if (pretty)
							builder.Append(NEWLINE); //for a bit more readability
#endif
						for (int i = 0; i < Children.Count; i++)
						{
							if (Children[i])
							{
#if (PRETTY)
								if (pretty)
									for (int j = 0; j < depth; j++)
										builder.Append("\t"); //for a bit more readability
#endif
								foreach (IEnumerable e in Children[i].StringifyAsync(depth, builder, pretty))
									yield return e;
								builder.Append(",");
#if (PRETTY)
								if (pretty)
									builder.Append(NEWLINE); //for a bit more readability
#endif
							}
						}
#if (PRETTY)
						if (pretty)
							builder.Length -= 2;
						else
#endif
							builder.Length--;
					}
#if (PRETTY)
					if (pretty && Children.Count > 0)
					{
						builder.Append(NEWLINE);
						for (int j = 0; j < depth - 1; j++)
							builder.Append("\t"); //for a bit more readability
					}
#endif
					builder.Append("]");
					break;
				case JsonNodeType.BOOL:
					if (BoolVal)
						builder.Append("true");
					else
						builder.Append("false");
					break;
				case JsonNodeType.NULL:
					builder.Append("null");
					break;
			}
			//Profiler.EndSample();
		}
		//TODO: Refactor Stringify functions to share core logic
		/*
		 * I know, I know, this is really bad form.  It turns out that there is a
		 * significant amount of garbage created when calling as a coroutine, so this
		 * method is duplicated.  Hopefully there won't be too many future changes, but
		 * I would still like a more elegant way to optionaly yield
		 */
		void Stringify(int depth, StringBuilder builder, bool pretty = false)
		{   //Convert the JSONObject into a string
			//Profiler.BeginSample("JSONprint");
			if (depth++ > MAX_DEPTH)
			{
#if UNITY_2 || UNITY_3 || UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
			Debug.Log
#else
				Debug.WriteLine
#endif
				("reached max depth!");
				return;
			}
			switch (NodeType)
			{
				case JsonNodeType.BAKED:
					builder.Append(StringVal);
					break;
				case JsonNodeType.STRING:
					builder.AppendFormat("\"{0}\"", StringVal);
					break;
				case JsonNodeType.NUMBER:
					if (UseInt)
					{
						builder.Append(IntVal.ToString(CultureInfo.InvariantCulture));
					}
					else
					{
#if USEFLOAT
						if (float.IsInfinity(NumberVal))
							builder.Append(INFINITY);
						else if (float.IsNegativeInfinity(NumberVal))
							builder.Append(NEGINFINITY);
						else if (float.IsNaN(NumberVal))
							builder.Append(NaN);
#else
				if(double.IsInfinity(n))
					builder.Append(INFINITY);
				else if(double.IsNegativeInfinity(n))
					builder.Append(NEGINFINITY);
				else if(double.IsNaN(n))
					builder.Append(NaN);
#endif
						else
							builder.Append(NumberVal.ToString(CultureInfo.InvariantCulture));
					}
					break;
				case JsonNodeType.OBJECT:
					builder.Append("{");
					if (Children.Count > 0)
					{
#if (PRETTY)        //for a bit more readability, comment the define above to disable system-wide                                                                                  
						if (pretty)
							builder.Append("\n");
#endif
						for (int i = 0; i < Children.Count; i++)
						{
							string key = Keys[i];
							JSONObject obj = Children[i];
							if (obj)
							{
#if (PRETTY)
								if (pretty)
									for (int j = 0; j < depth; j++)
										builder.Append("\t"); //for a bit more readability
#endif
								builder.AppendFormat("\"{0}\":", key);
								obj.Stringify(depth, builder, pretty);
								builder.Append(",");
#if (PRETTY)
								if (pretty)
									builder.Append("\n");
#endif
							}
						}
#if (PRETTY)
						if (pretty)
							builder.Length -= 2;
						else
#endif
							builder.Length--;
					}
#if (PRETTY)
					if (pretty && Children.Count > 0)
					{
						builder.Append("\n");
						for (int j = 0; j < depth - 1; j++)
							builder.Append("\t"); //for a bit more readability
					}
#endif
					builder.Append("}");
					break;
				case JsonNodeType.ARRAY:
					builder.Append("[");
					if (Children.Count > 0)
					{
#if (PRETTY)
						if (pretty)
							builder.Append("\n"); //for a bit more readability
#endif
						for (int i = 0; i < Children.Count; i++)
						{
							if (Children[i])
							{
#if (PRETTY)
								if (pretty)
									for (int j = 0; j < depth; j++)
										builder.Append("\t"); //for a bit more readability
#endif
								Children[i].Stringify(depth, builder, pretty);
								builder.Append(",");
#if (PRETTY)
								if (pretty)
									builder.Append("\n"); //for a bit more readability
#endif
							}
						}
#if (PRETTY)
						if (pretty)
							builder.Length -= 2;
						else
#endif
							builder.Length--;
					}
#if (PRETTY)
					if (pretty && Children.Count > 0)
					{
						builder.Append("\n");
						for (int j = 0; j < depth - 1; j++)
							builder.Append("\t"); //for a bit more readability
					}
#endif
					builder.Append("]");
					break;
				case JsonNodeType.BOOL:
					if (BoolVal)
						builder.Append("true");
					else
						builder.Append("false");
					break;
				case JsonNodeType.NULL:
					builder.Append("null");
					break;
			}
			//Profiler.EndSample();
		}
		#endregion
#if UNITY_2 || UNITY_3 || UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
	public static implicit operator WWWForm(JSONObject obj) {
		WWWForm form = new WWWForm();
		for(int i = 0; i < obj.list.Count; i++) {
			string key = i.ToString(CultureInfo.InvariantCulture);
			if(obj.type == Type.OBJECT)
				key = obj.keys[i];
			string val = obj.list[i].ToString();
			if(obj.list[i].type == Type.STRING)
				val = val.Replace("\"", "");
			form.AddField(key, val);
		}
		return form;
	}
#endif
		public JSONObject this[int index]
		{
			get
			{
				if (Children.Count > index) return Children[index];
				return null;
			}
			set
			{
				if (Children.Count > index)
					Children[index] = value;
			}
		}
		public JSONObject this[string index]
		{
			get
			{
				return GetField(index);
			}
			set
			{
				SetField(index, value);
			}
		}
		public override string ToString()
		{
			return Print();
		}
		public string ToString(bool pretty)
		{
			return Print(pretty);
		}
		public Dictionary<string, string> ToDictionary()
		{
			if (NodeType == JsonNodeType.OBJECT)
			{
				Dictionary<string, string> result = new Dictionary<string, string>();
				for (int i = 0; i < Children.Count; i++)
				{
					JSONObject val = Children[i];
					switch (val.NodeType)
					{
						case JsonNodeType.STRING: result.Add(Keys[i], val.StringVal); break;
						case JsonNodeType.NUMBER: result.Add(Keys[i], val.NumberVal.ToString(CultureInfo.InvariantCulture)); break;
						case JsonNodeType.BOOL: result.Add(Keys[i], val.BoolVal.ToString(CultureInfo.InvariantCulture)); break;
						default:
#if UNITY_2 || UNITY_3 || UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
						Debug.LogWarning
#else
							Debug.WriteLine
#endif
							("Omitting object: " + Keys[i] + " in dictionary conversion");
							break;
					}
				}
				return result;
			}
#if UNITY_2 || UNITY_3 || UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
		Debug.Log
#else
			Debug.WriteLine
#endif
			("Tried to turn non-Object JSONObject into a dictionary");
			return null;
		}
		public static implicit operator bool(JSONObject o)
		{
			return o != null;
		}
#if POOLING
	static bool pool = true;
	public static void ClearPool() {
		pool = false;
		releaseQueue.Clear();
		pool = true;
	}

	~JSONObject() {
		if(pool && releaseQueue.Count < MAX_POOL_SIZE) {
			type = Type.NULL;
			list = null;
			keys = null;
			str = "";
			n = 0;
			b = false;
			releaseQueue.Enqueue(this);
		}
	}
#endif

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		public JSONObjectEnumer GetEnumerator()
		{
			return new JSONObjectEnumer(this);
		}
	}

	public class JSONObjectEnumer : IEnumerator
	{
		public JSONObject _jobj;

		// Enumerators are positioned before the first element
		// until the first MoveNext() call.
		int position = -1;

		public JSONObjectEnumer(JSONObject jsonObject)
		{
			Debug.Assert(jsonObject.IsContainer); //must be an array or object to itterate
			_jobj = jsonObject;
		}

		public bool MoveNext()
		{
			position++;
			return (position < _jobj.Count);
		}

		public void Reset()
		{
			position = -1;
		}

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		public JSONObject Current
		{
			get
			{
				if (_jobj.IsArray)
				{
					return _jobj[position];
				}
				else
				{
					string key = _jobj.Keys[position];
					return _jobj[key];
				}
			}
		}
	}
}
