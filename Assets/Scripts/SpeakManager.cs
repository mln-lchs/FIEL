using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySpeechToText.Widgets;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SpeakManager : MonoBehaviour
{
    public Hand hand;
    public GameObject mrRobot;

    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean speak;
    
    public KeyCode keyboardShortcut;

    bool m_CanSpeak;
    bool m_CanSpawnRobot;
    bool m_CanSkip;
    SpeechToTextInteraction STTInteraction;

    // Start is called before the first frame update
    void Start()
    {
        STTInteraction = GetComponent<SpeechToTextInteraction>();
        m_CanSpeak = false;
        m_CanSpawnRobot = true;
        m_CanSkip = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (speak.GetStateDown(handType) || Input.GetKeyDown(keyboardShortcut))
        { 
            if (m_CanSpeak)
            {
                STTInteraction.OnRecordButtonClicked();
            }
            if (m_CanSpawnRobot)
            {
                //On teleporte le robot vers le joueur
                mrRobot.transform.position = Camera.main.transform.position + Camera.main.transform.forward;
            }
            if (m_CanSkip)
            {
                STTInteraction.FinishSession();
            }

        }
    }

    public void SetCanSpeak(bool canspeak)
    {
        m_CanSpeak = canspeak;
        if (canspeak)
        {
            ShowHints();
        } else
        {
            HideHints();
        }
    }

    public void SetCanSpawnRobot(bool canspawn)
    {
        m_CanSpawnRobot = canspawn;
        
    }

    public void SetCanSkip(bool canskip)
    {
        m_CanSkip = canskip;

    }

    void ShowHints()
    {
        ControllerButtonHints.ShowButtonHint(hand, hand.grabGripAction);
        ControllerButtonHints.ShowButtonHint(hand.otherHand, hand.otherHand.grabGripAction);
    }

    void HideHints()
    {
        ControllerButtonHints.HideButtonHint(hand, hand.grabGripAction);
        ControllerButtonHints.HideButtonHint(hand.otherHand, hand.otherHand.grabGripAction);
    }
}
