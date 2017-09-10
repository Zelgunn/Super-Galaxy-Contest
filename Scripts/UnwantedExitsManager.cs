using UnityEngine;
using System.Collections;

public class UnwantedExitsManager : MonoBehaviour
{
    [SerializeField] private Transform m_humanReplacePoint;
    [SerializeField] private Transform m_robotReplacePoint;

    private void OnTriggerExit(Collider other)
    {
        Snooper snooper = other.gameObject.GetComponent<Snooper>();

        if(snooper == null)
        {
            return;
        }

        snooper.rigidbody.velocity = Vector3.zero;
        if(snooper.isHuman)
        {
            snooper.transform.position = m_humanReplacePoint.transform.position;
        }
        else
        {
            snooper.transform.position = m_robotReplacePoint.transform.position;
        }
    }
}
