using UnityEngine;

public static class Constants
{
    public enum ESceneType
    {
        // Main,
        // Game,
        // Ending

        PrototypeMain,
        PrototypeGame,
        PrototypeEnding
    }
    public enum ESaveLoadType
    {
        Save,
        Load
    }
    public enum InteractType
    {
        None = 0,
        Door,      
        Search,     
        Pickup,       
        Talk,       
    }
}