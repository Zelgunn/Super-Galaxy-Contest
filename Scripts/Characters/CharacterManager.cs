using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour
{
    static private CharacterManager s_singleton;

    [Header("Humans")]
    [SerializeField] private Character m_humanRoamer;
    [SerializeField] private Character m_humanSnooper;

    [Header("Robots")]
    [SerializeField] private Character m_robotRoamer;
    [SerializeField] private Character m_robotSnooper;

    private void Awake ()
    {
        s_singleton = this;
        Team.SetRoamersAndSnoopers();
    }

    static public Character humanRoamer
    {
        get { return s_singleton.m_humanRoamer; }
    }

    static public Character humanSnooper
    {
        get { return s_singleton.m_humanSnooper; }
    }

    static public Character robotRoamer
    {
        get { return s_singleton.m_robotRoamer; }
    }

    static public Character robotSnooper
    {
        get { return s_singleton.m_robotSnooper; }
    }

    static public CharacterManager singleton
    {
        get { return s_singleton; }
    }
}
