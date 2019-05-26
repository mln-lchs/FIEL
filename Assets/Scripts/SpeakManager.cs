using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySpeechToText.Widgets;
using Valve.VR;

public class SpeakManager : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean speak;

    public SpeechToTextComparisonWidget STTWidget;

    bool m_isInteractable;
    // Start is called before the first frame update
    void Start()
    {
        m_isInteractable = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (m_isInteractable)
        {
            if (speak.GetStateDown(handType))
            {
                STTWidget.OnRecordButtonClicked();
            }
            
        }

    }

    public void SetInteractable(bool interactable)
    {
        m_isInteractable = interactable;
    }
}
