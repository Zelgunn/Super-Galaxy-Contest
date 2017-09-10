using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Rigidbody))]
public class Character : NetworkBehaviour
{
    static private Character s_characterControlledByLocalPlayer = null;
    private NetworkIdentity m_networkIdentity;
    protected Rigidbody m_rigidbody;

    [SerializeField] private bool m_isHuman;
    [SerializeField] private bool m_isRoamer;
    [SerializeField] protected Transform m_pointOfView;
    [SerializeField] protected RawImage m_statusIconTarget;

    virtual protected void Awake()
    {
        m_networkIdentity = GetComponent<NetworkIdentity>();
        m_rigidbody = GetComponent<Rigidbody>();
	}

    virtual protected void Start()
    {

    }

    virtual protected void Update()
    {

	}

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        if (hasAuthority && NetworkClient.active)
        {
            s_characterControlledByLocalPlayer = this;
        }
    }

    public void UpdateTransform(Vector3 position)
    {
        transform.position = position;
    }

    public NetworkIdentity networkIdentity
    {
        get { return m_networkIdentity; }
    }

    public bool isRoamer
    {
        get { return m_isRoamer; }
    }

    public bool isHuman
    {
        get { return m_isHuman; }
    }

    new public Rigidbody rigidbody
    {
        get { return m_rigidbody; }
    }

    static public Character characterControlledByLocalPlayer
    {
        get { return s_characterControlledByLocalPlayer; }
    }
}
