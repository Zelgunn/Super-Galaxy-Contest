using UnityEngine;
using UnityEngine.Networking;

public class FlagMarker : NetworkBehaviour
{
    [SerializeField] private bool m_isHuman;

    private void OnTriggerEnter(Collider other)
    {
        Snooper snooper = other.GetComponent<Snooper>();

        if (snooper == null)
            return;

        if((snooper == Flag.singleton.taker) && (snooper.isHuman == m_isHuman))
        {
            if (snooper.isHuman)
            {
                Team.humanTeam.MarkFlag();
            }
            else
            {
                Team.robotTeam.MarkFlag();
            }

            FlagManager.singleton.ResetFlag();
        }
    }
}
