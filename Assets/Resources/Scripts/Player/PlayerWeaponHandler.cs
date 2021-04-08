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

        m_playerAttackInput.ThrowEquipped += ThrowItem; // SHOULD BE FROM ANIMATOR AS WELL!
    }

    private void OnDestroy()
    {
        m_animHandler.AttackStarted -= AttackStart;
        m_animHandler.AttackEnded -= AttackEnd;

        m_playerAttackInput.ThrowEquipped -= ThrowItem;
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
}
