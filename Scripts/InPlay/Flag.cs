using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(CapsuleCollider))]
public class Flag : NetworkBehaviour
{
    static private Flag s_singleton;

    [SerializeField] private GameObject m_bigFlag;
    [SerializeField] private GameObject m_smallFlag;

    private Snooper m_taker = null;

	public void Start ()
    {
        s_singleton = this;
    }
	
	public void Update ()
    {
        m_bigFlag.SetActive(m_taker == null);
        m_smallFlag.SetActive(m_taker != null);

        if (m_taker == null)
        {
            Vector3 tmp = transform.position;
            tmp.y = 0;
            transform.position = tmp;
            transform.eulerAngles = Vector3.zero;
        }
        else
        {
            transform.position = m_taker.flagAttach.position;
            transform.eulerAngles = m_taker.flagAttach.eulerAngles;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if((m_taker != null) || !isServer)
        {
            return;
        }

        Snooper snooper = other.GetComponent<Snooper>();

        if (snooper == null)
            return;

        AttachToSnooper(snooper.isHuman);
    }

    [Server]
    private void AttachToSnooper(bool isHumanSnooper)
    {
        RpcAttachToSnooper(isHumanSnooper);
        _AttachToSnooper(isHumanSnooper);
    }

    [ClientRpc]
    private void RpcAttachToSnooper(bool isHumanSnooper)
    {
        _AttachToSnooper(isHumanSnooper);
        FlagManager.ShowPlaceholder(true);
    }

    private void _AttachToSnooper(bool isHumanSnooper)
    {
        if(isHumanSnooper)
        {
            m_taker = Snooper.humanSnooper;
        }
        else
        {
            m_taker = Snooper.robotSnooper;
        }

        m_taker.PickFlag();
        GalaxyAudioPlayer.PlaySnooperCatchesFlagSound();
    }

    [Server]
    public void Detach()
    {
        RpcDetach();
        _Detach();
    }

    [ClientRpc]
    private void RpcDetach()
    {
        _Detach();
        FlagManager.ShowPlaceholder(true);
    }

    private void _Detach()
    {
        m_taker = null;
        GalaxyAudioPlayer.PlaySnooperDropsFlagSound();
    }

    public Snooper taker
    {
        get { return m_taker; }
    }

    [Server]
    public void ForceUpdatePosition()
    {
        RpcForceUpdatePosition(transform.position);
    }

    [ClientRpc]
    private void RpcForceUpdatePosition(Vector3 position)
    {
        transform.position = position;
    }

    static public Flag singleton
    {
        get { return s_singleton; }
    }
}
