using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMeshesOfChildren : MonoBehaviour
{
    [SerializeField] Material material;

    // Start is called before the first frame update
    void Start()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1]; //bc GetComponentInChildren also returns parent's component

        int i = 1;
        Matrix4x4 parentWorldToLocalMatrix = transform.worldToLocalMatrix;
        while (i < meshFilters.Length)
        {
            combine[i - 1].mesh = meshFilters[i].sharedMesh;
            combine[i - 1].transform = parentWorldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }

        GetComponent<MeshRenderer>().material = material;
        GetComponent<MeshFilter>().mesh = new Mesh();
        GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
    }
}
