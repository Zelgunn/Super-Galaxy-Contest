using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoamerUI : MonoBehaviour
{
    [SerializeField] private Image m_roamerFillBar;

    [SerializeField] private RawImage m_portrait;

    [Header("Colored UI")]
    [SerializeField] private RawImage[] m_powersUI;
    [SerializeField] private RawImage[] m_basicColoredUIs;

    private Color m_baseColor;
    private Color m_fillColor;

    private bool m_powersBlinkingReset = false;
    private float m_powersBlinkingTimer = 0;

    private void Awake()
    {
        m_portrait.enabled = false;
    }

	private void Update ()
    {
        m_roamerFillBar.fillAmount = Team.localTeam.roamerStrenght;

        if(Team.localTeam.powersAvailable > 0)
        {
            if (!m_powersBlinkingReset)
                m_powersBlinkingTimer = 0;
            else
                m_powersBlinkingTimer += Time.deltaTime;

            Color blinkColor = Color.white;
            if(m_powersBlinkingTimer > 0.5f)
            {
                blinkColor = m_fillColor;
                if(m_powersBlinkingTimer > 1)
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

    public void SetRoamerPortrait()
    {
        m_portrait.enabled = true;

        if (GalaxyNetworkManager.localConfiguration.isHuman)
        {
            m_portrait.texture = GameData.humanRoamerIcon;
            m_fillColor = GameData.humanFillColor;
            m_baseColor = GameData.humanColor;
        }
        else
        {
            m_portrait.texture = GameData.robotRoamerIcon;
            m_fillColor = GameData.robotFillColor;
            m_baseColor = GameData.robotColor;
        }

        m_roamerFillBar.color = m_fillColor;

        foreach (RawImage image in m_powersUI)
        {
            image.color = m_fillColor;
        }

        foreach (RawImage image in m_basicColoredUIs)
        {
            image.color = m_baseColor;
        }
    }
}
