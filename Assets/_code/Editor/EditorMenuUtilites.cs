using UnityEditor;
using UnityEngine;

namespace MegaGame.Editor
{
    public class EditorMenuUtilites : EditorWindow
    {
        static void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Utilites/Удалить Player Prefs")]
        static void DeletePlayerPrefsButton()
        {
            DeletePlayerPrefs();
        }

        /*
        [MenuItem("Utilites/Добавить конструктор части тела")]
        static void AddInfoConstructorButton()
        {
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                if (selectedObject.GetComponent<InfoConstructor>() == null)
                {
                    selectedObject.AddComponent<InfoConstructor>();
                    EditorUtility.SetDirty(selectedObject);
                }
            }
        }

        [MenuItem("Utilites/UI Audio/Добавить UIButtonClickAudio")]
        static void AddUIButtonClickAudio()
        {
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                if (selectedObject.GetComponent<UIButtonClickAudio>() == null)
                {
                    selectedObject.AddComponent<UIButtonClickAudio>();
                    EditorUtility.SetDirty(selectedObject);
                }
            }
        }

        [MenuItem("Utilites/UI Audio/Добавить UIToggleClickAudio")]
        static void AddUIToggleClickAudio()
        {
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                if (selectedObject.GetComponent<UIToggleClickAudio>() == null)
                {
                    selectedObject.AddComponent<UIToggleClickAudio>();
                    EditorUtility.SetDirty(selectedObject);
                }
            }
        }

        [MenuItem("Utilites/UI Audio/Добавить UISliderAudio")]
        static void AddUISliderAudio()
        {
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                if (selectedObject.GetComponent<UISliderAudio>() == null)
                {
                    selectedObject.AddComponent<UISliderAudio>();
                    EditorUtility.SetDirty(selectedObject);
                }
            }
        }
        */
    }
}
