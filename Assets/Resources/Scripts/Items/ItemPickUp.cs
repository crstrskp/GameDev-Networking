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

    public void PickUp(GameObject owner)
    {
        gameObject.layer = LayerMask.NameToLayer("OwnedItem");
        rigidbody.isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        Owner = owner;
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
