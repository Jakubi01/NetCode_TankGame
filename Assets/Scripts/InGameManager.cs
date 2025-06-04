using UnityEngine;
using System.Collections.Generic;

public class InGameManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    
    public static InGameManager Instance { get; private set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }
}
