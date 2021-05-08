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

    public ItemPickUp[] WeaponSlots = new ItemPickUp[6]; // TODO: HIDEININSPECTOR ONCE DONE TESTING PICKUP / DROP / THROW / EQUIP / ETC
    public ItemPickUp ShieldSlot = null;                 // TODO: HIDEININSPECTOR ONCE DONE TESTING PICKUP / DROP / THROW / EQUIP / ETC

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

        TryEquip(WeaponSlots[weaponSlot - 1]);
    }

    private void TryEquip(ItemPickUp item)
    {
        if (item.ItemType == ItemPickUp.ItemTypeDefinitions.WEAPON)
        {
            if (item.EquipType == ItemPickUp.WeaponEquipType.OneHanded && m_playerWeaponHandler.EquippedWeapon == null)
            {
                Equip(item, RightHandTransform);
                m_playerWeaponHandler.EquippedWeapon = item;
            
                if (ShieldSlot == null) return;

                if (m_playerWeaponHandler.EquippedShield != null) return;

                Equip(ShieldSlot, LeftHandTransform);
                m_playerWeaponHandler.EquippedShield = item;
            }
            else if (item.EquipType == ItemPickUp.WeaponEquipType.TwoHanded)
            {
                if (m_playerWeaponHandler.EquippedWeapon != null) return;
                if (m_playerWeaponHandler.EquippedShield != null) return;
            
                Equip(item, RightHandTransform);
                m_playerWeaponHandler.EquippedWeapon = item;
            }
            else if (item.EquipType == ItemPickUp.WeaponEquipType.Bow)
            {
                if (m_playerWeaponHandler.EquippedWeapon != null) return;
                if (m_playerWeaponHandler.EquippedShield != null) return;
            
                Equip(item, LeftHandTransform);
                m_playerWeaponHandler.EquippedWeapon = item;
            }
        }
        else if (item.ItemType == ItemPickUp.ItemTypeDefinitions.SHIELD)
        {
            if (m_playerWeaponHandler.EquippedShield != null) return;
            
            if (m_playerWeaponHandler.EquippedWeapon != null 
             && m_playerWeaponHandler.EquippedWeapon.EquipType != ItemPickUp.WeaponEquipType.OneHanded) return;

            Equip(item, LeftHandTransform);
            m_playerWeaponHandler.EquippedShield = item;
        }
        else if (item.ItemType == ItemPickUp.ItemTypeDefinitions.ARMOR)
        {
            var parentTransform = GetTransformFromArmorType(item.ItemArmorType);

            // CHECK IF SOMETHING ALREADY EQUIPPED
            Equip(item, parentTransform);
        }
    }

    private Transform GetTransformFromArmorType(ItemPickUp.ItemArmorSubType itemArmorType)
    {
        throw new NotImplementedException();
    }

    private void Equip(ItemPickUp item, Transform parentTransform)
    {
        item.transform.SetParent(parentTransform);

        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    #endregion
    #region Pick Up Functionality

    public void TryPickUp()
    {
        if (!m_photonView.IsMine) return;

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

        if (closestItem == null) return;

        var item = closestItem.transform.GetComponent<ItemPickUp>();
        var itemID = item.GetComponent<PhotonView>().ViewID;

        if (item.ItemType == ItemPickUp.ItemTypeDefinitions.WEAPON)
        {
            // Already something equppied on that weaponSlot.
            if (WeaponSlots[item.WeaponSlotKey] != null) return;
            var equip = false;
            if (m_playerWeaponHandler.EquippedWeapon == null)
                equip = true;

            m_photonView.RPC("PickUpItem", RpcTarget.All, itemID, equip);
        } 
        else if (item.ItemType == ItemPickUp.ItemTypeDefinitions.SHIELD)
        {
            if (m_playerWeaponHandler.EquippedShield == null)

            if (ShieldSlot != null) return;
            m_photonView.RPC("PickUpItem", RpcTarget.All, itemID, false);

        }
        else if (item.ItemType == ItemPickUp.ItemTypeDefinitions.ARMOR)
        {
            throw new NotImplementedException();

            m_photonView.RPC("PickUpItem", RpcTarget.All, itemID, true);
        }
    }

    [PunRPC]
    private void PickUpItem(int itemID, bool autoEquip)
    {
        var item = PhotonView.Find(itemID);

        if (item == null) return;

        var itemPickUp = item.GetComponent<ItemPickUp>();

        itemPickUp.PickUp(owner: gameObject);
        TryEquip(itemPickUp);
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
