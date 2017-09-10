using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private Text m_humanScoreDisplay;
    [SerializeField] private Text m_robotScoreDisplay;
	
	private void Update ()
    {
        if((Team.humanTeam == null) || (Team.robotTeam == null))
        {
            m_humanScoreDisplay.text = "0";
            m_robotScoreDisplay.text = "0";
            return;
        }

	    m_humanScoreDisplay.text = Team.humanTeam.score.ToString();
        m_robotScoreDisplay.text = Team.robotTeam.score.ToString();
	}
}
