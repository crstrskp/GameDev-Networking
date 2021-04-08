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

    public bool Throwable;
    public GameObject Owner;

    [Tooltip("The input number pressed for equipping this item")]
    public int WeaponSlotKey;

    [HideInInspector] public Rigidbody rigidbody;
    [HideInInspector] public bool AttackActive = false;
    [HideInInspector] public bool BeingThrown = false;

    [SerializeField] private Transform HoldPoint;
    [SerializeField] private int m_baseDamage = 15;
    [SerializeField] private int m_criticalHitChance = 15;



    public void Drop()
    {
        rigidbody.isKinematic = false;
        transform.parent = null;
        Owner = null;
        GetComponent<Collider>().isTrigger = false;
        gameObject.layer = LayerMask.NameToLayer("ItemPickUp");
    }

    public void PickUp()
    {
        rigidbody.isKinematic = true;
        //transform.parent = LEFT OR RIGHT HAND ?
        GetComponent<Collider>().isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("OwnedItem");
    }

    private void LateUpdate()
    {
        if (BeingThrown == false) return;
        
        if (rigidbody.velocity.magnitude > 0.05f) return;

        BeingThrown = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (WeaponType != WeaponSubType.Melee) return;
        
        if (AttackActive == false) return;

        var attackable = col.gameObject.GetComponent<IAttackable>();
        
        if (attackable == null) return;

        var isCritical = DetermineCritical();

        var attack = new Attack(m_baseDamage, isCritical);

        //Debug.Log($"{Owner.name} attacks {col.gameObject.name}, damage: {attack.Damage}, isCritical: {isCritical}");
        
        attackable.OnAttack(Owner, attack);
    }

    private bool DetermineCritical() => (UnityEngine.Random.Range(0, 100) < m_criticalHitChance);
}
