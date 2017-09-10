using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Previewer : MonoBehaviour
{
    [SerializeField] private RawImage m_target;
    [SerializeField] private bool m_isRoamer;
    [SerializeField] private Animator m_animator;

    static private int m_hashReady = Animator.StringToHash("Ready");
    static private int m_hashFight = Animator.StringToHash("Fight");

    public void SetReady(bool ready)
    {
        if(m_isRoamer)
        {
            m_animator.SetBool(m_hashReady, ready);
        }
        else
        {
            m_animator.SetBool(m_hashFight, ready);
        }
    }

    public void Show(bool show)
    {
        foreach(Transform t in transform)
        {
            t.gameObject.SetActive(show);
        }
        m_target.gameObject.SetActive(show);
    }
}
