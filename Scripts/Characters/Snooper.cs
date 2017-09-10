using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using XInputDotNetPure;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CapsuleCollider))]
public class Snooper : Character
{
    static private Snooper s_humanSnooper;
    static private Snooper s_robotSnooper;

    [SerializeField] private Transform m_flagAttach;

    [Header("Animator Data")]
    [SerializeField] protected float m_speed = 1.0f;
    [SerializeField] protected float m_coefSpeed = 10f;
    [SerializeField] protected bool m_useCurves;

    [Header("Sounds")]
    [SerializeField] private AudioClip m_jumpAudioClip;
    [SerializeField] private AudioClip m_tackleAudioClip;

    private float m_scaleSpeed = 0.4f;
    private float m_powerSpeed = 0.5f;

    protected Animator m_animator;
    protected AnimatorStateInfo m_currentBaseState;
    private AudioSource m_audioSource;
    private CapsuleCollider m_capsuleCollider;

    protected bool m_playerIndexSet = false;
    protected PlayerIndex m_playerIndex;
    protected GamePadState m_state;
    protected GamePadState m_prevState;

    protected float m_valuestickleft;
    protected float m_valuetriggerright;
    protected float m_valuetriggerleft;
    private bool m_controlsInverted = false;
    private bool m_isTackling = false;
    private bool m_isJumping = false;

    static protected int IdleState = Animator.StringToHash("Base Layer.Idle");
    static protected int RunFState = Animator.StringToHash("Base Layer.RunningFwd");
    static protected int RunBState = Animator.StringToHash("Base Layer.RunningBck");
    static protected int TackleState = Animator.StringToHash("Base Layer.Tackle");
    static protected int TackleIdleState = Animator.StringToHash("Base Layer.TackleIdle");
    static protected int FightState = Animator.StringToHash("Base Layer.Fight");
    static protected int JumpState = Animator.StringToHash("Base Layer.Jump");
    static protected int JumpInPlaceState = Animator.StringToHash("Base Layer.JumpInPlace");
    static protected int FallState = Animator.StringToHash("Base Layer.Fall");
    static protected int FallIdleState = Animator.StringToHash("Base Layer.FallIdle");
    static protected int FallBckState = Animator.StringToHash("Base Layer.FallBck");
    static protected int PickState = Animator.StringToHash("Base Layer.Pick");
    static protected int PickBckState = Animator.StringToHash("Base Layer.PickBck");

    static protected int s_hashSpeed = Animator.StringToHash("Speed");
    static protected int s_hashJumpInPlace = Animator.StringToHash("JumpInPlace");
    static protected int s_hashTackleIdle = Animator.StringToHash("TackleIdle");
    static protected int s_hashFight = Animator.StringToHash("Fight");
    static protected int s_hashFallIdle = Animator.StringToHash("FallIdle");
    static protected int s_hashJump = Animator.StringToHash("Jump");
    static protected int s_hashTackle = Animator.StringToHash("Tackle");
    static protected int s_hashPick = Animator.StringToHash("Pick");
    static protected int s_hashFall = Animator.StringToHash("Fall");
    static protected int s_hashPickBck = Animator.StringToHash("PickBck");
    static protected int s_hashFallBck = Animator.StringToHash("FallBck");

    override protected void Awake()
    {
        base.Awake();
        m_animator = GetComponent<Animator>();
        m_audioSource = GetComponent<AudioSource>();
        m_capsuleCollider = GetComponent<CapsuleCollider>();

        if (isHuman)
        {
            s_humanSnooper = this;
        }
        else
        {
            s_robotSnooper = this;
        }
    }

    override protected void Start()
    {
        if (m_animator.layerCount == 2)
            m_animator.SetLayerWeight(1, 1);
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        if (hasAuthority && NetworkClient.active)
        {
            FalconHandler.ActivateFalcon();

            PlayerCamera.snooperCamera.transform.parent = m_pointOfView;
            PlayerCamera.snooperCamera.transform.localPosition = Vector3.zero;
            PlayerCamera.snooperCamera.transform.localEulerAngles = Vector3.zero;
        }
    }

    public void EjectSnooper(Transform ejectionTrapTransform)
    {
        m_rigidbody.AddExplosionForce(
            TrapManager.ejectionStrenght,
            ejectionTrapTransform.position,
            TrapManager.ejectionRange,
            TrapManager.ejectionYAxisDelta,
            ForceMode.Impulse
            );

        m_animator.SetBool("Fall", true);
        m_animator.applyRootMotion = false;

        if(hasAuthority && NetworkClient.active)
            CmdEjectSnooper();
    }

    [Command]
    private void CmdEjectSnooper()
    {
        if (Flag.singleton.taker == this)
        {
            Flag.singleton.Detach();
        }
    }

    public void InvertControls()
    {
        StartCoroutine(InvertControlCoroutine());
    }

    private IEnumerator InvertControlCoroutine()
    {
        m_controlsInverted = true;

        yield return new WaitForSeconds(TrapManager.inversionDuration);

        m_controlsInverted = false;
    }

    public void PickFlag()
    {
        m_animator.SetBool(s_hashPick, true);
    }

    public Transform flagAttach
    {
        get { return m_flagAttach; }
    }

    private void FixedUpdate()
    {
        if (!hasAuthority || !NetworkClient.active)
            return;

        if (!m_playerIndexSet || !m_prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    m_playerIndex = testPlayerIndex;
                    m_playerIndexSet = true;
                }
            }
        }

        m_prevState = m_state;
        m_state = GamePad.GetState(m_playerIndex);

        #region Speed
        Vector2 tmp = new Vector2(
            Mathf.Max(m_state.ThumbSticks.Left.Y, 0),
            Mathf.Max(m_state.ThumbSticks.Left.X, 0)
            );

        m_valuestickleft = tmp.magnitude;
        if(isHuman)
        {
            m_powerSpeed = Team.humanTeam.snooperSpeed;
        }
        else
        {
            m_powerSpeed = Team.robotTeam.snooperSpeed;
        }
        if (!RoundManager.snoopersCanMove) m_powerSpeed = 0;
        m_animator.SetFloat(s_hashSpeed, m_valuestickleft);
        #endregion

        #region Status Icon Update
        Color iconColor = m_statusIconTarget.color;
        if (m_powerSpeed < 0.5f)
        {
            iconColor.a = (.5f - m_powerSpeed) * 2;
            m_statusIconTarget.texture = GameData.malusStatusIcon;
        }
        else
        {
            iconColor.a = (m_powerSpeed - 0.5f) * 2;
            m_statusIconTarget.texture = GameData.bonusStatusIcon;
        }
        m_statusIconTarget.color = iconColor;
        #endregion

        m_valuetriggerright = m_state.Triggers.Right;
        m_valuetriggerleft = m_state.Triggers.Left;

        #region Rotation
        int inversion = 1;
        if (m_controlsInverted) inversion = -1;

        transform.localRotation *= Quaternion.Euler(0.0f, m_state.ThumbSticks.Left.X * 200.0f * Time.deltaTime * inversion, 0.0f);
        #endregion
        m_currentBaseState = m_animator.GetCurrentAnimatorStateInfo(0);

        //GamePad.SetVibration(m_playerIndex, m_state.Triggers.Left, m_state.Triggers.Right);

        m_animator.speed = m_speed;
        //transform.Translate((Vector3.forward * m_state.ThumbSticks.Left.Y) * speed / m_coefSpeed);

        UpdateAnimator();
        m_isJumping = (m_currentBaseState.fullPathHash == JumpState) || (m_currentBaseState.fullPathHash == JumpInPlaceState);
        UpdateTackling();
    }

    private void UpdateAnimator()
    {
        #region IdleState
        if (m_currentBaseState.fullPathHash == IdleState)
        {
            if (m_valuetriggerright >= 0.75f)
            {
                if (m_currentBaseState.fullPathHash != JumpInPlaceState)
                {
                    m_audioSource.PlayOneShot(m_jumpAudioClip);
                }
                m_animator.SetBool(s_hashJumpInPlace, true);
            }
            else if (m_valuetriggerleft >= 0.75f)
            {
                if (m_currentBaseState.fullPathHash != TackleIdleState)
                {
                    m_audioSource.PlayOneShot(m_tackleAudioClip);
                }
                m_animator.SetBool(s_hashTackleIdle, true);
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                m_animator.SetBool(s_hashFight, true);
            }
            else if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                m_animator.SetBool(s_hashFallIdle, true);
            }
        }
        #endregion
        #region JumpInPlace
        else if (m_currentBaseState.fullPathHash == JumpInPlaceState)
        {
            if (m_useCurves)
            {
                //col.height = m_animator.GetFloat("ColliderHeight");
                //m_animator.gravityWeight = 0.0f;
                //m_animator.SetLayerWeight(1, 0);
                //float GravityWeight = 0.0f;
                float o = m_animator.GetLayerWeight(1);
                float a = m_animator.gravityWeight;
                Debug.Log("o" + o);
                Debug.Log("a" + a);
            }

            if (!m_animator.IsInTransition(0))
            {
                m_animator.SetBool(s_hashJumpInPlace, false);
            }
        }
        #endregion
        #region RunForward
        else if (m_currentBaseState.fullPathHash == RunFState)
        {
            transform.Translate((Vector3.forward * m_valuestickleft) * m_speed / m_coefSpeed * 2 * m_scaleSpeed * m_powerSpeed);

            if (m_valuetriggerright >= 0.75f)
            {
                if(m_currentBaseState.fullPathHash != JumpState)
                {
                    m_audioSource.PlayOneShot(m_jumpAudioClip);
                }
                m_animator.SetBool(s_hashJump, true);
            }
            else if (m_valuetriggerleft >= 0.75f)
            {
                if (m_currentBaseState.fullPathHash != TackleState)
                {
                    m_audioSource.PlayOneShot(m_tackleAudioClip);
                }
                m_animator.SetBool(s_hashTackle, true);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                m_animator.SetBool(s_hashPick, true);
            }
            else if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                m_animator.SetBool(s_hashFall, true);
            }
        }
        #endregion
        #region Run Backward
        else if (m_currentBaseState.fullPathHash == RunBState)
        {
            transform.Translate((Vector3.forward * m_valuestickleft) * m_speed / m_coefSpeed * 2 * m_scaleSpeed * m_powerSpeed);

            if (Input.GetKeyDown(KeyCode.Space))
                m_animator.SetBool(s_hashPickBck, true);

            else if (Input.GetKeyDown(KeyCode.KeypadEnter))
                m_animator.SetBool(s_hashFallBck, true);
        }
        #endregion
        #region Tackle (running)
        else if (m_currentBaseState.fullPathHash == TackleState)
        {
            m_animator.applyRootMotion = true;

            if (!m_animator.IsInTransition(0))
            {
                m_animator.SetBool(s_hashTackle, false);
            }
        }
        #endregion
        #region Tackle (idle)
        else if (m_currentBaseState.fullPathHash == TackleIdleState)
        {
            m_animator.applyRootMotion = true;

            if (!m_animator.IsInTransition(0))
            {
                m_animator.SetBool(s_hashTackleIdle, false);
            }
        }
        #endregion
        #region Jump (running)
        else if (m_currentBaseState.fullPathHash == JumpState)
        {
            m_animator.applyRootMotion = true;

            transform.Translate((Vector3.forward * m_valuestickleft) * m_speed / m_coefSpeed * 2 * m_scaleSpeed * m_powerSpeed);

            if (!m_animator.IsInTransition(0))
            {
                m_animator.SetBool(s_hashJump, false);
            }
        }
        #endregion
        #region Pick (running)
        else if (m_currentBaseState.fullPathHash == PickState)
        {
            m_animator.applyRootMotion = true;

            if (!m_animator.IsInTransition(0))
            {
                m_animator.SetBool(s_hashPick, false);
            }
        }
        #endregion
        #region Pick (backward)
        else if (m_currentBaseState.fullPathHash == PickBckState)
        {
            m_animator.applyRootMotion = true;

            if (!m_animator.IsInTransition(0))
            {
                m_animator.SetBool(s_hashPickBck, false);
            }
        }
        #endregion
        #region Fall (running)
        else if (m_currentBaseState.fullPathHash == FallState)
        {
            m_animator.applyRootMotion = false;
            if (!m_animator.IsInTransition(0))
            {
                m_animator.SetBool(s_hashFall, false);
            }
        }
        #endregion
        #region Fall (idle)
        else if (m_currentBaseState.fullPathHash == FallIdleState)
        {
            m_animator.applyRootMotion = false;
            if (!m_animator.IsInTransition(0))
            {
                m_animator.SetBool(s_hashFallIdle, false);
            }
        }
        #endregion
        #region Fall (backward)
        else if (m_currentBaseState.fullPathHash == FallBckState)
        {
            if (!m_animator.IsInTransition(0))
            {
                m_animator.SetBool(s_hashFallBck, false);
            }
        }
        #endregion
        #region Fight
        else if (m_currentBaseState.fullPathHash == FightState)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                m_animator.SetBool(s_hashFight, false);
        }
        #endregion

    }

    private void OnCollisionEnter(Collision other)
    {
        Ball ball = other.gameObject.GetComponent<Ball>();

        if ((ball == null) || !NetworkServer.active)
            return;

        MakeFall();
    }

    private void UpdateTackling()
    {
        m_isTackling = (m_currentBaseState.fullPathHash == TackleState) || ((m_currentBaseState.fullPathHash == TackleIdleState));

        if ((!m_isTackling) || (!hasAuthority))
            return;

        Snooper otherSnooper;
        if(isHuman)
        {
            otherSnooper = s_robotSnooper;
        }
        else
        {
            otherSnooper = s_humanSnooper;
        }

        float minimalDistanceForTacling = m_capsuleCollider.radius + otherSnooper.capsuleCollider.radius;
        //minimalDistanceForTacling *= 1.1f;

        if((Vector3.Distance(transform.position, otherSnooper.transform.position) < minimalDistanceForTacling) &&
                (!otherSnooper.isTackling))
        {
            CmdTackle();
        }
    }

    [Command]
    private void CmdTackle()
    {
        Snooper otherSnooper;
        if (isHuman)
        {
            otherSnooper = s_robotSnooper;
        }
        else
        {
            otherSnooper = s_humanSnooper;
        }

        otherSnooper.MakeFall();
    }

    [Server]
    private void MakeFall()
    {
        RpcMakeFall();
        _MakeFall();
        if(Flag.singleton.taker == this)
        {
            Flag.singleton.Detach();
        }
    }

    [ClientRpc]
    private void RpcMakeFall()
    {
        _MakeFall();
    }

    private void _MakeFall()
    {
        if ((m_currentBaseState.fullPathHash == IdleState) || (m_currentBaseState.fullPathHash == JumpInPlaceState))
        {
            m_animator.SetBool(s_hashFallIdle, true);
        }
        else
        {
            m_animator.SetBool(s_hashFall, true);
        }

        m_animator.SetFloat(s_hashSpeed, 0);
    }

    public bool isTackling
    {
        get { return m_isTackling; }
    }

    public bool isJumping
    {
        get { return m_isJumping; }
    }

    public CapsuleCollider capsuleCollider
    {
        get { return m_capsuleCollider; }
    }

    static public Snooper humanSnooper
    {
        get { return s_humanSnooper; }
    }

    static public Snooper robotSnooper
    {
        get { return s_robotSnooper; }
    }
}
