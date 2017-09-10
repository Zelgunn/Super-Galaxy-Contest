using UnityEngine;
using System.Collections;

public class Fireworks : MonoBehaviour
{
    static private Fireworks s_singleton;

    [SerializeField] private GameObject m_blueFireworks;
    [SerializeField] private GameObject m_redFireworks;

    private ParticleSystem[] m_blueParticlesSystems;
    private ParticleSystem[] m_redParticlesSystems;

    private void Awake ()
    {
        s_singleton = this;

        m_blueParticlesSystems = m_blueFireworks.GetComponentsInChildren<ParticleSystem>();
        m_redParticlesSystems = m_redFireworks.GetComponentsInChildren<ParticleSystem>();
    }

    static public void UseBlueFireworks()
    {
        foreach (ParticleSystem particleSystem in s_singleton.m_blueParticlesSystems)
        {
            particleSystem.Stop();
            particleSystem.Play();
        }
    }

    static public void UseRedFireworks()
    {
        foreach (ParticleSystem particleSystem in s_singleton.m_redParticlesSystems)
        {
            particleSystem.Stop();
            particleSystem.Play();
        }
    }
}
