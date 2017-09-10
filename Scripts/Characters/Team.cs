using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using XInputDotNetPure;

public class Team : NetworkBehaviour
{
    static private Team s_humanTeam;
    static private Team s_robotTeam;
    static private Team s_localTeam;

    [SerializeField] private bool m_humanTeam = true;

    private Character m_roamer;
    private Character m_snooper;

    private bool m_roamerReady = false;
    private bool m_snooperReady = false;

    private int m_powersAvailable = 0;
    private float m_speed = 0.5f;
    private float m_strenght = 0.5f;
    private float m_speedRegeneration = 0.05f;
    private float m_strenghtRegeneration = 0.05f;

    private int m_score = 0;
    private int m_totalScore = 0;

    protected bool m_playerIndexSet = false;
    protected PlayerIndex m_playerIndex;
    protected GamePadState m_state;
    protected GamePadState m_prevState;

    // Stats
    private int m_totalWins = 0;
    private int m_totalFlagsCaptured = 0;
    private int m_totalPowersUsed = 0;
    private int m_totalTrapsTaken = 0;

    public void Start()
    {
        if (isLocalPlayer)
        {
            s_localTeam = this;
            CmdSetIsHuman(GalaxyNetworkManager.localConfiguration.isHuman, true);
        }
    }

    private void Update()
    {
        if(isServer)
        {
            if(GalaxyNetworkManager.playSceneShown)
            {
                UpdatePowerAndStrenght();
            }

            if(Input.GetKeyDown(KeyCode.O))
            {
                MarkFlag();
            }
            return;
        }

        if(isLocalPlayer)
        {
            if(!GalaxyNetworkManager.playSceneShown)
            {
                #region Input Gamepad Controller
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

                if ((m_state.Buttons.A == ButtonState.Pressed) && (m_prevState.Buttons.A == ButtonState.Released))
                {
                    SetSnooperReady();
                }
                #endregion

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SetRoamerReady();
                }

                if (Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    SetSnooperReady();
                }
            }
        }
    }

    private void UpdateIsHuman()
    {
        CmdSetIsHuman(m_humanTeam, false);
        CmdSetRoamerReady(m_roamerReady);
        CmdSetSnooperReady(m_snooperReady);
    }

    [Command]
    private void CmdSetIsHuman(bool isHuman, bool askForUpdate)
    {
        RpcSetIsHuman(isHuman, askForUpdate);
        _SetIsHuman(isHuman);
        LoadStats();
    }

    [ClientRpc]
    private void RpcSetIsHuman(bool isHuman, bool askForUpdate)
    {
        _SetIsHuman(isHuman);

        if(askForUpdate && NetworkClient.active && !isLocalPlayer)
        {
            Team otherTeam;
            if(isHuman)
            {
                otherTeam = robotTeam;
            }
            else
            {
                otherTeam = humanTeam;
            }

            if(otherTeam != null)
            {
                otherTeam.UpdateIsHuman();                
            }
        }
    }

    private void _SetIsHuman(bool isHuman)
    {
        m_humanTeam = isHuman;

        if (m_humanTeam)
        {
            s_humanTeam = this;
        }
        else
        {
            s_robotTeam = this;
        }

        MenuManager.UpdateLobbyMenu();
    }

    [Server]
    public void CheckAllPlayersAreReady()
    {
        if ((humanTeam == null) || (Team.robotTeam == null))
        {
            return;
        }

        if (!humanTeam.isRoamerReady || !humanTeam.isSnooperReady)
            return;

        if (!robotTeam.isRoamerReady || !robotTeam.isSnooperReady)
            return;

        RpcLoadPlayScene();
        GalaxyNetworkManager.singleton.ShowPlayScene();
    }

    [ClientRpc]
    private void RpcLoadPlayScene()
    {
        GalaxyNetworkManager.singleton.ShowPlayScene();
    }

    //private void 

    #region Set Ready (roamer & snooper)
    public void SetRoamerReady()
    {
        if (LobbyClientMenuManager.isReadyAnimationInProgressForRoamer) return;

        if (!m_roamerReady) GalaxyAudioPlayer.PlayReadySong();
        LobbyClientMenuManager.SetRoamerReady(!m_roamerReady);
        CmdSetRoamerReady(!m_roamerReady);
    }

    public void SetSnooperReady()
    {
        if (LobbyClientMenuManager.isReadyAnimationInProgressForSnooper) return;

        if (!m_snooperReady) GalaxyAudioPlayer.PlayReadySong();
        LobbyClientMenuManager.SetSnooperReady(!m_snooperReady);
        CmdSetSnooperReady(!m_snooperReady);
    }

    [Command]
    private void CmdSetRoamerReady(bool ready)
    {
        if(ready) GalaxyAudioPlayer.PlayReadySong();
        RpcSetRoamerReady(ready);
        _SetRoamerReady(ready);
        CheckAllPlayersAreReady();
    }

    [ClientRpc]
    private void RpcSetRoamerReady(bool ready)
    {
        _SetRoamerReady(ready);
    }

    private void _SetRoamerReady(bool ready)
    {
        if (ready) GalaxyAudioPlayer.PlayReadySong();
        m_roamerReady = ready;
        MenuManager.UpdateLobbyMenu();
    }

    [Command]
    private void CmdSetSnooperReady(bool ready)
    {
        RpcSetSnooperReady(ready);
        _SetSnooperReady(ready);
        CheckAllPlayersAreReady();
    }

    [ClientRpc]
    private void RpcSetSnooperReady(bool ready)
    {
        _SetSnooperReady(ready);
    }

    private void _SetSnooperReady(bool ready)
    {
        m_snooperReady = ready;
        MenuManager.UpdateLobbyMenu();
    }
    #endregion

    #region Update Power and Strenght
    [Server]
    private void UpdatePowerAndStrenght()
    {
        // Vitesse du Fureteur
        float speedRegen = m_speedRegeneration * Time.deltaTime;
        float strenghtRegen = m_strenghtRegeneration * Time.deltaTime;

        if (m_speed > (0.5f + speedRegen))
        {
            m_speed -= speedRegen;
        }
        else if (m_speed < (0.5f - speedRegen))
        {
            m_speed += speedRegen;
        }
        else
        {
            m_speed = 0.5f;
        }

        // Force du Bourlingueur
        if (m_strenght > (0.5f + strenghtRegen))
        {
            m_strenght -= strenghtRegen;
        }
        else if (m_strenght < (0.5f - strenghtRegen))
        {
            m_strenght += strenghtRegen;
        }
        else
        {
            m_strenght = 0.5f;
        }

        RpcUpdateSpeedAndStrenght(m_speed, m_strenght);
    }

    [ClientRpc]
    private void RpcUpdateSpeedAndStrenght(float speed, float strenght)
    {
        m_speed = speed;
        m_strenght = strenght;
    }
    #endregion

    #region Set Strenght
    [Server]
    public void SetStrenght(float strenght)
    {
        m_strenght = strenght;
        RpcSetStrenght(strenght);
    }

    [ClientRpc]
    private void RpcSetStrenght(float strenght)
    {
        m_strenght = strenght;
    }
    #endregion

    #region Set Speed
    [Server]
    public void SetSpeed(float speed)
    {
        m_speed = speed;
        RpcSetSpeed(speed);
    }

    [ClientRpc]
    private void RpcSetSpeed(float speed)
    {
        m_speed = speed;
    }
    #endregion

    #region Goal
    [Server]
    public void Goal()
    {
        m_score +=5;
        RoundManager.RedistributeBallNow();
        RpcGoal(m_score);
        _Goal(m_score);
        if (m_humanTeam)
        {
            GoalManager.SetBlueState(true);
        }
        else
        {
            GoalManager.SetRedState(true);
        }
    }

    [ClientRpc]
    private void RpcGoal(int score)
    {
        _Goal(score);
    }

    private void _Goal(int score)
    {
        m_score = score;
        GoalUI.ShowGoalUI();
        GalaxyAudioPlayer.PlayGoalSound();
        if(m_humanTeam)
        {
            Fireworks.UseBlueFireworks();
        }
        else
        {
            Fireworks.UseRedFireworks();
        }
    }
    #endregion

    #region ResetAndCountScore
    [Server]
    public void ResetScore()
    {
        m_totalScore += m_score;
        m_score = 0;

        RpcResetScore(m_totalScore);
    }

    [ClientRpc]
    private void RpcResetScore(int totalScore)
    {
        m_totalScore = totalScore;
        m_score = 0;
    }
    #endregion

    #region Statistics
    #region Load
    [Server]
    private void LoadStats()
    {
        if (m_humanTeam)
        {
            m_totalWins = GalaxyNetworkManager.localConfiguration.blueTotalWins;
            m_totalScore = GalaxyNetworkManager.localConfiguration.blueTotalGoals;
            m_totalFlagsCaptured = GalaxyNetworkManager.localConfiguration.blueTotalFlagsCaptured;
            m_totalPowersUsed = GalaxyNetworkManager.localConfiguration.blueTotalPowersUsed;
            m_totalTrapsTaken = GalaxyNetworkManager.localConfiguration.blueTotalTrapsTaken;
        }
        else
        {
            m_totalWins = GalaxyNetworkManager.localConfiguration.redTotalWins;
            m_totalScore = GalaxyNetworkManager.localConfiguration.redTotalGoals;
            m_totalFlagsCaptured = GalaxyNetworkManager.localConfiguration.redTotalFlagsCaptured;
            m_totalPowersUsed = GalaxyNetworkManager.localConfiguration.redTotalPowersUsed;
            m_totalTrapsTaken = GalaxyNetworkManager.localConfiguration.redTotalTrapsTaken;
        }

        RpcLoadStats(m_totalWins, m_totalScore, m_totalFlagsCaptured, m_totalPowersUsed, m_totalTrapsTaken);
    }

    [ClientRpc]
    private void RpcLoadStats(int totalWins, int totalScore, int totalFlagsCaptured, int totalPowersUsed, int totalTrapsTaken)
    {
        m_totalWins = totalWins;
        m_totalScore = totalScore;
        m_totalFlagsCaptured = totalFlagsCaptured;
        m_totalPowersUsed = totalPowersUsed;
        m_totalTrapsTaken = totalTrapsTaken;
    }
    #endregion

    #region VictoryCount
    [Server]
    public void IncreaseVictoryCount()
    {
        m_totalWins++;
        RpcSetTotalWins(m_totalWins);
    }

    [ClientRpc]
    private void RpcSetTotalWins(int totalWins)
    {
        m_totalWins = totalWins;
    }
    #endregion

    #region PowersUsed
    // Powers Used
    [Server]
    public void IncreasePowersUsedCount()
    {
        m_totalPowersUsed++;
        RpcIncreasePowersUsedCount(m_totalPowersUsed);
    }

    [ClientRpc]
    private void RpcIncreasePowersUsedCount(int totalPowersUsed)
    {
        m_totalPowersUsed = totalPowersUsed;
    }
    // ## Powers Used END
    #endregion

    #region TrapsTaken
    // Traps Taken
    [Server]
    public void IncreaseTrapsTakenCount()
    {
        m_totalTrapsTaken++;
        RpcIncreaseTrapsTakenCount(totalTrapsTaken);
    }

    [ClientRpc]
    private void RpcIncreaseTrapsTakenCount(int totalTrapsTaken)
    {
        m_totalTrapsTaken = totalTrapsTaken;
    }
    // ## Traps Taken END
    #endregion
    #endregion

    #region Powers
    #region Powers General
    private bool CanUseAPower()
    {
        return m_powersAvailable > 0;
    }

    [ClientRpc]
    private void RpcUseAPower(int powersAvailable)
    {
        if (isLocalPlayer)
        {
            GalaxyAudioPlayer.PlayPowerSound();
        }
        _UseAPower(powersAvailable);
    }

    private void _UseAPower(int powersAvailable)
    {
        m_powersAvailable = powersAvailable;
        m_totalPowersUsed++;
    }
    #endregion

    #region SuperStr
    public void UseSuperStrenght()
    {
        if(CanUseAPower())
        {
            CmdUseSuperStrenght();
        }
    }

    [Command]
    private void CmdUseSuperStrenght()
    {
        if(CanUseAPower())
        {
            m_powersAvailable--;

            RpcUseAPower(m_powersAvailable);
            SetStrenght(1);
            _UseAPower(m_powersAvailable);
        }
    }

    #endregion

    #region LowerStr
    public void UseLowerEnnemyStrenght()
    {
        if (CanUseAPower())
        {
            CmdUseLowerEnnemyStrenght();
        }
    }

    [Command]
    public void CmdUseLowerEnnemyStrenght()
    {
        if (CanUseAPower())
        {
            m_powersAvailable--;

            RpcUseAPower(m_powersAvailable);
            if(m_humanTeam)
            {
                s_robotTeam.SetStrenght(0);
            }
            else
            {
                s_humanTeam.SetStrenght(0);
            }
            _UseAPower(m_powersAvailable);
        }
    }
    #endregion

    #region SuperSpeed
    public void UseSuperSpeed()
    {
        if (CanUseAPower())
        {
            CmdUseSuperSpeed();
        }
    }

    [Command]
    public void CmdUseSuperSpeed()
    {
        if (CanUseAPower())
        {
            m_powersAvailable--;

            RpcUseAPower(m_powersAvailable);
            SetSpeed(1);
            _UseAPower(m_powersAvailable);
        }
    }
    #endregion

    #region LowerSpeed
    public void UseLowerEnnemySpeed()
    {
        if (CanUseAPower())
        {
            CmdUseLowerEnnemySpeed();
        }
    }

    [Command]
    public void CmdUseLowerEnnemySpeed()
    {
        if (CanUseAPower())
        {
            m_powersAvailable--;

            RpcUseAPower(m_powersAvailable);
            if (m_humanTeam)
            {
                s_robotTeam.SetSpeed(0);
            }
            else
            {
                s_humanTeam.SetSpeed(0);
            }
            _UseAPower(m_powersAvailable);
        }
    }
    #endregion
    #endregion    

    #region MarkFlag
    // Mark Flag
    [Server]
    public void MarkFlag()
    {
        m_powersAvailable++;
        m_totalFlagsCaptured++;
        m_score++;

        RpcMarkFlag(m_powersAvailable, m_totalFlagsCaptured, m_score);
    }

    [ClientRpc]
    private void RpcMarkFlag(int powersAvailable, int totalFlagsCaptured, int score)
    {
        m_powersAvailable = powersAvailable;
        m_totalFlagsCaptured = totalFlagsCaptured;
        m_score = score;

        if (isLocalPlayer && (m_powersAvailable == 1))
        {
            GalaxyAudioPlayer.PlayPowerAvailableSound();
        }
    }
    // ## Mark Flag END
    #endregion

    #region InitRoamerAndSnooper
    [Command]
    private void CmdSetRoamerAndSnooper(bool humanTeam)
    {
        if (humanTeam)
        {
            m_roamer = CharacterManager.humanRoamer;
            m_snooper = CharacterManager.humanSnooper;
        }
        else
        {
            m_roamer = CharacterManager.robotRoamer;
            m_snooper = CharacterManager.robotSnooper;
        }

        //NetworkServer.SpawnWithClientAuthority(m_roamer.gameObject, connectionToClient));
        //NetworkServer.SpawnWithClientAuthority(m_snooper.gameObject, connectionToClient));
        m_roamer.networkIdentity.AssignClientAuthority(connectionToClient);
        m_snooper.networkIdentity.AssignClientAuthority(connectionToClient);
    }

    private void SetRoamerAndSnooper()
    {
        if(m_humanTeam)
        {
            m_roamer = CharacterManager.humanRoamer;
            m_snooper = CharacterManager.humanSnooper;
        }
        else
        {
            m_roamer = CharacterManager.robotRoamer;
            m_snooper = CharacterManager.robotSnooper;
        }

        if(isLocalPlayer)
        {
            CmdSetRoamerAndSnooper(m_humanTeam);
        }
    }

    static public void SetRoamersAndSnoopers()
    {
        if (CharacterManager.singleton == null)
        {
            return;
        }

        if(s_humanTeam != null)
        {
            s_humanTeam.SetRoamerAndSnooper();
        }

        if(s_robotTeam != null)
        {
            s_robotTeam.SetRoamerAndSnooper();
        }
    }
    #endregion

    #region Getters
    public float roamerStrenght
    {
        get { return m_strenght; }
    }

    public float snooperSpeed
    {
        get { return m_speed; }
    }

    public int score
    {
        get { return m_score; }
    }

    public int totalGoals
    {
        get { return m_totalScore; }
    }

    public int totalWins
    {
        get { return m_totalWins; }
    }

    public int totalFlagsCaptured
    {
        get { return m_totalFlagsCaptured; }
    }

    public int totalPowersUsed
    {
        get { return m_totalPowersUsed; }
    }

    public int totalTrapsTaken
    {
        get { return m_totalTrapsTaken; }
    }

    public bool isRoamerReady
    {
        get { return m_roamerReady; }
    }

    public bool isSnooperReady
    {
        get { return m_snooperReady; }
    }

    public bool isHumanTeam
    {
        get { return m_humanTeam; }
    }

    public Character roamer
    {
        get { return m_roamer; }
    }

    public Character snooper
    {
        get { return m_snooper; }
    }

    public int powersAvailable
    {
        get { return m_powersAvailable; }
    }
    #endregion

    #region StaticGetters
    static public Team humanTeam
    {
        get { return s_humanTeam; }
    }

    static public Team robotTeam
    {
        get { return s_robotTeam; }
    }

    static public Team localTeam
    {
        get { return s_localTeam; }
    }
    #endregion
}
