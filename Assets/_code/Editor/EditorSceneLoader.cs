using UnityEditor;
using UnityEditor.SceneManagement;

namespace MegaGame.Editor
{
	public class EditorSceneLoader : EditorWindow
	{
		static void LoadScene(string sceneName)
		{
			string path = "Assets/Scenes/";

			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene(path + sceneName + ".unity", OpenSceneMode.Single);
			}
		}

		[MenuItem("Mega Game/Загрузить сцену/Main")]
		static void LoadSceneMainMenu()
		{
			LoadScene("Main");
		}
	}
}
