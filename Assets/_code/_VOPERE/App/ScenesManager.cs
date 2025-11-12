using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Vopere.Common
{
	public class ScenesManager : MonoBehaviour
	{
		public static ScenesManager Instance { get; private set; }

        [SerializeField] List<string> scenes = new List<string>();

        string currentLoadedScene;

		void Awake()
		{
			if (Instance != null)
			{
				Debug.LogWarning("Cannot create ScenesManager");
				Destroy(gameObject);
				return;
			}

			Instance = this;
		}

		public void LoadScene(string name)
		{
			SceneManager.LoadScene(name, LoadSceneMode.Single);
		}

		public void LoadSceneAdditive(string name)
		{
			SceneManager.LoadScene(name, LoadSceneMode.Additive);
			currentLoadedScene = name;
		}

		public void UnloadScene(string name)
		{
			SceneManager.UnloadSceneAsync(name);
		}

		public void UnloadCurrentLoadedScene()
		{
			UnloadScene(currentLoadedScene);
		}

		public string GetCurrentLoadedSceneName()
		{
			return currentLoadedScene;
		}

        public void LoadSceneByName(string sceneName)
        {
			for (int i = 0;  i < scenes.Count; i++)
				if (scenes[i] ==  sceneName)
					LoadScene(scenes[i]);
        }
    }
}
