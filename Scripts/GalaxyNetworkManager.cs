using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(GalaxyDiscovery))]
public class GalaxyNetworkManager : NetworkManager
{
    static private GalaxyNetworkManager s_singleton;

    private GalaxyDiscovery m_lobbyDiscovery;
    private GalaxyConfiguration m_configuration;
    private bool m_humansHaveBallFirst = false;

    [SerializeField] private GalaxyDisplayManager m_galaxyDisplayManager;
    [Header("Lobby")]
    [SerializeField] private GameObject m_lobbyScene;
    [Header("Play")]
    [SerializeField] private GameObject m_playScene;
    [SerializeField] private FalconMain m_falcon;
    [SerializeField] private GameObject m_playersCameras;
    [SerializeField] private Camera[] m_serverCameras;

    private void Awake()
    {
        s_singleton = this;

        m_configuration = new GalaxyConfiguration();
        m_lobbyDiscovery = GetComponent<GalaxyDiscovery>();
    }

    private void Start()
    {
        ShowLobbyScene();

        m_configuration.LoadConfiguration();
        m_lobbyDiscovery.Initialize();

        if(m_configuration.isServer)
        {
            m_falcon.gameObject.SetActive(false);
            StartLobbyServer();
        }
        else
        {
            m_falcon.gameObject.SetActive(true);
            ActivateSecondScreen();
            StartLobbyClientDiscovery();
        }
    }

    private void Update()
    {
        if(NetworkServer.active)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                m_humansHaveBallFirst = false;
            }

            if(Input.GetKeyDown(KeyCode.H))
            {
                m_humansHaveBallFirst = true;
            }
        }
    }

    public void ActivateSecondScreen()
    {
        if (m_galaxyDisplayManager.gameObject.activeInHierarchy)
        {
            return;
        }

        m_galaxyDisplayManager.gameObject.SetActive(true);
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
    }

    public void ShowLobbyScene()
    {
        m_lobbyScene.SetActive(true);
        m_playScene.SetActive(false);

        m_playersCameras.SetActive(false);
        foreach(Camera camera in m_serverCameras)
        {
            camera.gameObject.SetActive(false);
        }
    }

    public void ShowPlayScene()
    {
        m_playScene.SetActive(true);
        m_lobbyScene.SetActive(false);

        m_playersCameras.SetActive(NetworkClient.active);
        foreach (Camera camera in m_serverCameras)
        {
            camera.gameObject.SetActive(NetworkServer.active);
        }
        ClientUI.ShowClientUI(NetworkClient.active);
        SnooperUI.ShowSnooperPlayUI(NetworkClient.active);

        if(NetworkServer.active)
        {
            m_lobbyDiscovery.StopBroadcast();
            ServerUI.ShowServerUI(true);
            RoundManager.StartRound();
            TrapManager.StartSpawningTrap();
            GalaxyAudioPlayer.PlayMainAmbiance();
        }
    }

    public void StartLobbyServer()
    {
        if (m_lobbyDiscovery.StartAsServer() && StartServer())
        {
            MenuManager.ShowServerUI();
            GalaxyAudioPlayer.PlayLobbyAmbiance();
        }
    }

    public void StartLobbyClientDiscovery()
    {
        if(m_lobbyDiscovery.StartAsClient())
        {
            MenuManager.ShowConnectingMenu();
        }
    }

    public void StartLobbyClient(string address, int port)
    {
        networkAddress = address;
        networkPort = port;

        NetworkClient networkClient = StartClient();

        if (networkClient != null)
        {
            MenuManager.ShowPlayerUI();
        }
    }

    new static public GalaxyNetworkManager singleton
    {
        get { return s_singleton; }
    }

    static public GalaxyConfiguration localConfiguration
    {
        get { return s_singleton.m_configuration; }
    }

    static public bool playSceneShown
    {
        get { return s_singleton.m_playScene.activeInHierarchy; }
    }

    static public bool lobbySceneShown
    {
        get { return s_singleton.m_lobbyScene.activeInHierarchy; }
    }

    static public bool humansHaveBallFirst
    {
        get { return s_singleton.m_humansHaveBallFirst; }
    }
}
