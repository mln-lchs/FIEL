using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BowQuest : MonoBehaviour
{
    private const int BULLSEYE_REQUIRED = 5;
    private int bullseyeHit;
    private bool distanceOk;
    private bool cleared;

    // Start is called before the first frame update
    void Start()
    {
        bullseyeHit = 0;
        cleared = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            distanceOk = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            distanceOk = false;
        }
    }

    public void HitBullseye()
    {
        if (distanceOk)
        {
            bullseyeHit++;
            QuestCleared();
        }
    }

    private void QuestCleared()
    {
        if (bullseyeHit >= BULLSEYE_REQUIRED)
        {
            cleared = true;
            print("BOW QUEST CLEARED");
        }
    }


}
