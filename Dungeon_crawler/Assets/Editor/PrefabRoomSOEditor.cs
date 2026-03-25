#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabRoomSO))]
public class PrefabRoomSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PrefabRoomSO so = (PrefabRoomSO)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Extract Data From Prefab"))
        {
            if (so.prefab == null)
            {
                Debug.LogError("PrefabRoomSO: No prefab assigned.");
            }
            else
            {
                so.extract();
                EditorUtility.SetDirty(so);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif