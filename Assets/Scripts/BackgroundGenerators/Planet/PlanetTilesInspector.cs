#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;




[CustomEditor(typeof(PlanetTilesHolder))]
public class PlanetTilesInspector : Editor
{
    private PlanetTilesHolder _component;



    void OnEnable()
    {
        _component = (PlanetTilesHolder)target;
    }



    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Height-Temperature Grid", EditorStyles.boldLabel);


        var tilesProp = serializedObject.FindProperty("_serializedTileSprites");
        for (int y = 0; y < _component.GridHeight; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < _component.GridWidth; x++)
            {
                int index = y * _component.GridWidth + x;
                if (index < tilesProp.arraySize)
                {
                    var tileProp = tilesProp.GetArrayElementAtIndex(index);

                    EditorGUILayout.BeginVertical(GUILayout.Width(70));
                    var newTile = (Sprite)EditorGUILayout.ObjectField((Sprite)tileProp.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(60), GUILayout.Height(60));
                    if (newTile != (Sprite)tileProp.objectReferenceValue)
                    {
                        tileProp.objectReferenceValue = newTile;

                        
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif