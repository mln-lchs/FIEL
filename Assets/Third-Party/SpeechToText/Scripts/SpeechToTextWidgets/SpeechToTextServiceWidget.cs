using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnitySpeechToText.Services;
using UnitySpeechToText.Utilities;
using Valve.VR;

namespace UnitySpeechToText.Widgets
{
    /// <summary>
    /// Widget that handles interaction with a specific speech-to-text API.
    /// </summary>
    public class SpeechToTextServiceWidget : MonoBehaviour
    {

        /// <summary>
        /// Store for SpeechToTextService property
        /// </summary>
        [SerializeField]
        SpeechToTextService m_SpeechToTextService;


        
      
        /// <summary>
        /// All the final results that have already been determined in the current recording session
        /// </summary>
        string m_PreviousFinalResults;
        /// <summary>
        /// Whether this object has stopped recording and is waiting for the last final text result of the current session.
        /// Here "last" means that there will be no more results this session, and "final" means the result is fixed
        /// ("final" is used for the sake of consistency).
        /// </summary>
        bool m_WaitingForLastFinalResultOfSession;
        /// <summary>
        /// Whether the last speech-to-text result received was a final (rather than interim) result
        /// </summary>
        bool m_LastResultWasFinal;
        
        /// <summary>
        /// Delegate for recording timeout
        /// </summary>
        Action m_OnRecordingTimeout;
        /// <summary>
        /// Delegate for receiving the last text result
        /// </summary>
        Action<string> m_OnReceivedLastResponse;


        /// <summary>
        /// The specific speech-to-text service to use
        /// </summary>
        public SpeechToTextService SpeechToTextService
        {
            set
            {
                m_SpeechToTextService = value;
                RegisterSpeechToTextServiceCallbacks();
            }
        }

        

        /// <summary>
        /// Adds a function to the recording timeout delegate.
        /// </summary>
        /// <param name="action">Function to register</param>
        public void RegisterOnRecordingTimeout(Action action)
        {
            SmartLogger.Log(DebugFlags.SpeechToTextWidgets, SpeechToTextServiceString() + " register timeout");
            m_OnRecordingTimeout += action;
        }

        /// <summary>
        /// Removes a function from the recording timeout delegate.
        /// </summary>
        /// <param name="action">Function to unregister</param>
        public void UnregisterOnRecordingTimeout(Action action)
        {
            SmartLogger.Log(DebugFlags.SpeechToTextWidgets, SpeechToTextServiceString() + " unregister timeout");
            m_OnRecordingTimeout -= action;
        }

        /// <summary>
        /// Adds a function to the received last response delegate.
        /// </summary>
        /// <param name="action">Function to register</param>
        public void RegisterOnReceivedLastResponse(Action<string> action)
        {
            m_OnReceivedLastResponse += action;
        }

        /// <summary>
        /// Removes a function from the received last response delegate.
        /// </summary>
        /// <param name="action">Function to unregister</param>
        public void UnregisterOnReceivedLastResponse(Action<string> action)
        {
            m_OnReceivedLastResponse -= action;
        }

        /// <summary>
        /// Initialization function called on the frame when the script is enabled just before any of the Update
        /// methods is called the first time.
        /// </summary>
        void Start()
        {
            RegisterSpeechToTextServiceCallbacks();
        }

        

        /// <summary>
        /// Function that is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            UnregisterSpeechToTextServiceCallbacks();
        }

       
        /// <summary>
        /// Registers callbacks with the SpeechToTextService.
        /// </summary>
        void RegisterSpeechToTextServiceCallbacks()
        {
            if (m_SpeechToTextService != null)
            {
                m_SpeechToTextService.RegisterOnError(OnSpeechToTextError);
                m_SpeechToTextService.RegisterOnTextResult(OnTextResult);
                m_SpeechToTextService.RegisterOnRecordingTimeout(OnSpeechToTextRecordingTimeout);
            }
        }

        /// <summary>
        /// Unregisters callbacks with the SpeechToTextService.
        /// </summary>
        void UnregisterSpeechToTextServiceCallbacks()
        {
            if (m_SpeechToTextService != null)
            {
                m_SpeechToTextService.UnregisterOnError(OnSpeechToTextError);
                m_SpeechToTextService.UnregisterOnTextResult(OnTextResult);
                m_SpeechToTextService.UnregisterOnRecordingTimeout(OnSpeechToTextRecordingTimeout);
            }
        }

        /// <summary>
        /// Returns a string representation of the type of speech-to-text service used by this object.
        /// </summary>
        /// <returns>String representation of the type of speech-to-text service used by this object</returns>
        public string SpeechToTextServiceString()
        {
            return m_SpeechToTextService.GetType().ToString();
        }

        
        /// <summary>
        /// Clears the current results text and tells the speech-to-text service to start recording.
        /// </summary>
        public void StartRecording()
        {
            SmartLogger.Log(DebugFlags.SpeechToTextWidgets, "Start service widget recording");
            m_WaitingForLastFinalResultOfSession = false;
            m_LastResultWasFinal = false;
            m_PreviousFinalResults = "";
            m_SpeechToTextService.StartRecording();
        }

        /// <summary>
        /// Starts waiting for the last text result and tells the speech-to-text service to stop recording.
        /// If a streaming speech-to-text service stops recording and the last result sent by it was not already final,
        /// the service is guaranteed to send a final result or error after or before some defined amount of time has passed.
        /// </summary>
        public void StopRecording()
        {
            if (m_LastResultWasFinal)
            {
                ProcessEndResults();
            }
            else
            {
                m_WaitingForLastFinalResultOfSession = true;
            }
            m_SpeechToTextService.StopRecording();
        }

        /// <summary>
        /// Function that is called when a speech-to-text result is received. If it is a final result and this widget
        /// is waiting for the last result of the session, then the widget will begin processing the end results
        /// of the session.
        /// </summary>
        /// <param name="result">The speech-to-text result</param>
        void OnTextResult(SpeechToTextResult result)
        {
            // For the purposes of comparing results, this just uses the first alternative
            m_LastResultWasFinal = result.IsFinal;
            if (result.IsFinal)
            {
                m_PreviousFinalResults += result.TextAlternatives[0].Text;
                SmartLogger.Log(DebugFlags.SpeechToTextWidgets, m_SpeechToTextService.GetType().ToString() + " final result");
                if (m_WaitingForLastFinalResultOfSession)
                {
                    m_WaitingForLastFinalResultOfSession = false;
                    ProcessEndResults();
                }
            }
        }

        /// <summary>
        /// Does any final processing necessary for the results of the last started session and then
        /// stops the widget from displaying results until the start of the next session.
        /// </summary>
        void ProcessEndResults()
        {
            SmartLogger.Log(DebugFlags.SpeechToTextWidgets, m_SpeechToTextService.GetType().ToString() + " got last response");
           
            if (m_OnReceivedLastResponse != null)
            {
                m_OnReceivedLastResponse(m_PreviousFinalResults);
            }
        }

        

        /// <summary>
        /// Function that is called when an error occurs. If this object is waiting for
        /// a last response, then this error is treated as the last "result" of the current session.
        /// </summary>
        /// <param name="text">The error text</param>
        void OnSpeechToTextError(string text)
        {
            SmartLogger.LogError(DebugFlags.SpeechToTextWidgets, SpeechToTextServiceString() + " error: " + text);
            m_PreviousFinalResults += "[Error: " + text + "] ";
            if (m_WaitingForLastFinalResultOfSession)
            {
                m_WaitingForLastFinalResultOfSession = false;
                if (m_OnReceivedLastResponse != null)
                {
                    m_OnReceivedLastResponse(m_PreviousFinalResults);
                }
            }
        }

        /// <summary>
        /// Function that is called when the recording times out.
        /// </summary>
        void OnSpeechToTextRecordingTimeout()
        {
            SmartLogger.Log(DebugFlags.SpeechToTextWidgets, SpeechToTextServiceString() + " call timeout");
            if (m_OnRecordingTimeout != null)
            {
                m_OnRecordingTimeout();
            }
        }
    }
}
