using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

public class GalaxyConfiguration
{
    private bool m_isHuman = false;
    private bool m_isServer = false;

    // Stats Bleus
    private int m_blueTotalWins = 0;
    private int m_blueTotalGoals = 0;
    private int m_blueTotalFlagsCaptured = 0;
    private int m_blueTotalPowersUsed = 0;
    private int m_blueTotalTrapsTaken = 0;

    // Stats Rouges
    private int m_redTotalWins = 0;
    private int m_redTotalGoals = 0;
    private int m_redTotalFlagsCaptured = 0;
    private int m_redTotalPowersUsed = 0;
    private int m_redTotalTrapsTaken = 0;

    private string m_serverMode;
    private string m_factionMode;

    public GalaxyConfiguration()
    {

    }

    public GalaxyConfiguration(bool isServer, bool isHuman)
    {
        m_isHuman = isHuman;
        m_isServer = isServer;
    }

    public void LoadConfiguration()
    {
        if(!File.Exists("galaxy.cfg"))
        {
            Debug.LogWarning("Pas de fichier de configuration...");
            return;
        }

        StreamReader streamReader = new StreamReader("galaxy.cfg", Encoding.UTF8);
        string line = "";
        using(streamReader)
        {
            while(line != null)
            {
                line = streamReader.ReadLine();
                if(line == null)
                {
                    break;
                }

                string[] values = line.Split('=');

                if(values[0] == "Serveur")
                {
                    m_serverMode = line;
                    if (values[1].StartsWith("O"))
                    {
                        m_isServer = true;
                    }
                    else
                    {
                        m_isServer = false;
                    }
                }
                else if(values[0] == "Humain")
                {
                    m_factionMode = line;
                    if (values[1].StartsWith("O"))
                    {
                        m_isHuman = true;
                    }
                    else
                    {
                        m_isHuman = false;
                    }
                }
                else if (values[0] == "Victoires")
                {
                    m_blueTotalWins = Int32.Parse(values[1]);
                    m_redTotalWins = Int32.Parse(values[2]);
                }
                else if (values[0] == "Buts")
                {
                    m_blueTotalGoals = Int32.Parse(values[1]);
                    m_redTotalGoals = Int32.Parse(values[2]);
                }
                else if (values[0] == "Drapeaux")
                {
                    m_blueTotalFlagsCaptured = Int32.Parse(values[1]);
                    m_redTotalFlagsCaptured = Int32.Parse(values[2]);
                }
                else if (values[0] == "Pouvoirs")
                {
                    m_blueTotalPowersUsed = Int32.Parse(values[1]);
                    m_redTotalPowersUsed = Int32.Parse(values[2]);
                }
                else if (values[0] == "Pieges")
                {
                    m_blueTotalTrapsTaken = Int32.Parse(values[1]);
                    m_redTotalTrapsTaken = Int32.Parse(values[2]);
                }
            }
        }

        streamReader.Close();
    }

    public void SaveConfiguration()
    {
        if (Team.humanTeam == null || Team.robotTeam == null)
            return;

        StreamWriter streamWriter = new StreamWriter("galaxy.cfg", false, Encoding.UTF8);
        using (streamWriter)
        {
            streamWriter.WriteLine(m_serverMode);
            streamWriter.WriteLine(m_factionMode);

            streamWriter.WriteLine("Victoires=" + Team.humanTeam.totalWins          + "=" + Team.robotTeam.totalWins);
            streamWriter.WriteLine("Buts="      + Team.humanTeam.totalGoals         + "=" + Team.robotTeam.totalGoals);
            streamWriter.WriteLine("Drapeaux="  + Team.humanTeam.totalFlagsCaptured + "=" + Team.robotTeam.totalFlagsCaptured);
            streamWriter.WriteLine("Pouvoirs="  + Team.humanTeam.totalPowersUsed    + "=" + Team.robotTeam.totalPowersUsed);
            streamWriter.WriteLine("Pieges="    + Team.humanTeam.totalTrapsTaken    + "=" + Team.robotTeam.totalTrapsTaken);
        }
    }

    private string OuiNon(bool yes)
    {
        if (yes)
            return "Oui";
        else
            return "Non";
    }

    public bool isHuman
    {
        get { return m_isHuman; }
    }

    public bool isRobot
    {
        get { return !m_isHuman; }
    }

    public bool isServer
    {
        get { return m_isServer; }
    }

    public bool isClient
    {
        get { return !m_isServer; }
    }

    // Stats Bleus
    public int blueTotalWins
    {
        get { return m_blueTotalWins; }
    }
    public int blueTotalGoals
    {
        get { return m_blueTotalGoals; }
    }
    public int blueTotalFlagsCaptured
    {
        get { return m_blueTotalFlagsCaptured; }
    }
    public int blueTotalPowersUsed
    {
        get { return m_blueTotalPowersUsed; }
    }
    public int blueTotalTrapsTaken
    {
        get { return m_blueTotalTrapsTaken; }
    }

    // Stats Rouges
    public int redTotalWins
    {
        get { return m_redTotalWins; }
    }
    public int redTotalGoals
    {
        get { return m_redTotalGoals; }
    }
    public int redTotalFlagsCaptured
    {
        get { return m_redTotalFlagsCaptured; }
    }
    public int redTotalPowersUsed
    {
        get { return m_redTotalPowersUsed; }
    }
    public int redTotalTrapsTaken
    {
        get { return m_redTotalTrapsTaken; }
    }
}
