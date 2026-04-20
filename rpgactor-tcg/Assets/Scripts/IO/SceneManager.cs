using System.Threading.Tasks;

public class SceneManager : SingletonBehaviour<SceneManager>
{
    public void LoadSceneImmediate(string sceneName) => LoadSceneAsync(sceneName).Forget();

    public async Task LoadSceneAsync(string sceneName)
    {
        await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }
}