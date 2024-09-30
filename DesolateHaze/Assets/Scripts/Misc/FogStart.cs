using System.Text.RegularExpressions;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class FogStart : MonoBehaviour
{
    void Start()
    {
        Volume fog_V = GetComponent<Volume>();
        VolumeProfile profile = fog_V.profile;
        if (!profile.TryGet<Fog>(out var fog))
        {
            fog = profile.Add<Fog>(false);
        }
        fog.active = true;

    }
}

