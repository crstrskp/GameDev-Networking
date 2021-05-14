using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHandler : MonoBehaviourPun
{
    public ExitGames.Client.Photon.Hashtable PlayerCustomProps = new ExitGames.Client.Photon.Hashtable();
    public void Start()
    {
        var localPlayer = PlayerHandler.LocalPlayerInstance;

        if (localPlayer == null) return;

        var view = localPlayer.GetComponent<PhotonView>();
        if (!view.IsMine) return; 
        
        var playerStats = PlayerHandler.LocalPlayerInstance.GetComponent<PlayerScore>();

        PlayerCustomProps.Add("Kills", playerStats.GetKills());
        PlayerCustomProps.Add("CreepKills", playerStats.GetCreepKills());
        PlayerCustomProps.Add("Deaths", playerStats.GetDeaths());

        PhotonNetwork.SetPlayerCustomProperties(PlayerCustomProps);
    }

}
