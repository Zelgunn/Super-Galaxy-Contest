using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class Goal : NetworkBehaviour
{
    [SerializeField] private bool m_isHumanGoal = true;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != RoundManager.ball)
        {
            return;
        }

        if(m_isHumanGoal)
        {
            Team.robotTeam.Goal();
        }
        else
        {
            Team.humanTeam.Goal();
        }
    }
}
