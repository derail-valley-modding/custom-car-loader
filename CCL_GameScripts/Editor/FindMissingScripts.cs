using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Find missing scripts in object.
/// Optionally delete them.
/// </summary>
public class FindMissingScripts : EditorWindow 
{
    static int go_count = 0, components_count = 0, missing_count = 0;
    private bool deleteMissingScripts = false;

    [MenuItem("Tools/Find Missing Scripts")]
    public static void ShowWindow()
    {
        var window = (FindMissingScripts)EditorWindow.GetWindow(typeof(FindMissingScripts));
        window.deleteMissingScripts = false;
    }
 
    public void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
        {
            FindInSelected(deleteMissingScripts);
        }

        deleteMissingScripts = GUILayout.Toggle(deleteMissingScripts, "Delete Missing Scripts");

    }
    private static void FindInSelected(bool delete)
    {
        GameObject[] go = Selection.gameObjects;
        go_count = 0;
		components_count = 0;
		missing_count = 0;
        foreach (GameObject g in go)
        {
   			FindInGO(g, delete);
        }
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
    }
 
    private static void FindInGO(GameObject g, bool delete)
    {
        go_count++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            components_count++;
            if (components[i] == null)
            {
                missing_count++;
                string s = g.name;
                Transform t = g.transform;
                while (t.parent != null) 
                {
                    s = t.parent.name +"/"+s;
                    t = t.parent;
                }

                Debug.Log ($"{s} has an empty script attached in position: {i}", g);
            }
        }

        //delete scripts
        if (delete)
        {
            var num = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(g);
            Debug.Log($"Deleted {num} script on GameObject {g.name}");
        }

        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );
            FindInGO(childT.gameObject, delete);
        }
    }
}