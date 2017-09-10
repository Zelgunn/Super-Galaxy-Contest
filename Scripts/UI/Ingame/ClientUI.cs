using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class ClientUI : MonoBehaviour
{
    static private ClientUI s_singleton;

    [SerializeField] private Timer m_timer;

    [Header("SubUIs")]
    [SerializeField] private RoamerUI m_roamerUI;
    [SerializeField] private SnooperUI m_snooperUI;

    private void Awake()
    {
        s_singleton = this;
	}

    private void _ShowClientUI(bool show)
    {
        gameObject.SetActive(show);

        if (show)
        {
            m_roamerUI.SetRoamerPortrait();
            m_snooperUI.SetSnooperPortrait();
        }
    }

    static public void ShowClientUI(bool show)
    {
        s_singleton._ShowClientUI(show);
    }

    static public void StartTimer(int seconds)
    {
        s_singleton.m_timer.StartTimer(seconds);
    }

    static public void Hide()
    {
        if (s_singleton == null) return;
        s_singleton.gameObject.SetActive(false);
    }

    static public int timerRemainingSeconds
    {
        get { return s_singleton.m_timer.remainingSeconds; }
    }
}
