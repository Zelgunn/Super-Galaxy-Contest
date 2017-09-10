using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PreparationCountdown : MonoBehaviour
{
    static private PreparationCountdown s_singleton;
    static private PreparationCountdown s_snooperSingleton;

    [SerializeField] private bool m_snooperUI = false;

    [Header("UI elements")]
    [SerializeField] private Texture[] m_preparatinCountdownImages;
    [SerializeField] private RawImage m_currentNumber;
    [SerializeField] private RawImage m_previousNumber;

    [Header("Anchors")]
    [SerializeField] private RectTransform m_rightAnchor;
    [SerializeField] private RectTransform m_centerAnchor;
    [SerializeField] private RectTransform m_leftAnchor;

    [Header("Animation settings")]
    [SerializeField] private float m_animationDuration = 0.1f;
    [SerializeField] private int m_animationStepCount = 15;

    private void Awake()
    {
        if(m_snooperUI)
        {
            s_snooperSingleton = this;
        }
        else
        {
            s_singleton = this;
        }
    }

    private void Start()
    {
        EnableComponents(false);
	}

    private void EnableComponents(bool enable)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(enable);
        }
    }

    private void _ShowPreparationNumber(int numberToShow)
    {
        if ((numberToShow < 0) || (numberToShow >= m_preparatinCountdownImages.Length))
        {
            EnableComponents(false);
            return;
        }

        EnableComponents(true);
        m_currentNumber.enabled = true;

        m_currentNumber.texture = m_preparatinCountdownImages[numberToShow];

        if (numberToShow < (m_preparatinCountdownImages.Length - 1))
        {
            m_previousNumber.enabled = true;
            m_previousNumber.texture = m_preparatinCountdownImages[numberToShow + 1];
        }
        else
        {
            m_previousNumber.enabled = false;
        }

        StartCoroutine(CoutdownAnimationCoroutine());
    }

    private IEnumerator CoutdownAnimationCoroutine()
    {
        float stepDuration = m_animationDuration / m_animationStepCount;
        float animationPercentage = 0;

        for(int i = 0; i < m_animationStepCount; i++)
        {
            animationPercentage = (float)i / (float)m_animationStepCount;

            m_currentNumber.transform.position = Vector3.Lerp(m_rightAnchor.transform.position, m_centerAnchor.transform.position, animationPercentage);

            if(m_previousNumber.enabled)
                m_previousNumber.transform.position = Vector3.Lerp(m_centerAnchor.transform.position, m_leftAnchor.transform.position, animationPercentage);

            yield return new WaitForSeconds(stepDuration);
        }

        m_currentNumber.transform.position = m_centerAnchor.transform.position;
        m_previousNumber.enabled = false;
    }

    static public void ShowPreparationNumber(int numberToShow)
    {
        s_singleton._ShowPreparationNumber(numberToShow);
        if (NetworkClient.active)
        {
            s_snooperSingleton._ShowPreparationNumber(numberToShow);
        }
    }
}
