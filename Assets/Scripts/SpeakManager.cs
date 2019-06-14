using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySpeechToText.Widgets;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SpeakManager : MonoBehaviour
{
    public Hand hand;
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean speak;
    
    public KeyCode keyboardShortcut;

    bool m_isInteractable;
    SpeechToTextInteraction STTInteraction;
    // Start is called before the first frame update
    void Start()
    {
        m_isInteractable = true;
        STTInteraction = GetComponent<SpeechToTextInteraction>();
    }

    // Update is called once per frame
    void Update()
    {

        if (m_isInteractable)
        {
            if (speak.GetStateDown(handType) || Input.GetKeyDown(keyboardShortcut))
            {
                STTInteraction.OnRecordButtonClicked();
            }
            
        }

    }

    public void SetInteractable(bool interactable)
    {
        m_isInteractable = interactable;
        if (interactable)
        {
            ShowHints();
        } else
        {
            HideHints();
        }
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
