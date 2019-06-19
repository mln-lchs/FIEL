using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BowQuest : MonoBehaviour
{
    [SerializeField] int BULLSEYE_REQUIRED = 2;
    private int bullseyeHit;
    private bool distanceOk = false;
    private bool cleared;
    public AudioSource audioSource;
    public AudioClip[] clips;

    // Start is called before the first frame update
    void Start()
    {
        bullseyeHit = 0;
        cleared = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //8 PlayerLayer
        if (other.gameObject.layer == 8)
        {
            distanceOk = true;
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            distanceOk = false;
        }
    }

    public void HitBullseye()
    {
        
        if (distanceOk)
        {
            Debug.Log("coucou");
            audioSource.clip = clips[0];
            audioSource.Play();
            bullseyeHit++;
            QuestCleared();
        }
        else
        {
            Debug.Log("distance not ok");
        }
    }

    private void QuestCleared()
    {
        if (bullseyeHit >= BULLSEYE_REQUIRED && !cleared)
        {
            cleared = true;
            print("BOW QUEST CLEARED");
            audioSource.clip = clips[1];
            audioSource.Play();

            KeyValue kv;
            kv.type = KeyValueType.Bool;
            kv.key = "quest_drunkman";
            kv.value = "true";
            kv.value = "true";

            GlobalContext.Instance.SetContext(kv);
        }
    }


}
