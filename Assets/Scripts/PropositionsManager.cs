using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnitySpeechToText.Utilities;
using Valve.VR;

public class PropositionsManager : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean leftArrow;
    public SteamVR_Action_Boolean rightArrow;
    /// <summary>
    /// List of greetings propositions
    /// </summary>
    [SerializeField]
    List<string> m_GreetingsPropositions = new List<string>();
    /// <summary>
    /// Display Text when the player wants to say something else
    /// </summary>
    [SerializeField]
    string m_OtherText;
    /// <summary>
    /// Minimum accuracy for validation
    /// </summary>
    [SerializeField]
    [Range(0,100)]
    int m_MinAccuracy;
    /// <summary>
    /// STT error accuracy bonus
    /// </summary>
    [SerializeField]
    [Range(0, 100)]
    int m_AccuracyBonus;
    /// <summary>
    /// Correct accuracy percentage
    /// </summary>
    [SerializeField]
    [Range(0, 100)]
    int m_CorrectAccuracy;
    /// <summary>
    /// Correct Color
    /// </summary>
    [SerializeField]
    Color m_CorrectColor;
    /// <summary>
    /// Not Bad Color
    /// </summary>
    [SerializeField]
    Color m_NotBadColor;
    /// <summary>
    /// Error Color
    /// </summary>
    [SerializeField]
    Color m_ErrorColor;
    /// <summary>
    /// UI references
    /// </summary>
    [SerializeField]
    LeftUI m_LeftUI;

    /// <summary>
    /// Fallback UI references
    /// </summary>
    [SerializeField]
    LeftUI m_FallbackLeftUI;

    /// <summary>
    /// List of sounds
    /// </summary>
    [SerializeField]
    AudioClip[] clips;

    /// <summary>
    /// List of propositions
    /// </summary>
    List<string> m_Propositions = new List<string>();
    /// <summary>
    /// Text UI for propositions
    /// </summary>
    Text m_PropositionText;
    /// <summary>
    /// Text UI for accuracy
    /// </summary>
    Text m_AccuracyText;
    /// <summary>
    /// Image UI for left arrow
    /// </summary>
    Image m_LeftArrowImage;
    /// <summary>
    /// Image UI for right arrow
    /// </summary>
    Image m_RightArrowImage;
    /// <summary>
    /// Index of the selected proposition
    /// </summary>
    int m_Index = 0;
    /// <summary>
    /// if you can change propositions or not
    /// </summary>
    bool m_IsInteractable;

    // <summary>
    /// Set of leading characters for words to ignore when computing accuracy
    /// </summary>
    HashSet<char> m_LeadingCharsForSpecialWords = new HashSet<char> { '%' };
    /// <summary>
    /// Set of surrounding characters for text to ignore when computing accuracy, which includes brackets
    /// by default to account for instructions in speech such as "[pause]"
    /// </summary>
    Dictionary<char, char> m_SurroundingCharsForSpecialText = new Dictionary<char, char> { { '[', ']' } };

    // Audio source
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // No-VR fallbacks
        if (SteamVR.instance == null)
        {
            m_LeftUI = m_FallbackLeftUI;
        }
        m_PropositionText = m_LeftUI.m_PropositionText;
        m_AccuracyText = m_LeftUI.m_AccuracyText;
        m_LeftArrowImage = m_LeftUI.m_LeftArrow;
        m_RightArrowImage = m_LeftUI.m_RightArrow;
        m_IsInteractable = true;
        // Init propositions
        Reset();

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsInteractable)
        {
            if (rightArrow.GetStateDown(handType) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                audioSource.clip = clips[0];
                audioSource.Play();
                m_Index++;
                if (m_Index > m_Propositions.Count) m_Index = 0;
                SetPropositionText();
            }
            if (leftArrow.GetStateDown(handType) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                audioSource.clip = clips[0];
                audioSource.Play();
                m_Index--;
                if (m_Index < 0) m_Index = m_Propositions.Count;
                SetPropositionText();
            }
        }
    }

    void SetPropositionText()
    {
        if (m_Index == m_Propositions.Count)
        {
            m_PropositionText.text = m_OtherText;
        } else
        {
            m_PropositionText.text = m_Propositions[m_Index];
        }
    }

    /// <summary>
    /// Computes the accuracy (percentage) of the end text results in comparison to the given phrase, by using 
    /// the Levenshtein Distance between the two strings, and displays this percentage in the results text UI.
    /// </summary>
    /// <param name="input">the input of the user</param>
    /// <returns>String of the proposition if one is selected, an empty string if accuracy too low</returns>
    public string TestSelectedProposition(string input)
    {
        m_IsInteractable = false;

        m_LeftArrowImage.enabled = false;
        m_RightArrowImage.enabled = false;
        if (m_Index == m_Propositions.Count)
        {
            m_PropositionText.text = input;
            return input;
        }
        else
        {
            float accuracy = ComputeAccuracy(m_Propositions[m_Index], input);
            accuracy = Mathf.Min(100f, accuracy + m_AccuracyBonus);
            m_AccuracyText.text = string.Format("{0}%", (int) accuracy);
            DisplayWithErrors(m_Propositions[m_Index], input);
            if (accuracy < m_MinAccuracy)
            {
                m_AccuracyText.color = m_ErrorColor;
                return "";
            } else if (accuracy < m_CorrectAccuracy)
            {
                m_AccuracyText.color = m_NotBadColor;
                return m_Propositions[m_Index];
            } else
            {
                m_AccuracyText.color = m_CorrectColor;
                return m_Propositions[m_Index];
            }
        }
    }

    /// <summary>
    /// Computes the accuracy (percentage) of the end text results in comparison to the given phrase, by using 
    /// the Levenshtein Distance between the two strings.
    /// </summary>
    /// <param name="propositionPhrase">The phrase to compare against</param>
    /// <param name="inputPhrase">The phrase to compare against</param>
    /// <returns>Accuracy</returns>
    float ComputeAccuracy(string propositionPhrase, string inputPhrase)
    {
        inputPhrase = StringUtilities.TrimSpecialFormatting(inputPhrase, new HashSet<char> {},
            m_LeadingCharsForSpecialWords, m_SurroundingCharsForSpecialText);
        propositionPhrase = StringUtilities.TrimSpecialFormatting(propositionPhrase, new HashSet<char>(),
            m_LeadingCharsForSpecialWords, m_SurroundingCharsForSpecialText);

        int levenDistance = StringUtilities.LevenshteinDistance(inputPhrase, propositionPhrase);
        float accuracy = Mathf.Max(0, 100f - (100f * (float)levenDistance / (float)propositionPhrase.Length));
            return accuracy;
    }

    public void SetPropositions(List<string> propositions)
    {
        m_IsInteractable = true;
        m_LeftArrowImage.enabled = true;
        m_RightArrowImage.enabled = true;
        m_AccuracyText.text = "";
        m_Propositions.Clear();
        m_Propositions.AddRange(propositions);
        m_Index = 0;
        SetPropositionText();
    }

    public void Reset()
    {
        SetPropositions(m_GreetingsPropositions);
    }

    void DisplayWithErrors(string propositionPhrase, string inputPhrase)
    {
        inputPhrase = StringUtilities.TrimSpecialFormatting(inputPhrase, new HashSet<char>(),
           m_LeadingCharsForSpecialWords, m_SurroundingCharsForSpecialText);
        propositionPhrase = StringUtilities.TrimSpecialFormatting(propositionPhrase, new HashSet<char>(),
            m_LeadingCharsForSpecialWords, m_SurroundingCharsForSpecialText);

        string outputText = "";
        string[] splitProp = propositionPhrase.Split(' ');
        string[] splitInput = inputPhrase.Split(' ');
        int indexProp = 0;
        int indexInput = 0;
        while (indexProp < splitProp.Length)
        {
            if (indexInput < splitInput.Length)
            {
                int levenDistance = StringUtilities.LevenshteinDistance(splitInput[indexInput], splitProp[indexProp]);
                float accuracy = Mathf.Max(0, 100f - (100f * (float)levenDistance / (float)splitProp[indexProp].Length));
                float nextAccuracy = 0;
                if (indexInput < splitInput.Length - 1)
                {
                    int nextLevenDistance = StringUtilities.LevenshteinDistance(splitInput[indexInput + 1], splitProp[indexProp]);
                    nextAccuracy = Mathf.Max(0, 100f - (100f * (float)nextLevenDistance / (float)splitProp[indexProp].Length));
                }

                if (nextAccuracy > accuracy)
                {
                    outputText += string.Format("<color=#{0}>[?] </color>", ColorUtility.ToHtmlStringRGBA(m_ErrorColor));
                    accuracy = nextAccuracy;
                    indexInput++;
                }

                if (accuracy < m_MinAccuracy)
                {
                    outputText += string.Format("<color=#{0}>{1} </color>", ColorUtility.ToHtmlStringRGBA(m_ErrorColor), splitProp[indexProp]);
                }
                else if (accuracy < m_CorrectAccuracy)
                {
                    outputText += string.Format("<color=#{0}>{1} </color>", ColorUtility.ToHtmlStringRGBA(m_NotBadColor), splitProp[indexProp]);
                }
                else
                {
                    outputText += string.Format("<color=#{0}>{1} </color>", ColorUtility.ToHtmlStringRGBA(m_CorrectColor), splitProp[indexProp]);
                }
            } else
            {
                outputText += string.Format("<color=#{0}>[...] </color>", ColorUtility.ToHtmlStringRGBA(m_ErrorColor));
            }
            indexProp++;
            indexInput++;
        }
        m_PropositionText.text = outputText;

    }
}
