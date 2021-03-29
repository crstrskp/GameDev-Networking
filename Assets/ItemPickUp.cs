using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public enum ItemTypeDefinitions {  WEAPON, SHIELD, ARMOR };
    public enum ItemArmorSubType { None, Head, Chest, Hands, Legs, Boots };

    public ItemTypeDefinitions ItemType = ItemTypeDefinitions.WEAPON;
    public ItemArmorSubType ItemArmorType = ItemArmorSubType.None;

    public Rigidbody rigidbody;
    
    [SerializeField] private int m_baseDamage;

    public Transform HoldPoint;

    //public void Drop()
    //{
    //    Debug.Log("TODO: [RPC] Drop()");
    //    m_rigidbody.isKinematic = false;
    //    transform.parent = null;
    //}

    //public void Equip(PlayerWeaponHandler playerWeaponHandler)
    //{
    //    Debug.Log("TODO: [RPC] Equip()");

    //    if (transform.parent != null) return;

    //    m_rigidbody.isKinematic = true;
        
    //    if (ItemType == ItemTypeDefinitions.WEAPON)
    //    {
    //        if (playerWeaponHandler.EquippedWeapon == null)
    //        {
    //            playerWeaponHandler.EquippedWeapon = this;
    //            transform.parent = playerWeaponHandler.RightHandTransform;
    //        }
    //        else
    //        {
    //        }
    //    }
    //}
    
    public void ExecuteAttack()
    {
        if (ItemType != ItemTypeDefinitions.WEAPON) return;

        Debug.Log("TODO: [RPC] ExecuteAttack");

        var isCritical = false; // TODO: Determine critical! 

        Debug.Log($"Executing attack from: {gameObject.name}");
        Attack attack = new Attack(m_baseDamage, isCritical);

        Debug.Log("TODO apply attack to target!");
    }

    //public void ThrowItem()
    //{
    //    Debug.Log("TODO: [RPC]ThrowItem()");
    //    Debug.Log("TODO: Throw: Addforce, ApplyDamage OnCollision, unequip");
    //}
}
