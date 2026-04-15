using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListView : MonoBehaviour 
{
    [SerializeField] private GameObject prefab;

    public event Action<int> OnPopulate;

    public void Populate<T>(IEnumerable<T> data, Action<GameObject, T> populater, bool pool = true) {
        var index = 0;
        if (!pool) 
        {
            foreach (Transform trans in transform) 
            {
                Destroy(trans.gameObject);
            }
        }
        foreach (var datum in data) 
        {
            GameObject cellObject;
            if (index >= transform.childCount || !pool) 
            {
                cellObject = Instantiate(prefab, transform, false);
            } 
            else 
            {
                var child = transform.GetChild(index);
                cellObject = child.gameObject;
                cellObject.SetActive(true);
            }
            
            populater(cellObject, datum);
            
            index += 1;
        }

        var dataSize = index;

        // disable extra children
        for (; index < transform.childCount; index += 1) 
        {
            if (pool) 
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }
        }

        OnPopulate?.Invoke(dataSize);
    }

    public IEnumerable GetCells()
    {
        return transform;
    }

    public GameObject GetCell(int index) {
        return transform.GetChild(index).gameObject;
    }
}