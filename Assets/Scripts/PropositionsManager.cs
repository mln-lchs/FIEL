using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnitySpeechToText.Utilities;

public class PropositionsManager : MonoBehaviour
{
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
    /// UI references
    /// </summary>
    [SerializeField]
    LeftUI m_LeftUI;

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
    /// Index of the selected proposition
    /// </summary>
    int m_Index = 0;

    // <summary>
    /// Set of leading characters for words to ignore when computing accuracy, which includes '%' by default
    /// to account for Watson's "%HESITATION" in results
    /// </summary>
    HashSet<char> m_LeadingCharsForSpecialWords = new HashSet<char> { '%' };
    /// <summary>
    /// Set of surrounding characters for text to ignore when computing accuracy, which includes brackets
    /// by default to account for instructions in speech such as "[pause]"
    /// </summary>
    Dictionary<char, char> m_SurroundingCharsForSpecialText = new Dictionary<char, char> { { '[', ']' } };

    // Start is called before the first frame update
    void Start()
    {
        m_PropositionText = m_LeftUI.m_PropositionText;
        m_AccuracyText = m_LeftUI.m_AccuracyText;

        // Init propositions
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_Index++;
            if (m_Index > m_Propositions.Count) m_Index = 0;
            SetPropositionText();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            m_Index--;
            if (m_Index < 0) m_Index = m_Propositions.Count;
            SetPropositionText();
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
        if (m_Index == m_Propositions.Count)
        {
            return input;
        }
        else
        {
            float accuracy = ComputeAccuracy(m_Propositions[m_Index], input);
            accuracy = Mathf.Min(100f, accuracy + m_AccuracyBonus);
            m_AccuracyText.text = string.Format("{0}%", (int) accuracy);
            if (accuracy < m_MinAccuracy)
            {
                return "";
            } else
            {
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
        string speechToTextResult = StringUtilities.TrimSpecialFormatting(inputPhrase, new HashSet<char>(),
            m_LeadingCharsForSpecialWords, m_SurroundingCharsForSpecialText);
        propositionPhrase = StringUtilities.TrimSpecialFormatting(propositionPhrase, new HashSet<char>(),
            m_LeadingCharsForSpecialWords, m_SurroundingCharsForSpecialText);

        int levenDistance = StringUtilities.LevenshteinDistance(speechToTextResult, propositionPhrase);
        float accuracy = Mathf.Max(0, 100f - (100f * (float)levenDistance / (float)propositionPhrase.Length));
            return accuracy;
    }

    public void SetPropositions(List<string> propositions)
    {
        m_Propositions.Clear();
        m_Propositions.AddRange(propositions);
        m_Index = 0;
        SetPropositionText();
    }

    public void Reset()
    {
        SetPropositions(m_GreetingsPropositions);
    }
}
