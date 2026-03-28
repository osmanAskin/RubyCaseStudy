using System;
using UnityEngine;

public class JsonSaveSystem : ISaveSystem
{
    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public bool Save<T>(string key, T data)
    {
        try
        {
            var json = JsonUtility.ToJson(data);
            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            PlayerPrefs.SetString(key, json);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return false;
        }
    }

    public bool TryGet<T>(string key, out T data)
    {
        try
        {
            T value = JsonUtility.FromJson<T>(PlayerPrefs.GetString(key));
            if (value == null)
            {
                data = default;
                return false;
            }
            data = value;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            data = default(T);
            return false;
        }
    }
}
