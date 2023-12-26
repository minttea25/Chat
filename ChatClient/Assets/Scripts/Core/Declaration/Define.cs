namespace Core
{
    public enum AudioType : int
    {
        BGM = 0,
        Effect = 1,
        Voice = 2

        // add your type here from 2, 3 ...

    }

    public enum SceneTypes : int
    {
        INVALID = -1,

        Start = 1,
        Main = 2,
        
        // add your type here from 0, 1, ...
        //Title = 0,
        //Main = 1,
        //Store = 2,
        //Gacha = 3,
        //Game = 4,
        //Battle = 5,

    }

    public enum UIEvent : int
    {
        Click = 0,
        LongClick = 1,

        // add your type here from 1, 2, ...
    }
}
