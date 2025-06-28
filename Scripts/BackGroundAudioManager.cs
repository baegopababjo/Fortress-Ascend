using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundAudioManager : MonoBehaviour
{
    public bool DonsDestroy = true;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
