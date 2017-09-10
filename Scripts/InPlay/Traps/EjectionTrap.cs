using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EjectionTrap : Trap
{
    protected override void OnTrapTriggered(Collider other, Snooper trapedSnooper)
    {
        trapedSnooper.EjectSnooper(transform);
    }
}
