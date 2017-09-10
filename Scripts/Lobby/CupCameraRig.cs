using UnityEngine;
using System.Collections;

public class CupCameraRig : MonoBehaviour
{
	private void Update ()
    {
        Vector3 angle = transform.localEulerAngles;
        angle.y -= Time.deltaTime * 2.5f;
        transform.localEulerAngles = angle;
	}
}
