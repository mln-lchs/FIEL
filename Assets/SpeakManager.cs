using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySpeechToText.Widgets;
using Valve.VR;

public class SpeakManager : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean speakToRobot;

    public SpeechToTextComparisonWidget STTServiceRobot;

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
            if (speakToRobot.GetStateDown(handType))
            {
                STTServiceRobot.OnRecordButtonClicked();
            }
            
        }

    }

    public void SetInteractable(bool interactable)
    {
        m_isInteractable = interactable;
    }
}
