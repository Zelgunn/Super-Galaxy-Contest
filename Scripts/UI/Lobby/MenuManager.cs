using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    static private MenuManager s_singleton;

    [Header("Menus")]
    [SerializeField] private Canvas m_connectingMenu;
    [SerializeField] private LobbyServerMenuManager m_serverUI;
    [SerializeField] private LobbyClientMenuManager m_roamerUI;
    [SerializeField] private LobbyClientMenuManager m_snooperUI;

    private void Awake()
    {
        s_singleton = this;
        ShowConnectingMenu();
    }

    private void ShowSingleUI(GameObject shownUI, GameObject uiGameobject, bool hideIfDifferent)
    {
        shownUI.SetActive((shownUI == uiGameobject) || (!hideIfDifferent && shownUI.activeInHierarchy));
    }

    private void ShowUI(GameObject uiGameobject, bool hideIfDifferent = true)
    {
        ShowSingleUI(m_connectingMenu.gameObject, uiGameobject, hideIfDifferent);
        ShowSingleUI(m_serverUI.gameObject, uiGameobject, hideIfDifferent);
        ShowSingleUI(m_roamerUI.gameObject, uiGameobject, hideIfDifferent);
        ShowSingleUI(m_snooperUI.gameObject, uiGameobject, hideIfDifferent);
    }

    //static public void ShowLobby()
    //{
    //    ShowRoamerUI(m_)
    //    s_singleton.m_lobbyMenu.gameObject.SetActive(true);
    //    s_singleton.m_connectingMenu.gameObject.SetActive(false);
    //    LobbyMenuManager.Initialize();
    //}

    static public void ShowConnectingMenu()
    {
        s_singleton.ShowUI(s_singleton.m_connectingMenu.gameObject);
    }

    static public void ShowServerUI()
    {
        s_singleton.ShowUI(s_singleton.m_serverUI.gameObject);
        s_singleton.m_serverUI.UpdatePlayersPreview();
    }

    static public void ShowPlayerUI()
    {
        s_singleton.ShowUI(s_singleton.m_snooperUI.gameObject);
        s_singleton.ShowUI(s_singleton.m_roamerUI.gameObject, false);
    }

    static public void UpdateLobbyMenu()
    {
        s_singleton._UpdateLobbyMenu();
    }

    private void _UpdateLobbyMenu()
    {
        if (NetworkClient.active)
        {
            m_roamerUI.UpdateSelectors();
            m_snooperUI.UpdateSelectors();
        }
        else
        {
            m_serverUI.UpdatePlayersPreview();
        }
    }

    // 1 Show Connecting
    // 2 Show Server / Roamer / Snooper UI
}
