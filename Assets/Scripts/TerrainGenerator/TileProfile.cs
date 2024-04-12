using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProfile : MonoBehaviour
{
    [System.Serializable]
    public struct tileMesh{
        public MeshFilter filter;
        public int resolution;
    }

    public float size = 200;
    public tileMesh[] tiles;
    public TileCollider collider;
    public Transform propsHandle;

    private void Start()
    {
        gameObject.tag = "Root";
    }

    public void ChangeMaterial(Material mat)
    {
        foreach(tileMesh tm in tiles){
            tm.filter.GetComponent<MeshRenderer>().material = mat;
        }
    }
}
