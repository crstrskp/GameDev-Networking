using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{

    [SerializeField] private PlayerAttackInput m_playerAttackInput;
    [SerializeField] private Health m_health;

    public Transform LeftHandTransform;
    public Transform RightHandTransform;
    public ItemPickUp EquippedWeapon;
    public ItemPickUp EquippedShield;

    public ItemPickUp[] WeaponSlots = new ItemPickUp[6];

    [SerializeField] private AnimationHandler m_animHandler;


    private void Awake()
    {
        m_animHandler.Attack += Attack;

        m_playerAttackInput.WeaponSelect += TryChangeWeapon;
        m_playerAttackInput.DropEquipped += DropEquipped;
        m_playerAttackInput.ThrowEquipped += ThrowItem;

        m_health.OnDeath += DropItemsOnDeath;
    }

    private void OnDestroy()
    {
        m_animHandler.Attack -= Attack;
        
        m_playerAttackInput.WeaponSelect -= TryChangeWeapon;
        m_playerAttackInput.DropEquipped -= DropEquipped;
        m_playerAttackInput.ThrowEquipped -= ThrowItem;

        m_health.OnDeath -= DropItemsOnDeath;
    }

    private void TryChangeWeapon(int weaponSlot)
    {
        if (WeaponSlots[weaponSlot - 1] != null)
        {
            if (WeaponSlots[weaponSlot - 1] == EquippedWeapon) return;


            EquippedWeapon = WeaponSlots[weaponSlot - 1];
        }
    }

    private void DropEquipped()
    {
        if (EquippedWeapon != null)
        {
            //EquippedWeapon.Drop();
            EquippedWeapon.rigidbody.isKinematic = false;
            EquippedWeapon.transform.parent = null;
            EquippedWeapon = null;
        }
        else if (EquippedShield != null)
        {
            //EquippedShield.Drop();
            EquippedShield.rigidbody.isKinematic = false;
            EquippedShield.transform.parent = null;
            EquippedShield = null;
        }
    }

    private void DropItemsOnDeath()
    {
        if (EquippedWeapon != null) DropEquipped();
        if (EquippedShield != null) DropEquipped();

        for (int i = 0; i < WeaponSlots.Length; i++)
        {
            if (WeaponSlots[i] != null)
            {
                WeaponSlots[i].transform.parent = null;
                WeaponSlots[i].rigidbody.isKinematic = false;
                WeaponSlots[i] = null;
            }
        }
    }

    private void Attack()
    {
        Debug.Log("Attacking from PlayerHandler");

        if (EquippedWeapon == null) return;
        EquippedWeapon.ExecuteAttack();
    }


    private void ThrowItem()
    {
        Debug.Log("ThrowItem()");

        if (EquippedWeapon == null) return;

        // Add force
        // add flag to item ? beingThrown, so it will damage target

    }

    private void EquipItem()
    {
        Debug.Log("EquipItem()");
    }

    private void PickUpItem()
    {
        Debug.Log("PickUpItem");
        // auto equip? 
        // equip
        // else
        // populate weaponSlot

    }
}
