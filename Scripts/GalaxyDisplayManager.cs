using UnityEngine;
using System.Collections;

public class GalaxyDisplayManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
	}
}
