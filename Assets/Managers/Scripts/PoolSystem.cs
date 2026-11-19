using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct PooledItems
{
    List<GameObject> PooledObjects;

    public PooledItems(List<GameObject> InList)
    {
        PooledObjects = new();
    }


    public bool IsEmpty()
    {
        return PooledObjects.Count == 0;
    }

    public GameObject GetObject()
    {
        if (!IsEmpty())
        {
            for (int i = 0; i < PooledObjects.Count; i++)
            {
                if (!PooledObjects[i].activeInHierarchy)
                {
                    return PooledObjects[i];
                }
            }

        }
        return null;
    }

    public void Add(GameObject InObject)
    {
        if (!HasItem(InObject))
        {
            PooledObjects.Add(InObject);
        }
    }

    public bool HasItem(GameObject InObject)
    {
        return PooledObjects.Contains(InObject);
    }

}

public class PoolSystem : MonoBehaviour
{
    private static PoolSystem Instance;


    Dictionary<Object, PooledItems> PoolList = new();

    public static PoolSystem GetInstance()
    {
        return Instance;
    }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject SpawnFromPool(GameObject Object, Transform InTransform)
    {
        GameObject Found = GetObjectFromPool(Object);
        if (Found)
        {
            Found.transform.SetParent(InTransform);
            return Found;
        }

        GameObject O = Instantiate<GameObject>(Object, InTransform);
        AddObjectToPool(Object, O);


        return O;
    }

    public GameObject SpawnFromPool(GameObject Object, Vector3 Location, Quaternion Rotation)
    {
        GameObject Found = GetObjectFromPool(Object);
        if (Found)
        {
            Found.transform.SetPositionAndRotation(Location, Rotation);
            return Found;
        }

        GameObject O = Instantiate<GameObject>(Object, Location, Rotation);
        AddObjectToPool(Object, O);

        return O;
    }

    private GameObject GetObjectFromPool(GameObject Object)
    {
        PooledItems PooledItem = new(new());
        if (!PoolList.ContainsKey(Object))
        {
            PoolList.Add(Object, PooledItem);
        }
        else
        {
            PoolList.TryGetValue(Object, out PooledItem);
        }

        if (!PooledItem.IsEmpty())
        {
            GameObject Found = PooledItem.GetObject();
            if (Found)
            {
                Found.SetActive(true);
                return Found;
            }
        }

        return null;
    }

    private void AddObjectToPool(GameObject Base, GameObject InObject)
    {
        if (!InObject) return;
        PooledItems PooledItem = new(new());
        if (!PoolList.ContainsKey(Base))
        {
            PooledItem.Add(InObject);
            PoolList.Add(Base, PooledItem);
        }
        else
        {
            PoolList.TryGetValue(Base, out PooledItem);
            PooledItem.Add(InObject);
        }

    }

    [ContextMenu("Do Something")]
    public void TEST()
    {
        Debug.Log(PoolList);
    }
}
