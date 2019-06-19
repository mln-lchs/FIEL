using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffectsOnEnable : MonoBehaviour
{
    private ParticleSystem m_ParticleSystem;
    // Start is called before the first frame update
    void Start()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        m_ParticleSystem.Play();
    }
}
