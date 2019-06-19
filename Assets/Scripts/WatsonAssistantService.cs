using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct KeyValue
{
    public string key;
    public string value;
    public KeyValueType type;
}

public enum KeyValueType
{
    Bool,
    Integer,
    String
}

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
    private float m_CanvasTimeOut = 20f;

    private float m_Timer = 0f;
    
    [SerializeField]
    Canvas m_ResultsCanvas;

    

    [SerializeField]
    List<KeyValue> initialContext;
    [SerializeField]
    List<string> greetingsPropositions = new List<string>();

    List<string> m_listPropositions = new List<string>();

    SerializableDictionary<string, object> contextSkills;

    Text m_ResultsTextUI;
    
    public string IAMUrl { set { m_IAMUrl = value; } }
    
    public string IAMKey { set { m_IAMKey = value; } }
    public string assistantId { set { m_AssistantId = value; } }
    public List<string> listPropositions { get { return m_listPropositions; } }


    private void Awake()
    {
        m_ResultsTextUI = m_ResultsCanvas.GetComponentInChildren<Text>();
    }
    /// <summary>
    /// Initialization function called on the frame when the script is enabled just before any of the Update
    /// methods is called the first time.
    /// </summary>
    protected void Start()
    {

        StartCoroutine(TokenInit());

        m_listPropositions.Clear();
        m_listPropositions.AddRange(greetingsPropositions);

        m_Timer = 0f;
        m_ResultsCanvas.enabled = false;
        
        SerializableDictionary<string, object> userDefinedDictionary = new SerializableDictionary<string, object>();
        foreach (KeyValue kv in initialContext)
        {
            SetContextValue(kv, userDefinedDictionary);
        }
        foreach (KeyValue kv in GlobalContext.Instance.context)
        {
            SetContextValue(kv, userDefinedDictionary);
        }

        SerializableDictionary<string, object> skillDictionary = new SerializableDictionary<string, object>();
        skillDictionary.Add("user_defined", userDefinedDictionary);

        contextSkills = new SerializableDictionary<string, object>();
        contextSkills.Add("main skill", skillDictionary);

    }

    private void Update()
    {
        if (m_Timer > 0)
        {
            m_Timer -= Time.deltaTime;
        } else
        {
            m_ResultsCanvas.enabled = false;
        }
        
    }

    private void OnSessionCreated(DetailedResponse<SessionResponse> response, IBMError error)
    {
        m_SessionId = response.Result.SessionId;
        Debug.Log("SessionId :" + m_SessionId);
        Invoke("KeepAlive", 240);
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

    public void SendMessageToAssistant(string text)
    {
        
        MessageInput input = new MessageInput()
        {
            Text = text,
            Options = new MessageInputOptions()
            {
                ReturnContext = true
            }
        };

        SetGlobalContext();
        
        MessageContext context = new MessageContext()
        {
            Skills = JObject.FromObject(contextSkills)
        };
        m_Assistant.Message(OnResponseReceived, m_AssistantId, m_SessionId, input, context);
    }

    private void OnResponseReceived(DetailedResponse<MessageResponse> response, IBMError error)
    {
        
        if (response.Result.Output.Generic.Count > 0)
        {
            string text = response.Result.Output.Generic[0].Text;
            m_Timer = m_CanvasTimeOut;
            m_ResultsCanvas.enabled = true;
            TextToSpeechService tts = GetComponent<TextToSpeechService>();
            if (tts != null && tts.enabled)
            {
                tts.SpeakWithRESTAPI(text);
            }

            
            if (text[0] == '<')
            {
                m_ResultsTextUI.text = "";
                XElement parsedText = XElement.Parse(text);
                foreach (XElement xelmt in parsedText.Descendants())
                {
                    m_ResultsTextUI.text = m_ResultsTextUI.text + xelmt.Value;
                }
            } else
            {
                m_ResultsTextUI.text = text;
            }

        }
        if (response.Result.Output.Generic.Count > 1)
        {
            List<DialogNodeOutputOptionsElement> listOptions = response.Result.Output.Generic[1].Options;
            m_listPropositions.Clear();
            foreach (DialogNodeOutputOptionsElement elmt in listOptions)
            {
                m_listPropositions.Add(elmt.Value.Input.Text);
            }
        }
        
        if (response.Result.Context.Skills != null)
        {
            SerializableDictionary<string, object> tempContextSkills = response.Result.Context.Skills.ToObject<SerializableDictionary<string, object>>();
            object tempSkill = null;
            (tempContextSkills as Dictionary<string, object>).TryGetValue("main skill", out tempSkill);
            tempSkill = (tempSkill as JObject).ToObject<Dictionary<string, object>>();
            object tempUserDefined = null;
            (tempSkill as Dictionary<string, object>).TryGetValue("user_defined", out tempUserDefined);
            tempUserDefined = (tempUserDefined as JObject).ToObject<Dictionary<string, object>>();
            
            Dictionary<string, object> mainSkill = new Dictionary<string, object>();
            mainSkill.Add("user_defined", tempUserDefined);
            contextSkills.Clear();
            contextSkills.Add("main skill", mainSkill);
           
        }


    }

    public void SetContext(KeyValue kv)
    {
        object tempSkill = null;
        (contextSkills as Dictionary<string, object>).TryGetValue("main skill", out tempSkill);
        Debug.Log(tempSkill);
        object tempUserDefined = null;
        (tempSkill as Dictionary<string, object>).TryGetValue("user_defined", out tempUserDefined);
        SetContextValue(kv, tempUserDefined as Dictionary<string, object>);
        
    }



    private void SetContextValue(KeyValue kv, Dictionary<string, object> userDefined)
    {
        switch (kv.type)
        {
            case KeyValueType.Bool:
                if (userDefined.ContainsKey(kv.key))
                {
                    userDefined[kv.key]  = kv.value.Equals("true");
                } else
                {
                    userDefined.Add(kv.key, kv.value.Equals("true"));
                }
                break;
            case KeyValueType.Integer:
                if (userDefined.ContainsKey(kv.key))
                {
                    userDefined[kv.key] = int.Parse(kv.value);
                }
                else
                {
                    userDefined.Add(kv.key, int.Parse(kv.value));
                }
                break;
            case KeyValueType.String:
                if (userDefined.ContainsKey(kv.key))
                {
                    userDefined[kv.key] = kv.value;
                }
                else
                {
                    userDefined.Add(kv.key, kv.value);
                }
                break;
            default:
                break;
        }
    }

    private void SetGlobalContext()
    {
        object tempSkill = null;
        (contextSkills as Dictionary<string, object>).TryGetValue("main skill", out tempSkill);
        object tempUserDefined = null;
        (tempSkill as Dictionary<string, object>).TryGetValue("user_defined", out tempUserDefined);
        foreach (KeyValue kv in GlobalContext.Instance.context)
        {
            SetContextValue(kv, tempUserDefined as Dictionary<string, object>);
        }
    }

    private void KeepAlive()
    {
        CancelInvoke();
        SendMessageToAssistant("<KEEPALIVE>");
        Invoke("KeepAlive", 240);
    }
}

