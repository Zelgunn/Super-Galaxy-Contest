using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LobbyServerMenuManager : MonoBehaviour
{
    [SerializeField] private Transform m_previews;

    [Header("Models preview in lobby")]
    [SerializeField] private Previewer m_roamerTeamAPreviewer;
    [SerializeField] private Previewer m_snooperTeamAPreviewer;
    [SerializeField] private Previewer m_roamerTeamBPreviewer;
    [SerializeField] private Previewer m_snooperTeamBPreviewer;

    public void Initialize()
    {
        m_previews.gameObject.SetActive(true);
        UpdatePlayersPreview();
    }

    private void UpdatePlayerPreview(Previewer previewer, bool human, bool roamer)
    {
        Team selectorTeam = Team.humanTeam;
        if (!human) selectorTeam = Team.robotTeam;

        if (selectorTeam == null)
        {
            previewer.Show(false);
            return;
        }

        previewer.Show(true);
        
        bool ready;
        if (roamer)
            ready = selectorTeam.isRoamerReady;
        else
            ready = selectorTeam.isSnooperReady;

        previewer.SetReady(ready);
    }

    public void UpdatePlayersPreview()
    {
        UpdatePlayerPreview(m_roamerTeamAPreviewer, true, true);
        UpdatePlayerPreview(m_snooperTeamAPreviewer, true, false);
        UpdatePlayerPreview(m_roamerTeamBPreviewer, false, true);
        UpdatePlayerPreview(m_snooperTeamBPreviewer, false, false);
    }
}
