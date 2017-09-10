using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Canvas))]
public class LobbyClientMenuManager : MonoBehaviour
{
    static private LobbyClientMenuManager s_roamerSingleton;
    static private LobbyClientMenuManager s_snooperSingleton;

    [SerializeField] private RawImage m_innerCircle;
 
    [Header("Selector")]
    [SerializeField] private Transform m_overlays;
    [SerializeField] private RawImage m_roamerTeamAOverlay;
    [SerializeField] private RawImage m_snooperTeamAOverlay;
    [SerializeField] private RawImage m_roamerTeamBOverlay;
    [SerializeField] private RawImage m_snooperTeamBOverlay;

    [Header("Ready button")]
    [SerializeField] private RawImage m_readyButton;
    [SerializeField] private Texture[] m_readyTexturesForAnimation;

    private float m_innerCircleSpeed = 10;
    private bool m_readyAnimationInProgress = false;
    private Canvas m_canvas;

    private void Awake()
    {
        m_canvas = GetComponent<Canvas>();

        if(m_canvas.targetDisplay == 0)
        {
            s_roamerSingleton = this;
        }
        else
        {
            s_snooperSingleton = this;
        }
    }

    private void Update()
    {
        Vector3 innerCircleRotation = m_innerCircle.transform.eulerAngles;
        innerCircleRotation.z += Time.deltaTime * m_innerCircleSpeed;
        m_innerCircle.transform.eulerAngles = innerCircleRotation;

    }

    public void UpdateSelectors()
    {
        UpdateSelector(m_roamerTeamAOverlay,    true, true);
        UpdateSelector(m_snooperTeamAOverlay,   true, false);
        UpdateSelector(m_roamerTeamBOverlay,    false, true);
        UpdateSelector(m_snooperTeamBOverlay,   false, false);
    }

    private void UpdateSelector(RawImage selector, bool humanSelector, bool roamerSelector)
    {
        Team selectorTeam = Team.humanTeam;
        if (!humanSelector) selectorTeam = Team.robotTeam;

        if(selectorTeam == null)
        {
            selector.gameObject.SetActive(false);
            return;
        }

        selector.gameObject.SetActive(true);
        bool selectorReady;
        if (roamerSelector)
            selectorReady = selectorTeam.isRoamerReady;
        else
            selectorReady = selectorTeam.isSnooperReady;

        if(selectorReady)
        {
            selector.color = Color.yellow;
        }
        else
        {
            selector.color = Color.white;
        }
    }

    private IEnumerator SetReadyCoroutine(bool ready)
    {
        m_readyAnimationInProgress = true;

        int frameCount = m_readyTexturesForAnimation.Length;
        int i = 0;
        int frameIterator = 1;
        if (!ready)
        {
            i = frameCount - 1;
            frameIterator = -frameIterator;
        }

        while ((i >= 0) && (i < frameCount))
        {
            m_readyButton.texture = m_readyTexturesForAnimation[i];

            i += frameIterator;
            yield return new WaitForSeconds(0.03f);
        }

        UpdateSelectors();

        m_readyAnimationInProgress = false;
    }

    static public void SetRoamerReady(bool ready)
    {
        s_roamerSingleton.StartCoroutine(s_roamerSingleton.SetReadyCoroutine(ready));
    }

    static public void SetSnooperReady(bool ready)
    {
        s_snooperSingleton.StartCoroutine(s_snooperSingleton.SetReadyCoroutine(ready));
    }

    static public bool isReadyAnimationInProgressForRoamer
    {
        get { return s_roamerSingleton.m_readyAnimationInProgress; }
    }

    static public bool isReadyAnimationInProgressForSnooper
    {
        get { return s_snooperSingleton.m_readyAnimationInProgress; }
    }
}
