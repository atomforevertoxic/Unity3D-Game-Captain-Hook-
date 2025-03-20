using System;
using UnityEngine.SceneManagement;

namespace Root
{
    public static class SceneLoader
    {
        public enum SceneID
        {
            INIT_SCENE = -1,
            MAIN_MENU = 0,
        }

        public static void Load(SceneID id, Action callback = null)
        {
            switch (id)
            {
                case SceneID.INIT_SCENE:
                    SceneManager.LoadScene(0);
                    break;
                case SceneID.MAIN_MENU:
                    SceneManager.LoadScene(1);
                    break;
                default:
                    break;
            }

            callback?.Invoke();
        }

        public static void LoadActiveScene(Action callback = null)
        {
            SceneManager.LoadScene(GetActiveSceneBuildIndex());
            callback?.Invoke();
        }

        public static int GetActiveSceneBuildIndex()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
    }
}