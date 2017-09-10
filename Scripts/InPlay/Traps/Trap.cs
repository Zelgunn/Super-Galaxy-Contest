using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AudioSource))]
public class Trap : MonoBehaviour
{
    private float m_creationTime;
    protected int m_tileId;

    [SerializeField] private Renderer m_renderer;
    protected AudioSource m_audioSource;
    [SerializeField] protected AudioClip m_triggerSound;
    [SerializeField] protected ParticleSystem m_particuleOnTrigger;

    private void Awake()
    {
        m_creationTime = Time.time;
        m_audioSource = GetComponent<AudioSource>();
        m_particuleOnTrigger.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(!NetworkClient.active)
        {
            return;
        }

        Color mainColor = Color.white;
        Color emissionColor = mainColor;
        float distanceToSnooper;

        if(GalaxyNetworkManager.localConfiguration.isHuman)
        {
            distanceToSnooper = (Snooper.humanSnooper.transform.position - transform.position).magnitude;
        }
        else
        {
            distanceToSnooper = (Snooper.robotSnooper.transform.position - transform.position).magnitude;
        }

        float alpha = (1 - distanceToSnooper/2.5f);
        alpha = Mathf.Max(0, Mathf.Min(1, alpha));

        mainColor.a = alpha;
        emissionColor *= alpha;

        m_renderer.material.color = mainColor;
        m_renderer.material.SetColor("_EmissionColor", emissionColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        Snooper trapedSnooper = other.GetComponent<Snooper>();

        if (trapedSnooper == null)
            return;

        if (trapedSnooper.isJumping)
            return;

        m_audioSource.PlayOneShot(m_triggerSound);

        m_particuleOnTrigger.gameObject.SetActive(true);
        m_particuleOnTrigger.Play();
        if(NetworkServer.active)
        {
            TrapManager.RemoveTrap(this);
            if(trapedSnooper.isHuman)
            {
                Team.humanTeam.IncreaseTrapsTakenCount();
            }
            else
            {
                Team.robotTeam.IncreaseTrapsTakenCount();
            }
        }

        if (!trapedSnooper.hasAuthority)
            return;

        OnTrapTriggered(other, trapedSnooper);
    }

    virtual protected void OnTrapTriggered(Collider other, Snooper trapedSnooper)
    {

    }

    public float timeLived
    {
        get { return Time.time - m_creationTime; }
    }

    public void SetTileID(int tileID)
    {
        m_tileId = tileID;
        transform.position = TrapManager.TilePosition(tileID);
    }

    public int tileId
    {
        get { return m_tileId; }
    }
}
