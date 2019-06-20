using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float speed;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.parent.position.y + amplitude * Mathf.Sin(speed * Time.time), transform.position.z);
    }
}
