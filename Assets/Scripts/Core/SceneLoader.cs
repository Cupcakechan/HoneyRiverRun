using UnityEngine.SceneManagement;

/// One place for all scene changes. Scene names are constants so we never hard-code strings elsewhere.
public static class SceneLoader
{
    public const string MainMenu = "MainMenu";
    public const string Gameplay = "Gameplay";

    public static void Load(string sceneName) => SceneManager.LoadScene(sceneName);
    public static void LoadGameplay() => Load(Gameplay);
    public static void LoadMainMenu() => Load(MainMenu);
}