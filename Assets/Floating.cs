using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float speed;
    private float y0;

    // Start is called before the first frame update
    void Start()
    {
        y0 = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, y0 + amplitude * Mathf.Sin(speed * Time.time), transform.position.z);
    }
}
