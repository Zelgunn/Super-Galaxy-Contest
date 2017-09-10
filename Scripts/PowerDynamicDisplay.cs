using UnityEngine;
using System.Collections.Generic;

public class PowerDynamicDisplay : MonoBehaviour
{
    private float m_angle = 0;
    private List<GameObject> m_powersDisplays;

    [SerializeField] private GameObject m_powerDisplayPrefab;
    [SerializeField] private bool m_isHuman;

	private void Start ()
    {
        m_powersDisplays = new List<GameObject>();
    }

    private void Update ()
    {
        UpdateAngle();

        UpdateDisplaysCount();

        UpdateDisplaysRotation();
    }

    private void UpdateAngle()
    {
        m_angle += Time.deltaTime * 120.0f;
        if (m_angle > 360)
        {
            m_angle -= 360;
        }
    }

    private void UpdateDisplaysCount()
    {
        int powersToDisplayCount;

        if (m_isHuman)
            powersToDisplayCount = Team.humanTeam.powersAvailable;
        else
            powersToDisplayCount = Team.robotTeam.powersAvailable;

        if(powersToDisplayCount > m_powersDisplays.Count)
        {
            GameObject powerDisplay = Instantiate(m_powerDisplayPrefab);
            m_powersDisplays.Add(powerDisplay);

            powerDisplay.transform.SetParent(transform);
            powerDisplay.transform.localPosition = Vector3.zero;
            powerDisplay.transform.localEulerAngles = Vector3.zero;
        }
        else if(powersToDisplayCount < m_powersDisplays.Count)
        {
            GameObject powerDisplay = m_powersDisplays[0];
            m_powersDisplays.RemoveAt(0);
            Destroy(powerDisplay);
        }
    }

    private void UpdateDisplaysRotation()
    {
        if (m_powersDisplays.Count == 0) return;

        float angleBetweenDisplays = 360.0f / m_powersDisplays.Count;
        float tmp = m_angle;

        foreach (GameObject powerDisplay in m_powersDisplays)
        {
            powerDisplay.transform.localEulerAngles = new Vector3(0, tmp, 0);

            tmp += angleBetweenDisplays;
            if (tmp > 360)
                tmp -= 360;
        }
    }
}
