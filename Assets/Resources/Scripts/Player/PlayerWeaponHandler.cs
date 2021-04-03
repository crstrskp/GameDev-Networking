using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour, IDestructible
{
    [SerializeField] private PlayerAttackInput m_playerAttackInput;
    [SerializeField] private AnimationHandler m_animHandler;

    public Transform LeftHandTransform;
    public Transform RightHandTransform;
    public ItemPickUp EquippedWeapon;
    public ItemPickUp EquippedShield;

    public ItemPickUp[] WeaponSlots = new ItemPickUp[6];

    public bool IsAttacking = false;

    private void Awake()
    {
        m_animHandler.AttackStarted += AttackStart;
        m_animHandler.AttackEnded += AttackEnd;

        m_playerAttackInput.WeaponSelect += TryChangeWeapon;
        m_playerAttackInput.DropEquipped += DropEquipped;
        m_playerAttackInput.ThrowEquipped += ThrowItem;
    }

    private void OnDestroy()
    {
        m_animHandler.AttackStarted -= AttackStart;
        m_animHandler.AttackEnded -= AttackEnd;

        m_playerAttackInput.WeaponSelect -= TryChangeWeapon;
        m_playerAttackInput.DropEquipped -= DropEquipped;
        m_playerAttackInput.ThrowEquipped -= ThrowItem;
    }

    private void TryChangeWeapon(int weaponSlot)
    {
        if (WeaponSlots[weaponSlot - 1] == null) return;

        if (WeaponSlots[weaponSlot - 1] == EquippedWeapon) return;

        EquippedWeapon = WeaponSlots[weaponSlot - 1];
    }

    private void DropEquipped()
    {
        if (EquippedWeapon != null)
        {
            EquippedWeapon.rigidbody.isKinematic = false;
            EquippedWeapon.transform.parent = null;
            EquippedWeapon.GetComponent<Collider>().isTrigger = false;
            EquippedWeapon = null;
        }
        else if (EquippedShield != null)
        {
            EquippedShield.rigidbody.isKinematic = false;
            EquippedShield.transform.parent = null;
            EquippedShield.GetComponent<Collider>().isTrigger = false;
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
                WeaponSlots[i].GetComponent<Collider>().isTrigger = false;
                WeaponSlots[i].rigidbody.isKinematic = false;
                WeaponSlots[i] = null;
            }
        }
    }

    private void AttackStart()
    {
        EquippedWeapon.AttackActive = true;
    }

    private void AttackEnd()
    {
        EquippedWeapon.AttackActive = false;
        IsAttacking = false;
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

    public void OnDestruction(GameObject destroyer)
    {
        DropItemsOnDeath();
    }
}
