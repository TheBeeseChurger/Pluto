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

    private async void Start()
    {
        await SceneManager.LoadSceneAsync(SCENE_CREDITS, LoadSceneMode.Additive);
    }
}
