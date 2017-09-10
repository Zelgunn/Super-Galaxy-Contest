using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SnooperUI : MonoBehaviour
{
    static private SnooperUI s_singleton;

    [SerializeField] private Image m_snooperFillBar;

    [SerializeField] private RawImage m_portrait;
    [SerializeField] private Text m_snooperTimer;

    [Header("Colored UI")]
    [SerializeField] private RawImage[] m_powersUI;
    [SerializeField] private RawImage[] m_basicColoredUIs;

    private Color m_baseColor;
    private Color m_fillColor;

    private bool m_powersBlinkingReset = false;
    private float m_powersBlinkingTimer = 0;

    private void Awake()
    {
        s_singleton = this;
        m_portrait.enabled = false;
    }

	private void Update ()
    {
        m_snooperTimer.text = ClientUI.timerRemainingSeconds.ToString();
        m_snooperFillBar.fillAmount = Team.localTeam.snooperSpeed;

        if (Team.localTeam.powersAvailable > 0)
        {
            if (!m_powersBlinkingReset)
                m_powersBlinkingTimer = 0;
            else
                m_powersBlinkingTimer += Time.deltaTime;

            Color blinkColor = Color.white;
            if (m_powersBlinkingTimer > 0.5f)
            {
                blinkColor = m_fillColor;
                if (m_powersBlinkingTimer > 1)
                {
                    m_powersBlinkingTimer = 0;
                }
            }

            foreach (RawImage image in m_powersUI)
            {
                image.color = blinkColor;
            }

            m_powersBlinkingReset = true;
        }
        else
        {
            foreach (RawImage image in m_powersUI)
            {
                image.color = m_fillColor;
            }

            m_powersBlinkingReset = false;
        }
    }

    public void SetSnooperPortrait()
    {
        m_portrait.enabled = true;

        if (GalaxyNetworkManager.localConfiguration.isHuman)
        {
            m_portrait.texture = GameData.humanSnooperIcon;
            m_fillColor = GameData.humanFillColor;
            m_baseColor = GameData.humanColor;
        }
        else
        {
            m_portrait.texture = GameData.robotSnooperIcon;
            m_fillColor = GameData.robotFillColor;
            m_baseColor = GameData.robotColor;
        }

        m_snooperFillBar.color = m_fillColor;

        foreach (RawImage image in m_powersUI)
        {
            image.color = m_fillColor;
        }

        foreach (RawImage image in m_basicColoredUIs)
        {
            image.color = m_baseColor;
        }
    }

    static public void ShowSnooperPlayUI(bool show)
    {
        s_singleton.gameObject.SetActive(show);
    }
}
