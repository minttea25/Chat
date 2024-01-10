using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "PlayerSettings/Config")]
public class PlayerConfig : ScriptableObject
{
    public bool UseTimestamp;
    public string LocationPath;
    public string ExecutableFileName;
}
