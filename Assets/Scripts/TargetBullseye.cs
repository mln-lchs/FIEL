using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetBullseye : MonoBehaviour
{
    public UnityEvent OnBullsEyeHit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "projectile")
        {
            OnBullsEyeHit.Invoke();
        }

    }
}
