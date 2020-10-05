using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class revolution : MonoBehaviour
{
    public Transform center;
    private float speed;
    private Vector3 normal;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(10, 30);
        normal.y = Random.Range(-50, 0);
        normal.z = Random.Range(-50, 50);
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(center.position, normal, speed * Time.deltaTime);
    }
}
