using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDefenseManager : MonoBehaviour
{
    public static BaseDefenseManager Instance { get; private set; }
    void Start()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
