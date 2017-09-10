using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GalaxyTransform : NetworkBehaviour
{
    private Rigidbody m_rigidbody;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

	private void Update ()
    {
        if (!hasAuthority)
            return;

	    if(isServer)
        {
            ServerUpdateTransform();
        }
        else
        {
            CmdUpdateTransform();
        }
	}

    [Server]
    private void ServerUpdateTransform()
    {
        if (m_rigidbody == null)
        {
            RpcUpdateTransform(transform.position, transform.eulerAngles);
        }
        else
        {
            RpcUpdateRigidbody(m_rigidbody.position, m_rigidbody.rotation, m_rigidbody.velocity, m_rigidbody.angularVelocity);
        }
    }

    [Command]
    private void CmdUpdateTransform()
    {
        if(m_rigidbody == null)
        {
            RpcUpdateTransform(transform.position, transform.eulerAngles);
            _UpdateTransform(transform.position, transform.eulerAngles);
        }
        else
        {
            RpcUpdateRigidbody(m_rigidbody.position, m_rigidbody.rotation, m_rigidbody.velocity, m_rigidbody.angularVelocity);
            _UpdateRigidbody(m_rigidbody.position, m_rigidbody.rotation, m_rigidbody.velocity, m_rigidbody.angularVelocity);
        }
    }

    [ClientRpc]
    private void RpcUpdateTransform(Vector3 position, Vector3 angle)
    {
        _UpdateTransform(position, angle);
    }

    private void _UpdateTransform(Vector3 position, Vector3 angle)
    {
        transform.position = position;
        transform.eulerAngles = angle;
    }

    [ClientRpc]
    private void RpcUpdateRigidbody(Vector3 position, Quaternion angle, Vector3 velocity, Vector3 angularVelocity)
    {
        _UpdateRigidbody(position, angle, velocity, angularVelocity);
    }

    private void _UpdateRigidbody(Vector3 position, Quaternion angle, Vector3 velocity, Vector3 angularVelocity)
    {
        m_rigidbody.velocity = velocity;
        m_rigidbody.position = position;
        m_rigidbody.angularVelocity = angularVelocity;
        m_rigidbody.rotation = angle;
    }
}
