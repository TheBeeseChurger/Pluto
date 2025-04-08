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
    private LoadingManager _loading_manager = null;

    [SerializeField] private DataScript d_prefab;
    [SerializeField] private GameObject a_prefab;

    static GameObject audio_head;
    static DataScript data;

    [SerializeField] private bool Skip_Credits;

    private async void Start()
    {
        await InitStatics();

        if (!Skip_Credits)
        {
            await SceneManager.LoadSceneAsync(SCENE_CREDITS, LoadSceneMode.Additive);
            CreditsSceneInit();
            await _opening_script.RunCredits(audio_head);
            await CreditsSceneClose();
        }

        SceneManager.LoadScene(SCENE_LOADING, LoadSceneMode.Additive);
        await LoadingSceneInit();

        await Awaitable.BackgroundThreadAsync();
        await SceneManager.LoadSceneAsync(SCENE_MENU, LoadSceneMode.Additive);
        await MenuSceneInit();

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

        await _menu_manager_script.MenuStart(data, audio_head);
    }

    private async Awaitable LoadingSceneInit()
    {
        _loading_manager = FindFirstObjectByType<LoadingManager>();

        
    }

    private async Awaitable InitStatics()
    {
        await InstantiateAsync(d_prefab);
        await InstantiateAsync(a_prefab);

        await Awaitable.MainThreadAsync();
        data = GameObject.FindWithTag("data").GetComponent<DataScript>();
        audio_head = GameObject.FindWithTag("audio");
    }
}
