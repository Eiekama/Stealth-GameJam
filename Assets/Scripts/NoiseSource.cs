using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSource : MonoBehaviour
{
    public float soundRange;
    float sourceIntensity;

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    [SerializeField] int triangleCount;

    UnityEngine.Mesh soundcone;

     void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        sourceIntensity = soundRange * soundRange;

        soundcone = MeshMethods.CreateCone(soundRange, 360f, triangleCount);

        meshFilter.mesh = soundcone;
        meshCollider.sharedMesh = soundcone;
    }
}
