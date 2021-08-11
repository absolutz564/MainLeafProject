using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyConfig : MonoBehaviour
{

    public bool CanPlayMusic;
    public bool CanPlayEffects;

    //Game Timer (in seconds)
    public int InitTimerValue;
    public int GameTimerValue;

    public static DontDestroyConfig Instance;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Instance = this;

    }

}
