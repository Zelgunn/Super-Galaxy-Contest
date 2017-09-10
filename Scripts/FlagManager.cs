using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class FlagManager : NetworkBehaviour
{
    static public FlagManager s_singleton;

    [SerializeField] private Flag m_flag;
    [SerializeField] private float m_respawnDelay = 3.0f;
    [SerializeField] private Transform[] m_spawns;

    [Header("Placeholders")]
    [SerializeField] private GameObject m_humanPlaceholder;
    [SerializeField] private GameObject m_robotPlaceholder;

    private List<Transform> m_remainingSpawns;

    private void Awake()
    {
        s_singleton = this;
        m_remainingSpawns = new List<Transform>();
        foreach(Transform spawn in m_spawns)
        {
            m_remainingSpawns.Add(spawn);
        }
        m_remainingSpawns.Remove(m_spawns[0]);
	}

    [Server]
    public void ResetFlag()
    {
        StartCoroutine(ResetFlagCoroutine());
    }

    private IEnumerator ResetFlagCoroutine()
    {
        m_flag.Detach();
        RpcShowFlag(false);
        m_flag.gameObject.SetActive(false);

        yield return new WaitForSeconds(m_respawnDelay);

        if(m_remainingSpawns.Count == 0)
        {
            foreach(Transform spawn in m_spawns)
            {
                m_remainingSpawns.Add(spawn);
            }
        }

        m_flag.gameObject.SetActive(true);
        RpcShowFlag(true);

        int random = Random.Range(0, m_remainingSpawns.Count);
        Transform selectedSpawn = m_remainingSpawns[random];

        Vector3 safePosition = selectedSpawn.position;
        safePosition.y = 0;
        m_flag.transform.position = safePosition;
        m_remainingSpawns.Remove(selectedSpawn);

        m_flag.ForceUpdatePosition();
    }

    [ClientRpc]
    private void RpcShowFlag(bool show)
    {
        m_flag.gameObject.SetActive(show);
    }

    static public void ShowPlaceholder(bool show)
    {
        s_singleton._ShowPlaceholder(show);
    }

    private void _ShowPlaceholder(bool show)
    {
        if(!NetworkClient.active)
            return;

        show &= (Team.localTeam.isHumanTeam == m_flag.taker.isHuman);

        if(Team.localTeam.isHumanTeam)
        {
            m_humanPlaceholder.SetActive(show);
        }
        else
        {
            m_robotPlaceholder.SetActive(show);
        }
    }

    static public FlagManager singleton
    {
        get { return s_singleton; }
    }
}
