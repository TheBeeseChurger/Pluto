using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    [SerializeReference] private Scene credits_scene;

    private async void Start()
    {
        await SceneManager.LoadSceneAsync(credits_scene.name, LoadSceneMode.Additive);
    }
}
