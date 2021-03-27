using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAttackInput : MonoBehaviourPun
{
    public event Action StrafeStart;
    public event Action StrafeEnd;
    public event Action Attack;

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            StrafeStart?.Invoke();
        }

        if (Input.GetMouseButtonUp(1))
        {
            StrafeEnd?.Invoke();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Attack?.Invoke();
        }
    }
}
