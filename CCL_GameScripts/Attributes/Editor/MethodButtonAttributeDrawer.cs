using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(MethodButtonAttribute))]
public class MethodButtonAttributeDrawer : PropertyDrawer
{
    private int buttonCount;
    private readonly float buttonHeight = EditorGUIUtility.singleLineHeight * 2;
    private MethodButtonAttribute attr;
    
    public override void OnGUI(Rect position, SerializedProperty editorFoldout, GUIContent label)
    {
        if (editorFoldout.name.Equals("editorFoldout") == false)
        {
            LogErrorMessage(editorFoldout);
            return;
        }

        buttonCount = 0;

        Rect foldoutRect = new Rect(position.x, position.y, position.width, 5 + buttonHeight);

        editorFoldout.boolValue = EditorGUI.Foldout(foldoutRect, editorFoldout.boolValue, "Buttons", true);

        if (editorFoldout.boolValue)
        {
            buttonCount++;

            attr = (MethodButtonAttribute)base.attribute;

            foreach (var name in attr.MethodNames)
            {
                buttonCount++;

                Rect buttonRect = new Rect(position.x, position.y + ((1 + buttonHeight) * (buttonCount - 1)), position.width, buttonHeight - 1);
                if (GUI.Button(buttonRect, name))
                {
                    InvokeMethod(editorFoldout, name);
                }
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true) + (buttonHeight) * (buttonCount);
    }

    private void InvokeMethod(SerializedProperty property, string name)
    {
        Object target = property.serializedObject.targetObject;
        target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).Invoke(target, null);  
    }

    private void LogErrorMessage(SerializedProperty editorFoldout)
    {
        Debug.LogError("<color=red><b>Possible improper usage of method button attribute!</b></color>");
#if NET_4_6
        Debug.LogError($"Got field name: <b>{editorFoldout.name}</b>, Expected: <b>editorFoldout</b>");
        Debug.LogError($"Please see <b>{"Usage"}</b> at <b><i><color=blue>{"https://github.com/GlassToeStudio/UnityMethodButtonAttribute/blob/master/README.md"}</color></i></b>");
#else
        Debug.LogError(string.Format("Got field name: <b>{0}</b>, Expected: <b>editorFoldout</b>", editorFoldout.name));
        Debug.LogError("Please see <b>\"Usage\"</b> at <b><i><color=blue>\"https://github.com/GlassToeStudio/UnityMethodButtonAttribute/blob/master/README.md \"</color></i></b>");
#endif
    }
}
