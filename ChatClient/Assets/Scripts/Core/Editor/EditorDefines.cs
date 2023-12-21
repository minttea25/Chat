using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorDefines
{
    #region Common
    public const int SpacePreference = 10;
    public const int InfoFontSize = 15;
    public const int HighlightFontSize = 13;

    public readonly static Color OkColor = new Color(0.1f, 0.9f, 0.2f);
    public readonly static Color WarningColor = new Color(1f, 0.8f, 0.1f);
    public readonly static Color ErrorColor = new Color(0.8f, 0.2f, 0.2f);

    #endregion

    #region BaseUIEditor & Popup
    public const int RightAlignButtonWidth = 150;


    public const string FindAndAssignText = "Find UI Objects";
    public const string PushTheButtonText = "Push The Find Button.";

    public const string FindContentObjectText = "Find Content Object";
    public const string ContentPropertyNotFoundText = "The content property is not found.";

    #endregion



    public const string NotFoundObjectsHeader = "Not Found Objects (UIObject Names)";
    public const string DBRefInfoTitle_NotYet = "Push The Assign Button";
    public const string DBRefInfoTitle_AllFound = "All objects are found.";
    public const string DBRefInfoTitle_NotFound = "Some objects are not found.";
    public const string DBRefInfoTitle_NeedCheck = "Properties may have been changed";

}
