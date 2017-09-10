using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class Timer : MonoBehaviour
{
    private Text m_display;
    private int m_remainingSeconds = 0;
    private bool m_paused = false;

    public void Awake()
    {
        m_display = GetComponent<Text>();
        m_display.enabled = false;
    }

    public void StartTimer(int seconds)
    {
        StartCoroutine(_StartTimer(seconds));
    }

    private IEnumerator _StartTimer(int seconds)
    {
        m_remainingSeconds = seconds;
        m_display.enabled = true;

        while (m_remainingSeconds > 0)
        {
            m_display.text = (m_remainingSeconds - 1).ToString();
            yield return new WaitForSeconds(1.0f);
            while (m_paused)
                yield return null;
            m_remainingSeconds--;
        }

        m_display.enabled = false;
    }

    public void Pause()
    {
        m_paused = true;
    }

    public void Resume()
    {
        m_paused = false;
    }

    public int remainingSeconds
    {
        get { return m_remainingSeconds; }
    }
}
