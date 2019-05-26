using System;
using System.Collections;
using IBM.Cloud.SDK;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class WatsonAssistantService : MonoBehaviour
    {
    /// <summary>
    /// Watson assistant functionality
    /// </summary>
    AssistantService m_Assistant;

    Credentials credentials;
    string m_SessionId;

    /// <summary>
    /// Store for IAMUrl property
    /// </summary>
    [SerializeField]
    string m_IAMUrl;
    /// <summary>
    /// Store for IAMKey property
    /// </summary>
    [SerializeField]
    string m_IAMKey;

    [SerializeField]
    string m_AssistantId;

    [SerializeField]
    Text m_ResultsTextUI;

    public string IAMUrl { set { m_IAMUrl = value; } }
    
    public string IAMKey { set { m_IAMKey = value; } }
    public string assistantId { set { m_AssistantId = value; } }

    /// <summary>
    /// Initialization function called on the frame when the script is enabled just before any of the Update
    /// methods is called the first time.
    /// </summary>
    protected void Start()
    {
        StartCoroutine(TokenInit());
    }

    private void OnSessionCreated(DetailedResponse<SessionResponse> response, IBMError error)
    {
        m_SessionId = response.Result.SessionId;
        Debug.Log("SessionId :" + m_SessionId);
    }

    IEnumerator TokenInit()
    {
        //  Create IAM token options and supply the apikey. IamUrl is the URL used to get the 
        //  authorization token using the IamApiKey. It defaults to https://iam.cloud.ibm.com/identity/token
        TokenOptions iamTokenOptions = new TokenOptions()
        {
            IamApiKey = m_IAMKey,
            IamUrl = m_IAMUrl
        };

        //  Create credentials using the IAM token options
        credentials = new Credentials(iamTokenOptions, "https://gateway.watsonplatform.net/assistant/api");
        while (!credentials.HasIamTokenData())
            yield return null;

        m_Assistant = new AssistantService(
            versionDate: "2019-05-23",
            credentials: credentials
        );
        m_Assistant.CreateSession(OnSessionCreated, m_AssistantId);
    }

    public void SendMessage(string text)
    {
        MessageInput input = new MessageInput()
        {
            Text = text
        };
        m_Assistant.Message(OnResponseReceived, m_AssistantId, m_SessionId, input);
    }

    private void OnResponseReceived(DetailedResponse<MessageResponse> response, IBMError error)
    {
        if (response.Result.Output.Generic.Count > 0)
        {
            m_ResultsTextUI.text = response.Result.Output.Generic[0].Text;
        }
    }
}

