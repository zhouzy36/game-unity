using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class rotation : MonoBehaviour
{
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(30, 60);
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, speed * Time.deltaTime);
    }
}
