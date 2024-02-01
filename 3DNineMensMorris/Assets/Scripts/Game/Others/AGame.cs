using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AGame : MonoBehaviour
{
    protected abstract IEnumerator Play();
    protected void ChangeColor(Color newColor, GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = newColor;
        }
        else
        {
            Debug.LogWarning("A GameObject nem rendelkezik Renderer komponenssel, így nem lehet megváltoztatni a színét.");
        }
    }
}
