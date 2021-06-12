using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneObjects : MonoBehaviour
{
    public GameObject model;

    // Start is called before the first frame update
    void Start()
    {
        model.SetActive(false);
    }

    public void showModel()
    {
        model.SetActive(true);
    }
}
