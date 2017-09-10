using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class GoalUI : MonoBehaviour
{
    static private GoalUI s_singleton;
    static private GoalUI s_snooperSingleton;
    static private int s_hashShow = Animator.StringToHash("Show");

    private Animator m_animator;
    [SerializeField] private bool m_snooperUI = false;


    private void Awake()
    {
        if (m_snooperUI)
        {
            s_snooperSingleton = this;
        }
        else
        {
            s_singleton = this;
        }
        m_animator = GetComponent<Animator>();
	}

    static public void ShowGoalUI()
    {
        s_singleton.m_animator.SetTrigger(s_hashShow);
        if(NetworkClient.active)
        {
            s_snooperSingleton.m_animator.SetTrigger(s_hashShow);
        }
    }
}
