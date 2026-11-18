using System;
using System.Collections.Generic;

namespace Audio.Enums
{
    #region EditorEnums

    public enum SoundOptions
    {
        Sequentially,
        Concurrent,
        Random,
        Single
    }

    public enum VarOptions
    {
        Float,
        Int,
        Bool,
        Enum
    }

    #endregion EditorEnums

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

    public enum AudioBankID
    {
        FloorBank
    }

    #endregion ID

    public enum EnumType
    {
        Floor
    }

    public enum Floor
    {
        Wood,
        Stone,
        Grass
    }

    public static class AudioEnums
    {
        public static Dictionary<EnumType, Type> dictionary = new Dictionary<EnumType, Type> { { EnumType.Floor, typeof(Floor) } };
    }
}