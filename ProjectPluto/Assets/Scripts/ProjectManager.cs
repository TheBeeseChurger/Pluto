using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    private const int SCENE_LOADING = 1;
    private const int SCENE_CREDITS = 2;
    private const int SCENE_MENU = 3;
    private const int SCENE_GAMEIDLE = 4;
    private const int SCENE_INTERMEDIARY = 5;
    private const int SCENE_GAME = 6;

    private OpeningScript _opening_script = null;
    private async void Start()
    {
        await SceneManager.LoadSceneAsync(SCENE_CREDITS, LoadSceneMode.Additive);
        await CreditsSceneInit();
    }

    private async Awaitable CreditsSceneInit()
    {
        _opening_script = FindFirstObjectByType<OpeningScript>();

        await _opening_script.RunCredits();

        _opening_script = null;
    }
}
