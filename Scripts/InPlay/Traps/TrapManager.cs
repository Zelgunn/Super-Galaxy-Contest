using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class TrapManager : NetworkBehaviour
{
    static private TrapManager s_singleton;
    static private bool s_spawningActivated = false;

    [Header("Ejection Trap")]
    [SerializeField] private EjectionTrap m_ejectionTrapPrefab;
    [SerializeField] private int m_maxEjectionTrapsCount = 5;
    [SerializeField] private float m_ejectionStrenght;
    [SerializeField] private float m_ejectionRange;
    [SerializeField] private float m_ejectionYAxisDelta = 5f;
    [SerializeField] private float m_maxEjectionTrapDuration = 10;

    private List<EjectionTrap> m_ejectionTraps;
    private Dictionary<EjectionTrap, int> m_ejectionTrapsTileIDs;

    [Header("Inversion Trap")]
    [SerializeField] private InversionTrap m_inversionTrapPrefab;
    [SerializeField] private int m_maxInversionTrapsCount = 5;
    [SerializeField] private float m_maxInversionTrapDuration = 10;
    [SerializeField] private float m_inversionDuration = 3;

    private List<InversionTrap> m_inversionTraps;
    private Dictionary<InversionTrap, int> m_inversionTrapsTileIDs;

    private void Awake()
    {
        s_singleton = this;
        m_ejectionTraps = new List<EjectionTrap>();
        m_ejectionTrapsTileIDs = new Dictionary<EjectionTrap, int>();

        m_inversionTraps = new List<InversionTrap>();
        m_inversionTrapsTileIDs = new Dictionary<InversionTrap, int>();
    }

    private void Update()
    {
        if(!isServer || !s_spawningActivated)
        {
            return;
        }

        UpdateEjectionTraps();
        UpdateInversionTraps();
    }

    private void UpdateEjectionTraps()
    {
        for (int i = 0; i < m_ejectionTraps.Count; i++)
        {
            UpdateEjectionTrap(i);
        }

        if (m_ejectionTraps.Count < m_maxEjectionTrapsCount)
        {
            SpawnEjectionTrap();
        }
    }

    private void UpdateInversionTraps()
    {
        for (int i = 0; i < m_inversionTraps.Count; i++)
        {
            UpdateInversionTrap(i);
        }

        if (m_inversionTraps.Count < m_maxInversionTrapsCount)
        {
            SpawnInversionTrap();
        }
    }

    #region Ejection traps
    [Server]
    private void SpawnEjectionTrap()
    {
        int tileID = 0;

        do
        {
            tileID = Random.Range(0, transform.childCount);
        }
        while (m_ejectionTrapsTileIDs.ContainsValue(tileID));

        RpcSpawnEjectionTrap(tileID);
        _SpawnEjectionTrap(tileID);
    }

    [ClientRpc]
    private void RpcSpawnEjectionTrap(int tileID)
    {
        _SpawnEjectionTrap(tileID);
    }

    private void _SpawnEjectionTrap(int tileID)
    {
        EjectionTrap ejectionTrap = Instantiate(m_ejectionTrapPrefab) as EjectionTrap;
        ejectionTrap.SetTileID(tileID);

        m_ejectionTraps.Add(ejectionTrap);
        m_ejectionTrapsTileIDs.Add(ejectionTrap, tileID);
    }

    [Server]
    private void UpdateEjectionTrap(int trapID)
    {
        Trap trap = m_ejectionTraps[trapID];

        if (trap.timeLived > m_maxEjectionTrapDuration)
        {
            RpcRemoveEjectionTrap(trapID);
            _RemoveEjectionTrap(trapID);
        }
    }

    private void _RemoveEjectionTrap(int trapID)
    {
        EjectionTrap trap = m_ejectionTraps[trapID];

        Destroy(trap.gameObject, 1);
        trap.enabled = false;
        m_ejectionTraps.Remove(trap);
        m_ejectionTrapsTileIDs.Remove(trap);
    }

    #region EjectionTrap Statics parameters
    static public float ejectionStrenght
    {
        get { return s_singleton.m_ejectionStrenght; }
    }

    static public float ejectionRange
    {
        get { return s_singleton.m_ejectionRange; }
    }

    static public float ejectionYAxisDelta
    {
        get { return s_singleton.m_ejectionYAxisDelta; }
    }
    #endregion


    [ClientRpc]
    private void RpcRemoveEjectionTrap(int trapID)
    {
        _RemoveEjectionTrap(trapID);
    }

    #endregion

    #region Inversion traps
    [Server]
    private void SpawnInversionTrap()
    {
        int tileID = 0;

        do
        {
            tileID = Random.Range(0, transform.childCount);
        }
        while (m_inversionTrapsTileIDs.ContainsValue(tileID));

        RpcSpawnInversionTrap(tileID);
        _SpawnInversionTrap(tileID);
    }

    [ClientRpc]
    private void RpcSpawnInversionTrap(int tileID)
    {
        _SpawnInversionTrap(tileID);
    }

    private void _SpawnInversionTrap(int tileID)
    {
        InversionTrap inversionTrap = Instantiate(m_inversionTrapPrefab) as InversionTrap;
        inversionTrap.SetTileID(tileID);

        m_inversionTraps.Add(inversionTrap);
        m_inversionTrapsTileIDs.Add(inversionTrap, tileID);
    }

    [Server]
    private void UpdateInversionTrap(int trapID)
    {
        Trap trap = m_inversionTraps[trapID];

        if (trap.timeLived > m_maxInversionTrapDuration)
        {
            RpcRemoveInversionTrap(trapID);
            _RemoveInversionTrap(trapID);
        }
    }

    private void _RemoveInversionTrap(int trapID)
    {
        InversionTrap trap = m_inversionTraps[trapID];

        Destroy(trap.gameObject, 1);
        trap.enabled = false;
        m_inversionTraps.Remove(trap);
        m_inversionTrapsTileIDs.Remove(trap);
    }

    [ClientRpc]
    private void RpcRemoveInversionTrap(int trapID)
    {
        _RemoveInversionTrap(trapID);
    }

    #endregion

    static public void RemoveTrap(Trap trap)
    {
        // Ejection traps
        int trapID = s_singleton.m_ejectionTraps.IndexOf(trap as EjectionTrap);
        if(trapID >= 0)
        {
            s_singleton.RpcRemoveEjectionTrap(trapID);
            s_singleton._RemoveEjectionTrap(trapID);
        }

        // Inversion traps
        trapID = s_singleton.m_inversionTraps.IndexOf(trap as InversionTrap);
        if (trapID >= 0)
        {
            s_singleton.RpcRemoveInversionTrap(trapID);
            s_singleton._RemoveInversionTrap(trapID);
        }
    }

    static public float inversionDuration
    {
        get { return s_singleton.m_inversionDuration; }
    }

    static public Vector3 TilePosition(int tileID)
    {
        return s_singleton.transform.GetChild(tileID).position;
    }

    static public void StartSpawningTrap()
    {
        s_spawningActivated = true;
    }
}
