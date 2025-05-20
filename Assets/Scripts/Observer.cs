using System.Runtime.InteropServices;
using System;
using UnityEngine;

public class Observer
{
    public static Action<float> JumpSync = delegate { };
    public static Action<int> Score = delegate { };
    public static Action<float> Distance = delegate { };
}
