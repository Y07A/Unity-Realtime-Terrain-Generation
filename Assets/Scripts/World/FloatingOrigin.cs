using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingOrigin : MonoBehaviour
{
    public float threshold = 500f;
    private static Vector3 offset;
    static FloatingOrigin instance;

    [Header("Debug")]
    public Vector3 worldPositionDebug;

    private void Start()
    {
        instance = this;
        StartCoroutine(Check());
    }

    private void Update()
    {
        worldPositionDebug = Apply(transform.position);
    }

    private IEnumerator Check()
    {
        yield return new WaitForSeconds(.1f);
        while (true){
            CheckPosition();
            yield return new WaitForSeconds(5f);
        }
    }

    private void CheckPosition()
    {
        Vector3 position = transform.position;
        if (position.magnitude > threshold){
            addPosition(position);
            applyPosition(position);
        }
    }

    private void addPosition(Vector3 pos)
    {
        transform.position = new Vector3(0f, 0f, 0f);
        offset += pos;
    }

    private void applyPosition(Vector3 pos)
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Root"))
        {
            g.transform.position -= pos;
        }
    }

    public static void UpdatePosition()
    {
        instance.CheckPosition();
    }

    public static Vector3 Apply(Vector3 relativePos)
    {
        return offset+relativePos;
    }

    public static Vector3 Invert(Vector3 worldPos)
    {
        return worldPos-offset;
    }
}
