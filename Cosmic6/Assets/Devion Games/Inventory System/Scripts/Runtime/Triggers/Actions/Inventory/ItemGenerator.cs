using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemGenerator : MonoBehaviour
{
    private List<string> initialPrefabPaths = new List<string>
    {
        "Assets/Cosmic6/Prefabs12DB/삽",
        "Assets/Cosmic6/Prefabs12DB/1_Plant",
        "Assets/Cosmic6/Prefabs12DB/1_Plant",
        "Assets/Cosmic6/Prefabs12DB/1_Plant",
        "Assets/Cosmic6/Prefabs12DB/2_Plant",
        "Assets/Cosmic6/Prefabs12DB/2_Plant"
    };

    // Start is called before the first frame update
    void Start()
    {
        foreach (string path in initialPrefabPaths)
        {
            AddItemFromPrefab(path);
        }
    }

    public void AddItemFromPrefab(string prefabPath)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab != null)
        {
            ObjectManager.Instance.SpawnObject(prefab, new Vector3(), Quaternion.identity);
        }
        else
        {
            Debug.LogError($"Prefab을 찾을 수 없습니다: {prefabPath}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
