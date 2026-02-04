using UnityEngine;

namespace Vopere.Common
{
    public class DataSaveLoad : MonoBehaviour
    {
        public static DataSaveLoad Instance { get; private set; }

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create DataSaveLoad");
                Destroy(gameObject);
            }

            Instance = this;
        }

        public void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        public void Save(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public void Save(string key, short value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public void Save(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public void Save(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public int GetSavedInt(string key)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetInt(key);
            else
                return -1;
        }

        public short GetSavedShort(string key)
        {
            if (PlayerPrefs.HasKey(key))
                if (PlayerPrefs.GetInt(key) > short.MaxValue || PlayerPrefs.GetInt(key) < short.MinValue)
                    return -1;
                else
                    return (short)PlayerPrefs.GetInt(key);
            else
                return -1;
        }

        public float GetSavedFloat(string key)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetFloat(key);
            else
                return -1;
        }

        public string GetSavedString(string key)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetString(key);
            else
                return "";
        }
    }
}
