using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPooling : MonoBehaviour
{
    [SerializeField]
    private int initialPoolSize = 10; // Initial size of the pool
    [SerializeField]
    private GameObject objectPrefab; // Prefab to pool
    private Queue<GameObject> inactiveObjects = new Queue<GameObject>();
    private List<GameObject> activeObjects = new List<GameObject>();

    public void Init()
    {
        if (objectPrefab == null)
        {
            Debug.LogError("Object prefab is not set!");
            return;
        }
        // Initialize the pool with inactive objects
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(false);
            inactiveObjects.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        GameObject getObject;
        if (inactiveObjects.Count > 0)
        {
            getObject = inactiveObjects.Dequeue();
            getObject.SetActive(true);
        }
        else
        {
            getObject = Instantiate(objectPrefab);
        }
        activeObjects.Add(getObject);
        return getObject;
    }

    public void ReturnObjects(GameObject returnObject)
    {
        returnObject.SetActive(false);
        activeObjects.Remove(returnObject);
        inactiveObjects.Enqueue(returnObject);
    }
}
