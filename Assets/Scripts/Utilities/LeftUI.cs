using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftUI : MonoBehaviour
{
    public Image m_Background;
    public Text m_RecordInfoText;
    public Text m_PropositionText;
    public Text m_AccuracyText;
    public Image m_MicImage;
    public Image m_LeftArrow;
    public Image m_RightArrow;
    public GameObject m_FeedbackTryAgain;
    public GameObject m_FeedbackGood;
    public GameObject m_FeedbackPerfect;

    private void Start()
    {
        m_FeedbackTryAgain.SetActive(false);
        m_FeedbackGood.SetActive(false);
        m_FeedbackPerfect.SetActive(false);
    }

    public void SetTryAgainFeedback(bool active)
    {
        m_FeedbackTryAgain.SetActive(active);
        m_FeedbackGood.SetActive(false);
        m_FeedbackPerfect.SetActive(false);
    }

    public void SetGoodFeedback(bool active)
    {
        m_FeedbackTryAgain.SetActive(false);
        m_FeedbackGood.SetActive(active);
        m_FeedbackPerfect.SetActive(false);
    }

    public void SetPerfectFeedback(bool active)
    {
        m_FeedbackTryAgain.SetActive(false);
        m_FeedbackGood.SetActive(false);
        m_FeedbackPerfect.SetActive(active);
    }

    public void ResetFeedbacks()
    {
        m_FeedbackTryAgain.SetActive(false);
        m_FeedbackGood.SetActive(false);
        m_FeedbackPerfect.SetActive(false);
    }

}
