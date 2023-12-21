using Core;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BaseUIPopup), true)]
public class BaseUIPopupEditor : BaseUIEditor
{
    BaseUIPopup popup = null;
    SerializedProperty content;

    private void OnEnable()
    {
        CheckProperties();

        popup = target as BaseUIPopup;

        content = serializedObject.FindProperty(Const.ContentObjectFieldName);

        if (content == null)
        {
            Debug.LogError($"Can not find ContentObject property in BaseUIPopup. [name={Const.ContentObjectFieldName}]");
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ShowBaseInspector();

        EditorGUILayout.Space(EditorDefines.SpacePreference);
        //EditorGUILayout.Separator(); // 6f

        if (content != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(content, true);
            if (GUILayout.Button(EditorDefines.FindContentObjectText, GUILayout.Width(150)))
            {
                if (popup.FindContentObject() == false)
                {
                    Debug.LogWarning($"Can not find ContentObject reference. The ContentObject should be named '{Const.ContentObjectName}' or tagged with '{Const.ContentObjectTag}'");
                }

                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField(EditorDefines.ContentPropertyNotFoundText);
        }

        serializedObject.ApplyModifiedProperties();
    }
}