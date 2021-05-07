using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public enum ItemTypeDefinitions {  WEAPON, SHIELD, ARMOR, NA };
    public enum WeaponSubType { Melee, Ranged, Other, NA }
    //public enum WeaponEquipType { Sword, Mace, Axe, Sword2H, Mace2H, Axe2H, Bow, Spear }
    public enum WeaponEquipType { OneHanded, TwoHanded, Bow, NA }
    public enum ItemArmorSubType { None, Head, Chest, Hands, Legs, Boots, NA };

    public ItemTypeDefinitions ItemType = ItemTypeDefinitions.WEAPON;
    public WeaponSubType WeaponType = WeaponSubType.Melee;
    public WeaponEquipType EquipType = WeaponEquipType.OneHanded;
    public ItemArmorSubType ItemArmorType = ItemArmorSubType.None;

    public bool Throwable;
    public GameObject Owner;

    [Tooltip("The input number pressed for equipping this item")]
    public int WeaponSlotKey;

    [HideInInspector] public bool AttackActive = false;

    [SerializeField] private int m_baseDamage = 15;
    [SerializeField] private int m_criticalHitChance = 15;

    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private float m_throwForce;

    private bool m_beingThrown = false;

    public void Drop()
    {
        m_rigidbody.isKinematic = false;
        transform.parent = null;
        Owner = null;
        GetComponent<Collider>().isTrigger = false;
        gameObject.layer = LayerMask.NameToLayer("ItemPickUp");
    }

    public void PickUp(GameObject owner)
    {
        gameObject.layer = LayerMask.NameToLayer("OwnedItem");
        m_rigidbody.isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        Owner = owner;
    }

    public void StartThrow()
    {
        m_rigidbody.isKinematic = false;
        transform.parent = null;
        GetComponent<Collider>().isTrigger = false;
        gameObject.layer = LayerMask.NameToLayer("ItemPickUp");


        var dir = Owner.GetComponent<PlayerHandler>().GetModel().transform.forward;
        m_rigidbody.AddForce(dir * m_throwForce, ForceMode.Impulse);

        Debug.LogWarning("FIX Torque on throw");
        m_rigidbody.AddRelativeTorque(-transform.forward * 2000, ForceMode.Impulse);

        transform.up = -transform.up;
        
        m_beingThrown = true;
    }

    /// <summary>
    /// Used for melee attack hits
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter(Collider col)
    {
        if (WeaponType != WeaponSubType.Melee) return;

        if (AttackActive == false) return;
            
        ApplyMeleeAttack(col.gameObject, m_baseDamage);
    }

    /// <summary>
    /// Used for Throw hits
    /// </summary>
    /// <param name="col"></param>
    private void OnCollisionEnter(Collision col)
    {
        if (!m_beingThrown) return;

        ApplyMeleeAttack(col.gameObject, m_baseDamage * 2);
        m_beingThrown = false;
        Owner = null;
    }

    private void ApplyMeleeAttack(GameObject col, int dmg)
    {
        var attackable = col.GetComponent<IAttackable>();

        if (attackable == null) return;

        var isCritical = DetermineCritical();

        var attack = new Attack(dmg, isCritical);

        attackable.OnAttack(Owner, attack);
    }

    private bool DetermineCritical() => (UnityEngine.Random.Range(0, 100) < m_criticalHitChance);
}
