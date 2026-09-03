// DialogueDatabaseEditor.cs
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GraphDatabaseEditor<T> : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Scan For Nodes"))
        {
            ScanForNodes();
        }
    }

    private void ScanForNodes()
    {
        int id = 0;
        GraphDatabase<T> database = (GraphDatabase<T>)target;
        string searchFolder = database.SearchFolder;

        string[] guids = AssetDatabase.FindAssets(
            $"t:{database.NodeTypeName}",
            new[] { searchFolder }
        );

        List<GraphNode<T>> nodes = new List<GraphNode<T>>();

        foreach (string guid in guids)
        {
            GraphNode<T> node = AssetDatabase.LoadAssetByGUID<GraphNode<T>>(new GUID(guid));

            if (node != null)
            {
                node.SetID(id);
                id++;
                nodes.Add(node);
                EditorUtility.SetDirty(node);
            }
        }

        database.SetNodes(nodes);
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log(
            $"Scan complete. Found {nodes.Count} nodes."
        );
    }
}