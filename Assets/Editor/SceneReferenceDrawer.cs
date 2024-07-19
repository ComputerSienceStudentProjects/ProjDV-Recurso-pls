using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty sceneAsset = property.FindPropertyRelative("sceneAsset");
            SerializedProperty sceneName = property.FindPropertyRelative("sceneName");

            EditorGUI.BeginChangeCheck();
            Object value = EditorGUI.ObjectField(position, label, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
            if (EditorGUI.EndChangeCheck())
            {
                sceneAsset.objectReferenceValue = value;

                if (sceneAsset.objectReferenceValue != null)
                {
                    string path = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
                    string name = System.IO.Path.GetFileNameWithoutExtension(path);
                    sceneName.stringValue = name;
                }
                else
                {
                    sceneName.stringValue = "";
                }
            }

            EditorGUI.EndProperty();
        }
    }
}