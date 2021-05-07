using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private PlayerAttackInput m_playerAttackInput;
    [SerializeField] private AnimationHandler m_animHandler;


    public ItemPickUp EquippedWeapon;
    public ItemPickUp EquippedShield;


    public bool IsAttacking = false;

    private void Awake()
    {
        m_animHandler.AttackStarted += AttackStart;
        m_animHandler.AttackEnded += AttackEnd;

        m_animHandler.ThrowEquipped += ThrowItem; 
    }

    private void OnDestroy()
    {
        m_animHandler.AttackStarted -= AttackStart;
        m_animHandler.AttackEnded -= AttackEnd;

        m_animHandler.ThrowEquipped -= ThrowItem;
    }

    private void AttackStart()
    {
        if (EquippedWeapon == null) return; // TODO: Unarmed attacks?

        EquippedWeapon.AttackActive = true;
    }

    private void AttackEnd()
    {
        IsAttacking = false;

        if (EquippedWeapon == null) return;

        EquippedWeapon.AttackActive = false;
    }

    private void ThrowItem()
    {
        Debug.Log("ThrowItem()");

        if (EquippedWeapon == null) return;

        // Add force
        // add flag to item ? beingThrown, so it will damage target

    }
}
