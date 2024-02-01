using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddrKeys
{
    #region Labels
    // Scene 로드시 로드 되어야할 assets (sceneUI에 종속적인 것은 미포함)
    public const string Label_Start = "lStart";
    public const string Label_Main = "lMain";

    public const string Label_Chat = "lChat";
    public const string Label_Base = "lBase";
    public const string Label_Emoticon = "lEmoticons";
    
    #endregion


    // add keys of addressables...
    // ex) const string item1 = "item1_spawnable"; or hashcode

    public const string SimplePopupUI = "SimplePopupUI";
    public const string AlertPopupUI = "AlertPopupUI";
    
    public const string StartSceneUI = "StartSceneUI";
    public const string MainSceneUI = "MainSceneUI";

    #region MainScene
    // popup
    public const string CreateRoomPopupUI = "CreateRoomPopupUI";
    public const string EnterRoomPopupUI = "EnterRoomPopupUI";
    public const string InfoPopupUI = "InfoPopupUI";
    public const string LogoutPopupUI = "LogoutPopupUI";

    // items
    public const string RoomListItemUI = "RoomListItemUI";
    public const string ChatPanelItemUI = "ChatPanelItemUI";
    public const string ChatLeftItemUI = "ChatLeftItemUI";
    public const string ChatRightItemUI = "ChatRightItemUI";
    public const string ChatRightEmoticonItemUI = "ChatRightEmoticonItemUI";
    public const string ChatLeftEmoticonItemUI = "ChatLeftEmoticonItemUI";
    public const string ChatContentEtcItemUI = "ChatContentEtcItemUI";
    public const string EmoticonButtonItemUI = "EmoticonButtonItemUI";

    // common


    #endregion


    #region Emoticons
    public const string EmoticonLabel = "lEmoticons";

    public const string Icon_Party = "icon_party";
    public const string Icon_Laughing = "icon_laughing";
    public const string Icon_Kirakira = "icon_kirakira";
    public const string Icon_Smile = "icon_smile";
    public const string Icon_Sad = "icon_sad";
    #endregion


    #region Json Data
    public const string ChatEmoticonData = "ChatEmoticonData";
    #endregion
}
