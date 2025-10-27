#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PlayModeResetter
{
    // This static constructor runs automatically when the editor loads,
    // and whenever scripts recompile.
    static PlayModeResetter()
    {
        // Register a callback for play mode state changes
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // We only care about the moment you are LEAVING play mode
        // (the instant you hit Stop in the editor)
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // Try to find the SpellPoolManager that exists in the current (playing) scene
            SpellPoolManager pool = Object.FindFirstObjectByType<SpellPoolManager>();
            if (pool != null)
            {
                pool.ResetAllSpellsToBase();
                Debug.Log("[PlayModeResetter] All spell prefabs reset on exiting Play Mode.");
            }
            else
            {
                Debug.LogWarning("[PlayModeResetter] No SpellPoolManager found to reset spells.");
            }
        }
    }
}
#endif
