using System.Collections;
using UnityEngine;

namespace Audio.Enums
{
    #region EditorEnums
    public enum SoundOptions
    {
        Sequentially,
        Concurrent,
        Random,
        Variable
    }

    public enum VarOptions
    {
        Float,
        Int,
        Bool,
        Enum
    }
    #endregion

    #region ID
    public enum SFXID
    {
        NULL,
        Dialogue,
        Object
    }

    public enum MusicID
    {
        NULL,
        Tabern
    }

    public enum AudioEventsID
    {
        NULL
    }
    #endregion
}