using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneGameManager:Singleton<SceneGameManager>
    {
        void SwapScene(int index)
        {
            SceneManager.LoadScene(index);
        }
    }
}