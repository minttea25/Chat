using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Const
    {
        public const int INVALID = -1;

        public const int ContentObjectSearchDepthLevel = 2;

        #region For DontDestoyedOnLoad Object Names

        public const string ManagerName = "!Managers";
        public const string AudioName = "!Sound";
        public const string AudioSourceName = "!SoundSource";

        #endregion

        public const string RootUIName = "@RootUI";
        public const string AddressableEventSystemKey = "EventSystems";
        public const string EventSystemsName = "@EventSystems";

        // Field Names for Relfections. DO NOT CHANGE THESE VALUES!
        #region Field Names
        public const string BindObjectFieldName = "BindObject";
        public const string BindObjectTypeFieldName = "BindObjectType";
        public const string UIContextFieldName = "Context";
        public const string UIContextFieldName2 = "context";

        public const string DBInfoFieldName = "DBInfo";

        public const string ContentObjectTag = "UIContent";
        public const string ContentObjectName = "Content";
        public const string ContentObjectFieldName = "ContentObject";

        #endregion


        readonly static public int DefaultAudioSourceCount = Enum.GetValues(typeof(AudioType)).Length;
    }
}
