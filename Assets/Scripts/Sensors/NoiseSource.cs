using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSource : MonoBehaviour
{
    public float soundRange;
    public float sourceIntensity { get; private set; }

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    [SerializeField] int triangleCount;

    Mesh soundcone;

     void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        sourceIntensity = soundRange * soundRange;

        soundcone = CreateMesh.Cone(soundRange, 360f, triangleCount);

        meshFilter.mesh = soundcone;
        meshCollider.sharedMesh = soundcone;
    }
}
