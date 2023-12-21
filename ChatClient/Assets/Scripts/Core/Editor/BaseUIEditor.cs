using Core;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(BaseUI), true)]
public class BaseUIEditor : Editor
{
    protected SerializedProperty context;
    protected SerializedProperty dbRefInfo;

    protected BaseUI ui = null;

    bool foldout;

    protected void CheckProperties()
    {
        ui = target as BaseUI;

        dbRefInfo = serializedObject.FindProperty(Const.DBInfoFieldName);
        if (dbRefInfo == null)
        {
            Debug.LogError($"Can not find DBInfo property in BaseUI. [name = {Const.DBInfoFieldName}]");
        }

        context = serializedObject.FindProperty(Const.UIContextFieldName);
        context ??= serializedObject.FindProperty(Const.UIContextFieldName2);
        if (context == null)
        {
            Debug.LogWarning($"Can not find UIContext property in BaseUI. [name = {Const.UIContextFieldName} | {Const.UIContextFieldName2}");
        }
    }

    protected void ShowDBInfo()
    {
        GUIStyle titleTextStyle = new GUIStyle()
        {
            fontStyle = FontStyle.Bold,
            fontSize = EditorDefines.InfoFontSize,
        };

        if (ui.DBInfo.AllFound == false)
        {
            titleTextStyle.normal.textColor = EditorDefines.WarningColor;
            EditorGUILayout.LabelField(EditorDefines.DBRefInfoTitle_NotFound, titleTextStyle);

            if (ui.DBInfo.NotFoundObjects.Count != 0)
            {
                foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, EditorDefines.NotFoundObjectsHeader);
                if (foldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    foreach(var name in ui.DBInfo.NotFoundObjects)
                    {
                        EditorGUILayout.LabelField(name);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
        else
        {
            titleTextStyle.normal.textColor = EditorDefines.OkColor;
            EditorGUILayout.LabelField( EditorDefines.DBRefInfoTitle_AllFound, titleTextStyle);
        }
    }

    protected void ShowBaseInspector()
    {
        if (ui == null) return;

        if (ui.DBInfo.IsNew == true)
        {
            // It would be shown only before clicking the button.
            var style = new GUIStyle()
            {
                fontStyle = FontStyle.Italic | FontStyle.Bold,
            };
            style.normal.textColor = EditorDefines.WarningColor;
            EditorGUILayout.LabelField(EditorDefines.PushTheButtonText, style);
        }
        else
        {
            // check new uiobjects
            ui.DBInfo.AllFound = ui.CheckAll();

            //EditorGUILayout.PropertyField(refInfoDrawer);
            ShowDBInfo();
        }

        EditorGUILayout.Space(EditorDefines.SpacePreference);

        if (context != null)
        {
            // section 1 : UIContext Property Name Label, Assign Button
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            var style = new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = EditorDefines.HighlightFontSize,
            };
            style.normal.textColor = Color.white;
            GUILayout.Label(ui.GetContextType().Name, style);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(EditorDefines.FindAndAssignText, GUILayout.Width(EditorDefines.RightAlignButtonWidth)))
            {
                ButtonOnClicked();
            }
            GUILayout.EndHorizontal();

            // section 2 : Context Data
            EditorGUILayout.PropertyField(context, true);

            // space
            EditorGUILayout.Separator();

            // section 3 : Assign Button
            if (GUILayout.Button(EditorDefines.FindAndAssignText))
            {
                ButtonOnClicked();
            }
        }
    }


    private void OnEnable()
    {
        CheckProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ShowBaseInspector();

        serializedObject.ApplyModifiedProperties();
    }


    void ButtonOnClicked()
    {
        // update the not found object list

        ui.DBInfo.NotFoundObjects.Clear();
        List<string> notFounds = ui.AutoAssignUIs();

        // update the list of DBInfo.NotFoundObjects
        foreach (var name in notFounds) ui.DBInfo.NotFoundObjects.Add(name);

        // set IsNew false: hide the 'push button' label
        ui.DBInfo.IsNew = false;


        // notify
        EditorUtility.SetDirty(target);
    }
}
