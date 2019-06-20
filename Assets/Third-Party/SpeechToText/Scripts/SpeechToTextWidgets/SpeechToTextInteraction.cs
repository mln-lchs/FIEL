using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnitySpeechToText.Utilities;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace UnitySpeechToText.Widgets
{
    /// <summary>
    /// Widget that handles the side-by-side comparison of different speech-to-text services.
    /// </summary>
    public class SpeechToTextInteraction : MonoBehaviour
    {
        [Header("UI Elements")]
        /// <summary>
        /// Text to display on the record button when recording
        /// </summary>
        [SerializeField]
        string m_RecordingText;
        /// <summary>
        /// Text to display on the record button when not recording
        /// </summary>
        [SerializeField]
        string m_NotRecordingText;
        /// <summary>
        /// Text to display on the record button when waiting for responses
        /// </summary>
        [SerializeField]
        string m_WaitingForResponsesText;
        /// <summary>
        /// Text to display on the record button when feedbacks are displayed
        /// </summary>
        [SerializeField]
        string m_FeedbacksText;
        /// <summary>
        /// Image to display when mic is enabled
        /// </summary>
        [SerializeField]
        Sprite m_EnabledMicImage;
        /// <summary>
        /// Image to display when mic is disabled
        /// </summary>
        [SerializeField]
        Sprite m_DisabledMicImage;
        /// <summary>
        /// Material of the mic when disabled
        /// </summary>
        [SerializeField]
        Material m_DisabledMicMaterial;
        /// <summary>
        /// Material of the mic when disabled
        /// </summary>
        [SerializeField]
        Material m_EnabledMicMaterial;
        /// <summary>
        /// Material of the record button when recording
        /// </summary>
        [SerializeField]
        Material m_RecordingButtonMaterial;
        /// <summary>
        /// Material of the record button when not recording
        /// </summary>
        [SerializeField]
        Material m_NotRecordingButtonMaterial;
        /// <summary>
        /// Canvas of the left hand
        /// </summary>
        [SerializeField]
        Canvas m_LeftHandUI;

        [Header("Fallbacks Hands UI")]
        /// <summary>
        /// Canvas of the fallback left hand
        /// </summary>
        [SerializeField]
        Canvas m_FallbackLeftHandUI;

        [Header("Settings")]
        /// <summary>
        /// Number of seconds to wait for feedback
        /// </summary>
        [SerializeField]
        float m_FeedbackTimeoutInSeconds = 8f;
        /// <summary>
        /// Number of seconds to wait for all responses after recording
        /// </summary>
        [SerializeField]
        float m_ResponsesTimeoutInSeconds = 8f;
        /// <summary>
        /// Maximum distance of interaction
        /// </summary>
        [SerializeField]
        float m_RayDistance = 3f;
        /// <summary>
        /// Time without looking at the npc before disabling speech
        /// </summary>
        [SerializeField]
        float m_LookAwayTimer = 10f;

        /// <summary>
        /// List of sounds
        /// </summary>
        [SerializeField]
        AudioClip[] clips;


        /// <summary>
        /// Camera of the player for raycasting
        /// </summary>
        Camera m_Camera;
        /// <summary>
        /// Manager of the controls for speaking
        /// </summary>
        SpeakManager m_SpeakManager;
        /// <summary>
        /// Manager of the propositions
        /// </summary>
        PropositionsManager m_PropositionsManager;
        /// <summary>
        /// Selected assistant for speech
        /// </summary>
        WatsonAssistantService m_WatsonAssistant;
        /// <summary>
        /// Store for SpeechToTextServiceWidgets property
        /// </summary>
        SpeechToTextServiceWidget m_SpeechToTextServiceWidget;
        /// <summary>
        /// Transform of the selected assistant for distance
        /// </summary>
        Transform m_NPCTransform;
        /// <summary>
        /// Timer for selecting npc
        /// </summary>
        float m_NPCTimer;

        /// <summary>
        /// Text UI for the left hand
        /// </summary>
        Text m_LeftHandRecordInfoText;
        /// <summary>
        /// Image background for the left hand
        /// </summary>
        Image m_LeftHandTextBackground;
        /// <summary>
        /// Image microphone for the left hand
        /// </summary>
        Image m_LeftHandMicImage;

        /// <summary>
        /// Whether the application is currently in a speech-to-text session
        /// </summary>
        bool m_IsCurrentlyInSpeechToTextSession;
        /// <summary>
        /// Whether the application is currently recording audio
        /// </summary>
        bool m_IsRecording;

        // Audio source
        AudioSource audioSource;



        /// <summary>
        /// Speech-to-text service widget 
        /// </summary>
        public SpeechToTextServiceWidget SpeechToTextServiceWidgets
        {
            set
            {
                m_SpeechToTextServiceWidget = value;
                RegisterSpeechToTextServiceWidgetsCallbacks();
            }
        }

        

        /// <summary>
        /// Initialization function called on the frame when the script is enabled just before any of the Update
        /// methods is called the first time.
        /// </summary>
        void Start()
        {
            // Initialize timer
            m_NPCTimer = m_LookAwayTimer;

            // Npc selection initialization
            m_Camera = Camera.main;
            m_NPCTransform = m_Camera.transform;

            

            // Get Components
            m_SpeakManager = GetComponent<SpeakManager>();
            m_SpeechToTextServiceWidget = GetComponent<SpeechToTextServiceWidget>();
            m_PropositionsManager = GetComponent<PropositionsManager>();

            // No-VR fallbacks
            if (SteamVR.instance == null)
            {
                m_LeftHandUI = m_FallbackLeftHandUI;
            }

            SetCanvasChildComponents();
            DisableSpeechUI();
            RegisterSpeechToTextServiceWidgetsCallbacks();

            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Function that is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            float dist = Vector2.Distance(new Vector2(m_Camera.transform.position.x, m_Camera.transform.position.z), new Vector2(m_NPCTransform.position.x, m_NPCTransform.position.z));
            if (m_NPCTimer > 0 && dist <= m_RayDistance)
            {
                m_NPCTimer -= Time.deltaTime;
            }
            else
            {
                if (!m_IsCurrentlyInSpeechToTextSession && m_WatsonAssistant != null)
                {
                    m_WatsonAssistant.DeactivatePrism();
                    m_WatsonAssistant = null;
                    DisableSpeechUI();
                }
            }

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.TransformDirection(Vector3.forward), out hit, m_RayDistance, layerMask))
            {

                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                if (hit.collider.gameObject.layer == 9)
                {
                    WatsonAssistantService previousAssistant = m_WatsonAssistant;
                    m_WatsonAssistant = hit.collider.GetComponentInChildren<WatsonAssistantService>();
                    m_WatsonAssistant.ActivatePrism();
                    if (previousAssistant != m_WatsonAssistant)
                    {
                        m_NPCTransform = hit.collider.transform;
                        m_PropositionsManager.SetPropositions(m_WatsonAssistant.listPropositions);
                        EnableSpeechUI();
                    }
                    m_SpeakManager.SetCanSpawnRobot(false);
                    m_NPCTimer = m_LookAwayTimer;
                } else
                {
                    m_SpeakManager.SetCanSpawnRobot(true);
                }
            } else
            {
                m_SpeakManager.SetCanSpawnRobot(true);
            }
        }

        void EnableSpeechUI()
        {
            m_SpeakManager.SetCanSpeak(true);
            m_SpeakManager.SetCanSkip(false);
            m_LeftHandUI.enabled = true;
        }

        void DisableSpeechUI()
        {
            m_SpeakManager.SetCanSpeak(false);
            m_SpeakManager.SetCanSkip(false);
            m_LeftHandUI.enabled = false;
        }

        /// <summary>
        /// Function that is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            UnregisterSpeechToTextServiceWidgetsCallbacks();
        }

        /// <summary>
        /// Finds child components for the record button and assigns them to the appropriate member variables.
        /// </summary>
        void SetCanvasChildComponents()
        {
            if (m_LeftHandUI != null)
            {
                LeftUI leftUI = m_LeftHandUI.GetComponentInChildren<LeftUI>();
                m_LeftHandTextBackground = leftUI.m_Background;
                m_LeftHandRecordInfoText = leftUI.m_RecordInfoText; ;
                m_LeftHandMicImage = leftUI.m_MicImage;

                m_LeftHandRecordInfoText.text = m_NotRecordingText;
                m_LeftHandMicImage.sprite = m_EnabledMicImage;

            }
        }

        /// <summary>
        /// Registers callbacks with each SpeechToTextServiceWidget.
        /// </summary>
        void RegisterSpeechToTextServiceWidgetsCallbacks()
        {
            if (m_SpeechToTextServiceWidget != null)
            {
                SmartLogger.Log(DebugFlags.SpeechToTextWidgets, "register service widget callbacks");
                m_SpeechToTextServiceWidget.RegisterOnRecordingTimeout(OnRecordTimeout);
                m_SpeechToTextServiceWidget.RegisterOnReceivedLastResponse(OnSpeechToTextReceivedLastResponse);
            }
        }

        /// <summary>
        /// Unregisters callbacks with each SpeechToTextServiceWidget.
        /// </summary>
        void UnregisterSpeechToTextServiceWidgetsCallbacks()
        {
            if (m_SpeechToTextServiceWidget != null)
            {
                SmartLogger.Log(DebugFlags.SpeechToTextWidgets, "unregister service widget callbacks");
                m_SpeechToTextServiceWidget.UnregisterOnRecordingTimeout(OnRecordTimeout);
                m_SpeechToTextServiceWidget.UnregisterOnReceivedLastResponse(OnSpeechToTextReceivedLastResponse);
                
            }
        }

        /// <summary>
        /// Function that is called when the record button is clicked.
        /// </summary>
        public void OnRecordButtonClicked()
        {
            if (m_IsRecording)
            {
                audioSource.clip = clips[1];
                audioSource.Play();
                StopRecording();
            }
            else
            {
                audioSource.clip = clips[0];
                audioSource.Play();
                StartRecording();
            }
        }

        /// <summary>
        /// Function that is called when audio recording times out.
        /// </summary>
        void OnRecordTimeout()
        {
            StopRecording();
        }

        /// <summary>
        /// Function that is called when the given SpeechToTextServiceWidget has gotten its last response. If there are no waiting
        /// SpeechToTextServiceWidgets left, then this function will wrap-up the current comparison session.
        /// </summary>
        /// <param name="serviceWidget">The speech-to-text service widget that received a last response</param>
        void OnSpeechToTextReceivedLastResponse(string finalResult)
        {
            SmartLogger.Log(DebugFlags.SpeechToTextWidgets, "Final Result " + finalResult);
            string result = m_PropositionsManager.TestSelectedProposition(finalResult);
            if (!result.Equals(""))
            {
                m_WatsonAssistant.SendMessageToAssistant(result);
            }
            WaitForFeedbacks();
        }

        /// <summary>
        /// Starts recording audio for each speech-to-text service widget if not already recording.
        /// </summary>
        public void StartRecording()
        {
            if (!m_IsRecording)
            {
                SmartLogger.Log(DebugFlags.SpeechToTextWidgets, "Start comparison recording");
                m_IsCurrentlyInSpeechToTextSession = true;
                m_IsRecording = true;
                m_LeftHandRecordInfoText.text = m_RecordingText;
                m_LeftHandTextBackground.material = m_RecordingButtonMaterial;
                SmartLogger.Log(DebugFlags.SpeechToTextWidgets, "tell service widget to start recording");
                m_SpeechToTextServiceWidget.StartRecording();
            }
        }

        /// <summary>
        /// Stops recording audio for each speech-to-text service widget if already recording. Also schedules a wrap-up of the
        /// current comparison session to happen after the responses timeout.
        /// </summary>
        public void StopRecording()
        {
            if (m_IsRecording)
            {
                m_IsRecording = false;

                // Disable all UI interaction until all responses have been received or after the specified timeout.
                DisableSpeaking();
                m_LeftHandTextBackground.material = m_NotRecordingButtonMaterial;
                Invoke("FinishSession", m_ResponsesTimeoutInSeconds);
                m_SpeechToTextServiceWidget.StopRecording();
            }
        }

        /// <summary>
        /// Wraps up the current speech-to-text comparison session by enabling all UI interaction.
        /// </summary>
        public void FinishSession()
        {
            CancelInvoke();
            m_SpeakManager.SetCanSkip(false);
            m_PropositionsManager.SetPropositions(m_WatsonAssistant.listPropositions);
            if (m_IsCurrentlyInSpeechToTextSession)
            {
                m_IsCurrentlyInSpeechToTextSession = false;
                EnableSpeaking();
            }
        }

        /// <summary>
        /// Enables speak.
        /// </summary>
        void EnableSpeaking()
        {
            m_SpeakManager.SetCanSpeak(true);
            m_SpeakManager.SetCanSkip(false);
            m_LeftHandRecordInfoText.text = m_NotRecordingText;
            m_LeftHandMicImage.sprite = m_EnabledMicImage;
            m_LeftHandMicImage.material = m_EnabledMicMaterial;
        }

        /// <summary>
        /// Disables speak.
        /// </summary>
        void DisableSpeaking()
        {
            m_SpeakManager.SetCanSpeak(false);
            m_SpeakManager.SetCanSkip(false);
            m_LeftHandRecordInfoText.text = m_WaitingForResponsesText;
            m_LeftHandMicImage.sprite = m_DisabledMicImage;
            m_LeftHandMicImage.material = m_DisabledMicMaterial;

        }

        void WaitForFeedbacks()
        {
            CancelInvoke("FinishSession");

            m_SpeakManager.SetCanSkip(true);
            m_SpeakManager.SetCanSpeak(false);
            m_LeftHandRecordInfoText.text = m_FeedbacksText;

            Invoke("FinishSession", m_FeedbackTimeoutInSeconds);
        }

    }
}
