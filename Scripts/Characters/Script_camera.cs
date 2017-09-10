using UnityEngine;
using System.Collections;

public class Script_camera : MonoBehaviour {

    public float smooth = 3f;
    Transform standardPos;

	void Start ()
    {
        standardPos = GameObject.Find("CamPos").transform;
	}

	void Update ()
    {
        transform.position = Vector3.Lerp(transform.position, standardPos.position, Time.deltaTime * smooth);
        transform.forward = Vector3.Lerp(transform.forward, standardPos.forward, Time.deltaTime * smooth);
    }
}
