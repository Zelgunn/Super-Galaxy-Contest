using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Ball : NetworkBehaviour
{
    static private Ball s_singleton;
    [SerializeField] private float m_forceFeedbackFactor;
    private Rigidbody m_rigidbody;
    private NetworkIdentity m_networkIndentity;

    private void Awake()
    {
        s_singleton = this;
        m_rigidbody = GetComponent<Rigidbody>();
        m_networkIndentity = GetComponent<NetworkIdentity>();
    }

    //private void Update()
    //{
    //    if(transform.position)
    //}

    private void OnCollisionEnter(Collision other)
    {
		m_rigidbody.velocity *= 1.1f;

		Roamer roamer = other.gameObject.GetComponent<Roamer>();
        if(roamer == null)
            return;

        m_rigidbody.AddForce(other.contacts[0].normal * 5 * roamer.strenght * 2, ForceMode.Impulse);

        if (!NetworkClient.active)
            return;
        if (roamer.isHuman != Character.characterControlledByLocalPlayer.isHuman)
            return;

        Vector3 direction = 100 * other.contacts[0].normal * roamer.velocity;
        FalconUnity.setForceField(0, direction * m_forceFeedbackFactor * 5);
    }

    private void OnCollisionExit(Collision other)
    {
		Roamer roamer = other.gameObject.GetComponent<Roamer>();
		if(roamer == null)
			return;

        Vector3 tmp = m_rigidbody.velocity;
        tmp.z /= 5;
        m_rigidbody.velocity = tmp;

        if (!NetworkClient.active)
            return;
		if (roamer.isHuman != Character.characterControlledByLocalPlayer.isHuman)
			return;

        FalconUnity.setForceField(0, Vector3.zero);
    }

    [Server]
    public void GiveToPlayer(bool isHuman, Vector3 position)
    {
        if (m_networkIndentity.clientAuthorityOwner != null)
        {
            m_networkIndentity.RemoveClientAuthority(m_networkIndentity.clientAuthorityOwner);
        }

        if(isHuman)
        {
            m_networkIndentity.AssignClientAuthority(Team.humanTeam.connectionToClient);
        }
        else
        {

            m_networkIndentity.AssignClientAuthority(Team.robotTeam.connectionToClient);
        }

        RpcGiveToPlayer(position);
    }

    [ClientRpc]
    private void RpcGiveToPlayer(Vector3 position)
    {
        transform.position = position;
        m_rigidbody.position = position;
        m_rigidbody.velocity = Vector3.zero;
    }

    public NetworkIdentity networkIdentity
    {
        get { return m_networkIndentity; }
    }

    public Rigidbody ballRigidbody
    {
        get { return m_rigidbody; }
    }

    static public Ball singleton
    {
        get { return s_singleton; }
    }
}
