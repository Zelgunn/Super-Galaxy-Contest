using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioListener))]
[RequireComponent(typeof(AudioSource))]
public class GalaxyAudioPlayer : MonoBehaviour
{

    static private GalaxyAudioPlayer s_singleton;
    [Header("Ambiance")]
    private AudioSource m_ambianceAudioSource;
    [SerializeField] private AudioClip m_lobbyAmbiance;
    [SerializeField] private AudioClip m_mainAmbiance;
    [SerializeField] private AudioClip m_endAmbiance;

    [Header("One shot sounds")]
    [SerializeField] private AudioSource m_goalAudioSource;
    [SerializeField] private AudioSource m_powerAudioSource;
    [SerializeField] private AudioSource m_powerAvailableAudioSource;
    [SerializeField] private AudioSource m_playerReadyAudioSource;

    [Header("Snooper sounds")]
    [SerializeField] private AudioSource m_snooperGlobalAudioSource;
    [SerializeField] private AudioClip m_snooperCatchesFlagAudioClip;
    [SerializeField] private AudioClip m_snooperDropsFlagAudioClip;

    [Header("Countdown sounds")]
    [SerializeField] private AudioSource m_countdownAudioSource;
    [SerializeField] private AudioClip m_countdownSmallBip;
    [SerializeField] private AudioClip m_countdownLongBip;

    private void Awake()
    {
        s_singleton = this;

        m_ambianceAudioSource = GetComponent<AudioSource>();
        _PlayMainAmbiance();
    }

    static public void PlayGoalSound()
    {
        s_singleton.m_goalAudioSource.Play();
    }

    static public void PlayPowerSound()
    {
        s_singleton.m_powerAudioSource.Play();
    }

    static public void PlayPowerAvailableSound()
    {
        s_singleton.m_powerAvailableAudioSource.Play();
    }

    static public void PlayReadySong()
    {
        s_singleton.m_playerReadyAudioSource.Play();
    }

    private void _PlayLobbyAmbiance()
    {
        m_ambianceAudioSource.Stop();
        m_ambianceAudioSource.clip = m_lobbyAmbiance;
        m_ambianceAudioSource.loop = true;
        m_ambianceAudioSource.Play();
    }

    static public void PlayLobbyAmbiance()
    {
        s_singleton._PlayLobbyAmbiance();
    }

    private void _PlayMainAmbiance()
    {
        m_ambianceAudioSource.Stop();
        m_ambianceAudioSource.clip = m_mainAmbiance;
        m_ambianceAudioSource.loop = true;
        m_ambianceAudioSource.Play();
    }

    static public void PlayMainAmbiance()
    {
        s_singleton._PlayMainAmbiance();
    }

    private void _PlayEndAmbiance()
    {
        m_ambianceAudioSource.Stop();
        m_ambianceAudioSource.clip = m_endAmbiance;
        m_ambianceAudioSource.loop = false;
        m_ambianceAudioSource.Play();
    }

    static public void PlayEndAmbiance()
    {
        s_singleton._PlayEndAmbiance();
    }

    private void _PlaySnooperCatchesFlagSound()
    {
        m_snooperGlobalAudioSource.PlayOneShot(m_snooperCatchesFlagAudioClip);
    }

    static public void PlaySnooperCatchesFlagSound()
    {
        s_singleton._PlaySnooperCatchesFlagSound();
    }

    private void _PlaySnooperDropsFlagSound()
    {
        m_snooperGlobalAudioSource.PlayOneShot(m_snooperDropsFlagAudioClip);
    }

    static public void PlaySnooperDropsFlagSound()
    {
        s_singleton._PlaySnooperDropsFlagSound();
    }

    private void _PlaySmallBip()
    {
        m_countdownAudioSource.PlayOneShot(m_countdownSmallBip);
    }

    static public void PlaySmallBip()
    {
        s_singleton._PlaySmallBip();
    }

    private void _PlayLongBip()
    {
        m_countdownAudioSource.PlayOneShot(m_countdownLongBip);
    }

    static public void PlayLongBip()
    {
        s_singleton._PlayLongBip();
    }


}
