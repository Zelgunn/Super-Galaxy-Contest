using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class DevLog
{
    static private bool s_fileAlreadyOpened = false;
    static private int s_checkPoint = 0;
    static private Dictionary<string, int> s_checkDictionnary = new Dictionary<string, int>();

    static public void Log(string str)
    {
        str += "\n";

        if (NetworkServer.active)
        {
            Debug.Log(str);
            return;
        }

        if (s_fileAlreadyOpened)
        {
            //System.IO.File.AppendAllText(@"C:\Users\degva_000\Documents\Unity\GalaxyContest\Galaxy.log", str);
        }
        else
        {
            s_fileAlreadyOpened = true;
            //System.IO.File.WriteAllText(@"C:\Users\degva_000\Documents\Unity\GalaxyContest\Galaxy.log", str);
        }
    }

    static public void LogCheckPoint(string str = "")
    {
        if(str != "")
        {
            if(s_checkDictionnary.ContainsKey(str))
            {
                s_checkDictionnary[str]++;
            }
            else
            {
                s_checkDictionnary.Add(str, 0);
            }
            str = "Checkpoint(" + str + ") " + (s_checkDictionnary[str]).ToString() + "\n";
        }
        else
        {
            str = "Checkpoint " + (s_checkPoint++).ToString() + "\n";
        }

        if (NetworkServer.active)
        {
            Debug.Log(str);
            return;
        }

        if (s_fileAlreadyOpened)
        {
            //System.IO.File.AppendAllText(@"C:\Users\degva_000\Documents\Unity\GalaxyContest\Galaxy.log", str);
        }
        else
        {
            s_fileAlreadyOpened = true;
            //System.IO.File.WriteAllText(@"C:\Users\degva_000\Documents\Unity\GalaxyContest\Galaxy.log", str);
        }
    }
}
