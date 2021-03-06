using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Roamer : Character
{
    private Animator m_animator;
    static private int s_hashTurn = Animator.StringToHash("Turn");

    private Vector3 m_origin;
    [SerializeField] private CapsuleCollider m_capsuleCollider;
    private Vector3 m_lastPosition;
    private Vector3 m_currentPosition;
    private float m_strenght;

    override protected void Awake()
    {
        base.Awake();
        m_origin = transform.position;
        m_lastPosition = m_origin;
        m_currentPosition = m_origin;

        m_animator = GetComponent<NetworkAnimator>().animator;
    }

    override protected void Update()
    {
        base.Update();

        if(isHuman)
        {
            m_strenght = Team.humanTeam.roamerStrenght;
        }
        else
        {
            m_strenght = Team.robotTeam.roamerStrenght;
        }

        #region Status Icon Update
        Color iconColor = m_statusIconTarget.color;
        if (m_strenght < 0.5f)
        {
            iconColor.a = (0.5f - m_strenght) * 2;
            m_statusIconTarget.texture = GameData.malusStatusIcon;
        }
        else
        {
            iconColor.a = (m_strenght - 0.5f) * 2;
            m_statusIconTarget.texture = GameData.bonusStatusIcon;
        }
        m_statusIconTarget.color = iconColor;
        #endregion

        if (hasAuthority && NetworkClient.active)
        {
            Vector3 falconPosition = m_origin + FalconHandler.godObjectPosition;
            falconPosition.y = m_origin.y;
            transform.position = falconPosition;

            m_animator.SetFloat(s_hashTurn, falconPosition.z / 3.5f);
        }
    }

    private void LateUpdate()
    {
        m_lastPosition = m_currentPosition;
        m_currentPosition = transform.position;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        if (hasAuthority && NetworkClient.active)
        {
            FalconHandler.ActivateFalcon();

            PlayerCamera.roamerCamera.transform.parent = m_pointOfView;
            PlayerCamera.roamerCamera.transform.localPosition = Vector3.zero;
            PlayerCamera.roamerCamera.transform.localEulerAngles = Vector3.zero;

            m_animator.SetBool("Ready", true);

            if (GalaxyNetworkManager.localConfiguration.isHuman)
            {
                transform.eulerAngles = new Vector3(0, -90, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 90, 0);
            }
        }
    }

    public CapsuleCollider capsuleCollider
    {
        get { return m_capsuleCollider; }
    }

    public float velocity
    {
        get { return (m_lastPosition - m_currentPosition).magnitude; }
    }

    public float strenght
    {
        get { return m_strenght; }
    }
}
