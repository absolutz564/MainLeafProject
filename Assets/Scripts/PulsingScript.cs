using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Light))]
public class PulsingScript : MonoBehaviour
{
    public float duration;
    public Light lt;
    // Start is called before the first frame update
    void Start()
    {
        lt = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        float phi = (Time.time / duration) * Mathf.PI;
        float amplitude = Mathf.Cos(phi) * 0.4f + 0.5f;
        lt.intensity = amplitude;
    }
}
