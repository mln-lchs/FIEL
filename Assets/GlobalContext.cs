using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalContext : MonoBehaviour
{
    private static GlobalContext instance = null;

    public List<KeyValue> context = new List<KeyValue>();

    // Game Instance Singleton
    public static GlobalContext Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetContext(KeyValue kv) {
        if (context.Contains(kv))
        {
            context[context.FindIndex((KeyValue elmt) => kv.key.Equals(elmt.key))] = kv;
        } else
        {
            context.Add(kv);
        }
    }
    
}
