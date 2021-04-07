using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHandler : MonoBehaviourPun
{
    private ExitGames.Client.Photon.Hashtable PlayerCustomProps;
    public void OnJoinedRoom()
    {
        PlayerCustomProps = new ExitGames.Client.Photon.Hashtable();
        PlayerCustomProps["Name"] = photonView.Owner.NickName;
        PlayerCustomProps["Kills"] = 0;
        PlayerCustomProps["Death"] = 0;
        PhotonNetwork.SetPlayerCustomProperties(PlayerCustomProps);
    }

    private void TestMethod()
    {
        PhotonNetwork
    }
}
