using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public enum ItemTypeDefinitions {  WEAPON, SHIELD, ARMOR };
    public enum WeaponSubType { Melee, Ranged, Other }
    public enum ItemArmorSubType { None, Head, Chest, Hands, Legs, Boots };

    public ItemTypeDefinitions ItemType = ItemTypeDefinitions.WEAPON;
    public WeaponSubType WeaponType = WeaponSubType.Melee;
    public ItemArmorSubType ItemArmorType = ItemArmorSubType.None;

    public Rigidbody rigidbody;

    public bool AttackActive = false;

    [SerializeField] private Transform HoldPoint;
    [SerializeField] private int m_baseDamage = 15;
    [SerializeField] private int m_criticalHitChance = 15;
    [SerializeField] private GameObject Owner; 
    
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
    //            Owner = playerWeaponHandler.gameObject;
    //        }
    //        else
    //        {
    //        }
    //    }
    //}

    void OnTriggerEnter(Collider col)
    {
        if (WeaponType != WeaponSubType.Melee) return;
        
        if (AttackActive == false) return;

        var attackable = col.gameObject.GetComponent<IAttackable>();
        
        if (attackable == null) return;

        var isCritical = DetermineCritical();

        var attack = new Attack(m_baseDamage, isCritical);

        Debug.Log($"{Owner.name} attacks {col.gameObject.name}, damage: {attack.Damage}, isCritical: {isCritical}");
        
        attackable.OnAttack(Owner, attack);
    }

    private bool DetermineCritical() => (UnityEngine.Random.Range(0, 100) < m_criticalHitChance);
        

    //public void ThrowItem()
    //{
    //    Debug.Log("TODO: [RPC]ThrowItem()");
    //    Debug.Log("TODO: Throw: Addforce, ApplyDamage OnCollision, unequip");
    //}
}
