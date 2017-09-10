using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class FinalScore : MonoBehaviour
{
    static private FinalScore s_singleton;
    static private FinalScore s_snooperSingleton;

    [SerializeField] private bool m_snooperUI = false;

    [Header("Winning team display")]
    [SerializeField] private Text m_scoreDisplay;
    [SerializeField] private RawImage m_winningTeamFrame;
    [SerializeField] private Texture m_humansWinFrame;
    [SerializeField] private Texture m_robotsWinFrame;
    [SerializeField] private Texture m_equalityFrame;

    [Header("Summary Table")]
    [SerializeField] private RawImage m_summaryTableFrame;
    [Header("Human Side")]
    [SerializeField] private Text m_humanVictoriesDisplay;
    [SerializeField] private Text m_humanGoalsDisplay;
    [SerializeField] private Text m_humanFlagsDisplay;
    [SerializeField] private Text m_humanPowersDisplay;
    [SerializeField] private Text m_humanTrapsDisplay;
    [Header("Robot Side")]
    [SerializeField] private Text m_robotVictoriesDisplay;
    [SerializeField] private Text m_robotGoalsDisplay;
    [SerializeField] private Text m_robotFlagsDisplay;
    [SerializeField] private Text m_robotPowersDisplay;
    [SerializeField] private Text m_robotTrapsDisplay;

    private void Awake()
    {
        if (m_snooperUI)
        {
            s_snooperSingleton = this;
        }
        else
        {
            s_singleton = this;
        }

        m_winningTeamFrame.gameObject.SetActive(false);
        m_summaryTableFrame.gameObject.SetActive(false);
	}

    private void _ShowWinningTeam()
    {
        m_winningTeamFrame.gameObject.SetActive(true);
        m_summaryTableFrame.gameObject.SetActive(false);
        ServerUI.Hide();
        ClientUI.Hide();

        int humanScore =  Team.humanTeam.score;
        int robotScore = Team.robotTeam.score;

        string displayedScore = "";

        if (humanScore > robotScore)
        {
            m_winningTeamFrame.texture = m_humansWinFrame;
            displayedScore = humanScore.ToString() + " - " + robotScore.ToString();
        }
        else if (robotScore > humanScore)
        {
            m_winningTeamFrame.texture = m_robotsWinFrame;
            displayedScore = robotScore.ToString() + " - " + humanScore.ToString();
        }
        else
        {
            m_winningTeamFrame.texture = m_equalityFrame;
            displayedScore = robotScore.ToString() + " - " + humanScore.ToString();
        }

        m_scoreDisplay.text = displayedScore;
    }

    static public void ShowWinningTeam()
    {
        s_singleton._ShowWinningTeam();
        if (NetworkClient.active)
        {
            s_snooperSingleton._ShowWinningTeam();
        }
    }

    private void _ShowSummaryTable()
    {
        m_winningTeamFrame.gameObject.SetActive(false);
        m_summaryTableFrame.gameObject.SetActive(true);

        Debug.Log("H Stats : ");
        m_humanVictoriesDisplay.text = Team.humanTeam.totalWins.ToString();
        Debug.Log("H Vic: " + Team.humanTeam.totalGoals.ToString());
        m_humanGoalsDisplay.text = Team.humanTeam.totalGoals.ToString();
        Debug.Log("H But: " + Team.humanTeam.totalGoals.ToString());
        m_humanFlagsDisplay.text = Team.humanTeam.totalFlagsCaptured.ToString();
        Debug.Log("H Drap: " + Team.humanTeam.totalFlagsCaptured.ToString());
        m_humanPowersDisplay.text = Team.humanTeam.totalPowersUsed.ToString();
        Debug.Log("H Pouv: " + Team.humanTeam.totalPowersUsed.ToString());
        m_humanTrapsDisplay.text = Team.humanTeam.totalTrapsTaken.ToString();
        Debug.Log("H Pieges: " + Team.humanTeam.totalTrapsTaken.ToString());

        Debug.Log("R Stats : ");
        m_robotVictoriesDisplay.text = Team.robotTeam.totalWins.ToString();
        Debug.Log("R Vic: " + Team.humanTeam.totalGoals.ToString());
        m_robotGoalsDisplay.text = Team.robotTeam.totalGoals.ToString();
        Debug.Log("R But: " + Team.humanTeam.totalGoals.ToString());
        m_robotFlagsDisplay.text = Team.robotTeam.totalFlagsCaptured.ToString();
        Debug.Log("R Drap: " + Team.humanTeam.totalFlagsCaptured.ToString());
        m_robotPowersDisplay.text = Team.robotTeam.totalPowersUsed.ToString();
        Debug.Log("R Pouv: " + Team.humanTeam.totalPowersUsed.ToString());
        m_robotTrapsDisplay.text = Team.robotTeam.totalTrapsTaken.ToString();
        Debug.Log("R Pieges: " + Team.humanTeam.totalTrapsTaken.ToString());
    }

    static public void ShowSummaryTable()
    {
        s_singleton._ShowSummaryTable();
        if (NetworkClient.active)
        {
            s_snooperSingleton._ShowSummaryTable();
        }
    }
}
