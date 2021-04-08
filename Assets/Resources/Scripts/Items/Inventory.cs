using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, IDestructible
{
    public LayerMask PickUpLayerMask = new LayerMask();

    [SerializeField] private float m_pickUpRadius;
    [SerializeField] private PlayerAttackInput m_playerAttackInput;
    [SerializeField] private PlayerWeaponHandler m_playerWeaponHandler;
    [SerializeField] private PhotonView m_photonView;

    [SerializeField] private Transform LeftHandTransform;
    [SerializeField] private Transform RightHandTransform;

    public ItemPickUp[] WeaponSlots = new ItemPickUp[6];
    public ItemPickUp[] ShieldSlot = null;

    private void OnEnable()
    {
        m_playerAttackInput.WeaponSelect += TrySetEquipped;
        m_playerAttackInput.DropEquipped += TryDropEquipped;
        m_playerAttackInput.PickUp += TryPickUp;
    }

    private void OnDestroy()
    {
        m_playerAttackInput.WeaponSelect -= TrySetEquipped;
        m_playerAttackInput.DropEquipped -= TryDropEquipped;
        m_playerAttackInput.PickUp -= TryPickUp;
    }

    #region Equip Functionality

    private void TrySetEquipped(int weaponSlot)
    {
        if (WeaponSlots[weaponSlot - 1] == null) return;

        if (WeaponSlots[weaponSlot - 1] == m_playerWeaponHandler.EquippedWeapon) return;

        m_playerWeaponHandler.EquippedWeapon = WeaponSlots[weaponSlot - 1];
    }

    private void TryAutoEquip()
    {
        throw new NotImplementedException();
    }

    #endregion
    #region Pick Up Functionality

    private void TryPickUp()
    {
        var itemsWithinPickUpRadius = Physics.OverlapSphere(transform.position, m_pickUpRadius, PickUpLayerMask);

        var closestDist = m_pickUpRadius;
        Collider closestItem = null;
        foreach (Collider c in itemsWithinPickUpRadius)
        {
            var dist = (c.transform.position - transform.position).magnitude;
            if (dist < closestDist)
            {
                closestItem = c;
                closestDist = dist;
            }
        }

        var item = closestItem.transform.GetComponent<ItemPickUp>();
        var itemID = item.GetComponent<PhotonView>().ViewID;

        if (item.ItemType == ItemPickUp.ItemTypeDefinitions.WEAPON)
        {
            // Already something equppied on that weaponSlot.
            if (WeaponSlots[item.WeaponSlotKey] != null) return;
            
            m_photonView.RPC("PickUpItem", RpcTarget.All, itemID);
        } 
        else if (item.ItemType == ItemPickUp.ItemTypeDefinitions.SHIELD)
        {
            if (m_playerWeaponHandler.EquippedShield == null)

            if (ShieldSlot != null) return;

            m_photonView.RPC("PickUpItem", RpcTarget.All, itemID);
        }
        else if (item.ItemType == ItemPickUp.ItemTypeDefinitions.ARMOR)
        {
            throw new NotImplementedException();

            m_photonView.RPC("PickUpItem", RpcTarget.All, itemID);
        }
    }

    [PunRPC]
    private void PickUpItem(int itemID)
    {
        var item = PhotonView.Find(itemID);

        if (item == null) return;

        // TODO: IS THIS ENOUGH!? 

        var itemPickUp = item.GetComponent<ItemPickUp>();
        itemPickUp.PickUp();
        itemPickUp.Owner = gameObject; // TODO: refactor. SetOwner(PhotonViewID)
    }

    #endregion
    #region Drop functionality
    private void TryDropEquipped()
    {
        if (!m_photonView.IsMine) return;

        m_photonView.RPC("DropEquipped", RpcTarget.All);
    }

    [PunRPC]
    private void DropEquipped()
    {
        if (m_playerWeaponHandler.EquippedWeapon != null)
        {
            m_playerWeaponHandler.EquippedWeapon.Drop();
            m_playerWeaponHandler.EquippedWeapon = null;
        }
        else if (m_playerWeaponHandler.EquippedShield != null)
        {
            m_playerWeaponHandler.EquippedShield.Drop();
            m_playerWeaponHandler.EquippedShield = null;
        }
    }

    [PunRPC]
    private void DropItemsOnDeath()
    {
        if (m_playerWeaponHandler.EquippedWeapon != null) TryDropEquipped();
        if (m_playerWeaponHandler.EquippedShield != null) TryDropEquipped();

        for (int i = 0; i < WeaponSlots.Length; i++)
        {
            if (WeaponSlots[i] != null)
            {
                WeaponSlots[i].Drop();
                WeaponSlots[i] = null;
            }
        }
    }

    public void OnDestruction(GameObject destroyer)
    {
        if (!m_photonView.IsMine) return;

        m_photonView.RPC("DropItemsOnDeath", RpcTarget.All);
    }

    #endregion
}
