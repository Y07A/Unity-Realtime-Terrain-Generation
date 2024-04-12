using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileLod : MonoBehaviour
{
    [System.Serializable]
    public struct Lod{
        public Renderer renderer;
        public float range;
    }


    public Lod[] lods;

    [Header("Props")]
    public GameObject props;
    public float propsRange = 2.5f;

    private void Start()
    {
        StartCoroutine(Check());
    }

    private IEnumerator Check()
    {
        while (true){
            Vector3 camPosition = FloatingOrigin.Apply(Camera.main.transform.position);
            camPosition.y = 0f;
            float distance = Vector3.Distance(FloatingOrigin.Apply(transform.position), camPosition);
            float rangeMin = 0f;
            float scale = transform.localScale.z;
            foreach(Lod lod in lods){
                lod.renderer.enabled = (distance < lod.range*scale && distance > rangeMin);
                rangeMin = lod.range*scale;
            }
            EnableProps(distance < propsRange*scale);
            yield return new WaitForSeconds(.3f);
        }
    }

    private void EnableProps(bool b)
    {
        if (props.activeSelf == b)
            return;
        props.SetActive(b);
    }
}
