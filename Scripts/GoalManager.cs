using UnityEngine;
using System.IO.Ports;
using System.Collections;

public class GoalManager : MonoBehaviour
{
    static private GoalManager s_singleton;
	
    private SerialPort port = new SerialPort("COM6", 9600);

    private void Start ()
    {
		s_singleton = this;
        OpenConnection();
    }

    private void OpenConnection()
    {
        if (port != null)
        {
            if (port.IsOpen)
            {
                port.Close();
            }
            else
            {
                port.Open();  // opens the connection
                port.ReadTimeout = 50;  // sets the timeout value before reporting error
            }
        }
        else
        {
            if (port.IsOpen)
            {
                Debug.Log("Port is already open");
            }
            else
            {
                Debug.Log("Port == null");
            }
        }
    }
	
	static public void SetRedState(bool turnedOn)
	{
		if(turnedOn)
        {
            s_singleton.TurnOnRedLed();
        }
        else
        {
            s_singleton.TurnOffRedLed();
        }
	}

    static public void SetBlueState(bool turnedOn)
    {
        if (turnedOn)
        {
            s_singleton.TurnOnBlueLed();
        }
        else
        {
            s_singleton.TurnOffBlueLed();
        }
    }

    private void TurnOnRedLed()
    {
        port.Write("1");
        Invoke("TurnOffRedLed", 5);
    }

    private void TurnOffRedLed()
    {
        port.Write("3");
    }

    private void TurnOnBlueLed()
    {
        port.Write("2");
        Invoke("TurnOffBlueLed", 5);
    }

    private void TurnOffBlueLed()
    {
        port.Write("4");
    }
}
