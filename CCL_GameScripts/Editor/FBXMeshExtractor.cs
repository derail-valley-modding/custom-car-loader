using UnityEngine;
using UnityEditor;

public class FBXMeshExtractor
{
    private static string _progressTitle = "Extracting Meshes";
    private static string _sourceExtension = ".fbx";
    private static string _targetExtension = ".asset";


    [MenuItem("Assets/Extract Meshes", validate = true)]
    private static bool ExtractMeshesMenuItemValidate()
    {
        for( int i = 0; i < Selection.objects.Length; i++ )
        {
            if( !AssetDatabase.GetAssetPath(Selection.objects[i]).EndsWith(_sourceExtension) )
                return false;
        }
        return true;
    }

    [MenuItem("Assets/Extract Meshes")]
    private static void ExtractMeshesMenuItem()
    {
        EditorUtility.DisplayProgressBar(_progressTitle, "", 0);
        for( int i = 0; i < Selection.objects.Length; i++ )
        {
            EditorUtility.DisplayProgressBar(_progressTitle, Selection.objects[i].name, (float)i / (Selection.objects.Length - 1));
            ExtractMeshes(Selection.objects[i]);
        }
        EditorUtility.ClearProgressBar();
    }

    private static void ExtractMeshes( Object selectedObject )
    {
        //Create Folder Hierarchy
        string selectedObjectPath = AssetDatabase.GetAssetPath(selectedObject);
        string parentfolderPath = selectedObjectPath.Substring(0, selectedObjectPath.Length - (selectedObject.name.Length + 5));
        string objectFolderName = selectedObject.name;
        string objectFolderPath = parentfolderPath + "/" + objectFolderName;

        if( !AssetDatabase.IsValidFolder(objectFolderPath) )
        {
            AssetDatabase.CreateFolder(parentfolderPath, objectFolderName);
        }

        //Create Meshes
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(selectedObjectPath);

        for( int i = 0; i < objects.Length; i++ )
        {
            if( objects[i] is Mesh )
            {
                EditorUtility.DisplayProgressBar(_progressTitle, selectedObject.name + " : " + objects[i].name, (float)i / (objects.Length - 1));

                Mesh mesh = Object.Instantiate(objects[i]) as Mesh;

                AssetDatabase.CreateAsset(mesh, objectFolderPath + "/" + objects[i].name + _targetExtension);
            }
        }

        //Cleanup
        AssetDatabase.MoveAsset(selectedObjectPath, objectFolderPath + "/" + selectedObject.name + _sourceExtension);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
