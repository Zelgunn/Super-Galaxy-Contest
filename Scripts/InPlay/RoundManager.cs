using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class RoundManager : NetworkBehaviour
{
    static private RoundManager s_singleton;

    [SerializeField] private GameObject m_ball;
    [SerializeField] private Transform m_humanBallSpawn;
    [SerializeField] private Transform m_robotBallSpawn;
    [SerializeField] private FlagManager m_flagManager;

    private bool m_redistributeBallNow = false;
    private bool m_snooperCanMove = false;

    private void Awake()
    {
        s_singleton = this;
	}

    [Server]
    private void ServerStartRound()
    {
        StartCoroutine(StartServerRound());
    }

    // Routine principale
    [Server]
    private IEnumerator StartServerRound()
    {
        // Une demi seconde de latence pour laisser plus de préparation ?
        yield return new WaitForSeconds(1f);

        RpcStartPreparationPhase();
        yield return PreparationPhase();

        yield return MainPhase();

        yield return EndPhase();
        GalaxyNetworkManager.localConfiguration.SaveConfiguration();

        // Pause avant de retourner au lobby
        while (!Input.GetKeyDown(KeyCode.Return))
            yield return null;

        RpcRestartGame();
        yield return new WaitForSeconds(0.1f);
        GalaxyNetworkManager.singleton.StopServer();
        SceneManager.LoadScene(0);
    }

    // Preparation
    [ClientRpc]
    private void RpcStartPreparationPhase()
    {
        StartCoroutine(PreparationPhase());
    }

    private IEnumerator PreparationPhase()
    {
        m_snooperCanMove = false;
        for (int i = 3; i >= 0; i--)
        {
            PreparationCountdown.ShowPreparationNumber(i);
            if(i != 0)
            {
                GalaxyAudioPlayer.PlaySmallBip();
            }
            else
            {
                GalaxyAudioPlayer.PlayLongBip();
            }
            yield return new WaitForSeconds(1.0f);
        }

        PreparationCountdown.ShowPreparationNumber(-1);
        m_snooperCanMove = true;
    }

    // Partie principale
    [Server]
    private IEnumerator MainPhase()
    {
        RpcStartMainPhase(GameData.gameDuration);
        ServerUI.StartTimer(GameData.gameDuration);
        StartCoroutine(BallDistribution());

        while(ServerUI.timerRemainingSeconds > 0)
        {
            yield return null;
        }
    }

    [Server]
    private IEnumerator BallDistribution()
    {
        bool distributeToHuman = GalaxyNetworkManager.humansHaveBallFirst;
        float timeTmp;
        while (ServerUI.timerRemainingSeconds > 0)
        {
            timeTmp = Time.time;
            m_redistributeBallNow = false;

            if (distributeToHuman)
            {
                Ball.singleton.GiveToPlayer(distributeToHuman, m_humanBallSpawn.position);
            }
            else
            {
                Ball.singleton.GiveToPlayer(distributeToHuman, m_robotBallSpawn.position);
            }
            
            distributeToHuman = !distributeToHuman;

            while(((Time.time - timeTmp) < 10) && !m_redistributeBallNow)
                yield return null;
        }
    }

    static public void RedistributeBallNow()
    {
        s_singleton.m_redistributeBallNow = true;
    }

    [ClientRpc]
    private void RpcStartMainPhase(int seconds)
    {
        ClientUI.StartTimer(seconds);
    }

    // Fin de round
    [Server]
    private IEnumerator EndPhase()
    {
        Rigidbody ballRigidbody = m_ball.GetComponent<Rigidbody>();

        ballRigidbody.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ |
            RigidbodyConstraints.FreezePositionX |
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezePositionZ;

        ballRigidbody.isKinematic = true;

        RpcEndPhase();
        yield return _EndPhase();
    }

    [ClientRpc]
    private void RpcEndPhase()
    {
        StartCoroutine(_EndPhase());
    }

    private IEnumerator _EndPhase()
    {
        FinalScore.ShowWinningTeam();

        yield return new WaitForSeconds(1);

        if(isServer)
        {
            GalaxyAudioPlayer.PlayEndAmbiance();
            if (Team.humanTeam.score > Team.robotTeam.score) Team.humanTeam.IncreaseVictoryCount();
            else if (Team.robotTeam.score > Team.humanTeam.score) Team.robotTeam.IncreaseVictoryCount();
            Team.humanTeam.ResetScore();
            Team.robotTeam.ResetScore();
        }

        yield return new WaitForSeconds(2);

        FinalScore.ShowSummaryTable();
    }

    [ClientRpc]
    private void RpcRestartGame()
    {
        FalconUnity.Stop();
        FalconUnity.disconnect();

        SceneManager.LoadScene(0);
    } 

    static public void StartRound()
    {
        if(!NetworkServer.active)
        {
            return;
        }

        if(s_singleton == null)
        {
            Debug.LogWarning("Pas de RoundManager...");
        }

        s_singleton.ServerStartRound();
    }

    static public GameObject ball
    {
        get { return s_singleton.m_ball; }
    }

    static public bool snoopersCanMove
    {
        get { return s_singleton.m_snooperCanMove; }
    }
}
