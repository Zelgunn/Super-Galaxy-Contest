using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class GalaxyDiscovery : NetworkDiscovery
{
    new public void Initialize()
    {
        GalaxyNetworkManager.singleton.networkAddress = Network.player.ipAddress;
        broadcastData = "NetworkManager:" + GalaxyNetworkManager.singleton.networkAddress + ":" + GalaxyNetworkManager.singleton.networkPort;

        if (!base.Initialize())
            return;
	}

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if (GalaxyNetworkManager.singleton == null || GalaxyNetworkManager.singleton.client != null)
        {
            Debug.LogWarning("Impossible de recevoir des données : pas de LobbyManager ou un client a déjà été démarré.");
            return;
        }

        // Data :
        // 0) "NetworkManager"
        // 1) Adresse réseau du serveur
        // 2) Port réseau utilisé par le serveur
        // 3) A voir (Pseudo de l'hôte, type de partie, config...)
        string[] splittedDatas = data.Split(':');

        if (splittedDatas.Length != 3)
            return;

        if (splittedDatas[0] != "NetworkManager")
            return;

        int port;
        bool success = Int32.TryParse(splittedDatas[2], out port);

        if (!success)
        {
            Debug.LogWarning("Démarrage du client impossible : \"" + data + "\" ne contient pas de numéro de port, ou les champs ne sont pas bien séparés par des \":\"");
            return;
        }

        //StopBroadcast();
        GalaxyNetworkManager.singleton.StartLobbyClient(splittedDatas[1], port);
    }
}
