using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager
{
    private static TimeManager _instance;

    public static TimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TimeManager();
            }

            return _instance;
        }
    }

    public const float FPS = 60;
    
    public const float FrameLength = 1.0f / FPS;

    private TimeManager()
    {}

    public float DeltaTime
    {
        get
        {
            return FrameLength;
        }
    }

    public float Ratio
    {
        get
        {
            return 60.0f / FPS;
        }
    }
}
