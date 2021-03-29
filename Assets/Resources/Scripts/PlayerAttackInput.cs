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

    public event Action<int> WeaponSelect;

    public event Action PickUp;
    public event Action DropEquipped;
    public event Action ThrowEquipped;

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

        #region AlphaNumericInputKeys

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            WeaponSelect?.Invoke(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            WeaponSelect?.Invoke(2);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            WeaponSelect?.Invoke(3);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            WeaponSelect?.Invoke(4);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            WeaponSelect?.Invoke(5);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            WeaponSelect?.Invoke(6);
        }

        #endregion

        if (Input.GetKeyDown(KeyCode.G))
        {
            DropEquipped?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUp?.Invoke();
        }

        if (Input.GetMouseButtonUp(2))
        {
            ThrowEquipped?.Invoke();
        }
    }
}
