using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    static private PlayerCamera s_roamerCamera;
    static private PlayerCamera s_snooperCamera;

    [SerializeField] private bool m_roamerCamera;
    private Camera m_camera;

    public void Awake ()
    {
        m_camera = GetComponent<Camera>();
	    if(m_roamerCamera)
        {
            s_roamerCamera = this;
            //if (GalaxyNetworkManager.localConfiguration.isHuman)
            //{

            //}
        }
        else
        {
            s_snooperCamera = this;
        }
	}

    static public Camera roamerCamera
    {
        get { return s_roamerCamera.m_camera; }
    }

    static public Camera snooperCamera
    {
        get { return s_snooperCamera.m_camera; }
    }
}
