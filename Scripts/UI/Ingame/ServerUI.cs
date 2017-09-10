using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ServerUI : MonoBehaviour
{
    static private ServerUI s_singleton;
   
    [Header("Timer")]
    [SerializeField] private Timer m_timer;

    private void Awake()
    {
        s_singleton = this;
	}

    static public void StartTimer(int seconds)
    {
        s_singleton.m_timer.StartTimer(seconds);
    }

    static public int timerRemainingSeconds
    {
        get { return s_singleton.m_timer.remainingSeconds; }
    }

    static public void ShowServerUI(bool show)
    {
        s_singleton.gameObject.SetActive(show);
    }

    static public void Hide()
    {
        if (s_singleton == null) return;
        s_singleton.gameObject.SetActive(false);
    }
}
