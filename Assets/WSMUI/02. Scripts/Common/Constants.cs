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
        Ending
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