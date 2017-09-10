using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour
{
    static private GameData s_singleton;

    [SerializeField] private int m_gameDuration = 180;

    [Header("Colors")]
    [SerializeField] private Color m_humanColor;
    [SerializeField] private Color m_humanFillColor;
    [SerializeField] private Color m_robotColor;
    [SerializeField] private Color m_robotFillColor;

    [Header("Portraits")]
    [SerializeField] private Texture m_humanRoamerIcon;
    [SerializeField] private Texture m_humanSnooperIcon;
    [SerializeField] private Texture m_robotRoamerIcon;
    [SerializeField] private Texture m_robotSnooperIcon;

    [Header("Status Icons")]
    [SerializeField] private Texture m_bonusIcon;
    [SerializeField] private Texture m_malusIcon;

    private void Awake()
    {
        s_singleton = this;
    }

    #region Colors
    static public Color humanColor
    {
        get { return s_singleton.m_humanColor; }
    }

    static public Color humanFillColor
    {
        get { return s_singleton.m_humanFillColor; }
    }

    static public Color robotColor
    {
        get { return s_singleton.m_robotColor; }
    }

    static public Color robotFillColor
    {
        get { return s_singleton.m_robotFillColor; }
    }
    #endregion

    static public int gameDuration
    {
        get { return s_singleton.m_gameDuration; }
    }

    #region Portraits
    static public Texture humanRoamerIcon
    {
        get { return s_singleton.m_humanRoamerIcon; }
    }

    static public Texture humanSnooperIcon
    {
        get { return s_singleton.m_humanSnooperIcon; }
    }

    static public Texture robotRoamerIcon
    {
        get { return s_singleton.m_robotRoamerIcon; }
    }

    static public Texture robotSnooperIcon
    {
        get { return s_singleton.m_robotSnooperIcon; }
    }
    #endregion
    #region Status Icons
    static public Texture bonusStatusIcon
    {
        get { return s_singleton.m_bonusIcon; }
    }

    static public Texture malusStatusIcon
    {
        get { return s_singleton.m_malusIcon; }
    }
    #endregion
}
