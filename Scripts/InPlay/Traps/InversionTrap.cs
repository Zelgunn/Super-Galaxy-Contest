using UnityEngine;
using System.Collections;

public class InversionTrap : Trap
{
    protected override void OnTrapTriggered(Collider other, Snooper trapedSnooper)
    {
        trapedSnooper.InvertControls();
    }
}
