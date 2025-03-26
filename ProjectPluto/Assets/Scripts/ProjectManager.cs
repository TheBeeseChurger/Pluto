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
    private MenuManagerScript _menu_manager_script = null;

    private async void Start()
    {
        await SceneManager.LoadSceneAsync(SCENE_CREDITS, LoadSceneMode.Additive);
        CreditsSceneInit();
        await _opening_script.RunCredits();
        await CreditsSceneClose();
        //await SceneManager.LoadSceneAsync(SCENE_MENU, LoadSceneMode.Additive);

    }

    private void CreditsSceneInit()
    {
        _opening_script = FindFirstObjectByType<OpeningScript>();
    }

    private async Awaitable CreditsSceneClose()
    {
        _opening_script = null;

        await SceneManager.UnloadSceneAsync(SCENE_CREDITS);
    }

    private async Awaitable MenuSceneInit()
    {
        _menu_manager_script = FindFirstObjectByType<MenuManagerScript>();

        await _menu_manager_script.MenuStart();
    }
}
