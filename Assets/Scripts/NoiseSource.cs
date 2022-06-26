using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class NoiseSource : MonoBehaviour
{
    public float soundRange;
    public float SourceIntensity { get; private set; }

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    [SerializeField] int triangleCount;

    Mesh soundcone;

     void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        SourceIntensity = soundRange * soundRange;

        soundcone = CreateMesh.Cone(soundRange, 360f, triangleCount);

        meshFilter.mesh = soundcone;
        meshCollider.sharedMesh = soundcone;
    }
}
