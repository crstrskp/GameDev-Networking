using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnimationHandler : MonoBehaviourPun
{
    [SerializeField] private Animator m_anim;
    [SerializeField] private PlayerMovement m_playerMovement;
    [SerializeField] private PlayerAttackInput m_playerAttackInput;
    [SerializeField] private PlayerWeaponHandler m_playerWeaponHandler;

    public event Action AttackStarted;
    public event Action AttackEnded;
    public event Action ThrowEquipped;

    private void Awake() 
    {
        if (!photonView.IsMine) return;

        if (!m_playerMovement) SetPlayerMovement();

        m_playerMovement.Jumping += SetJumpAnimation;    
        m_playerMovement.Landing += SetLandingAnimation;
        m_playerAttackInput.Attack += SetAttackAnimation;
        m_playerAttackInput.Throw += SetThrowAnimation;
    }

    private void SetPlayerMovement()
    {
        
    }

    private void LateUpdate() 
    {
        if (!photonView.IsMine) return;

        SetAnimationVariables();
    }
    
    private void SetAnimationVariables()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        var moving = Mathf.Abs(x) + Mathf.Abs(y) > 0.25 ? true : false;

        m_anim.SetFloat("VelocityX", x);
        m_anim.SetFloat("VelocityY", y);

        m_anim.SetBool("Moving", moving);
        m_anim.SetBool("Running", m_playerMovement.IsSprinting());

        m_anim.SetBool("Falling", m_playerMovement.isFalling());

        m_anim.SetBool("isGrounded", m_playerMovement.IsGrounded());

    }

    private void SetJumpAnimation() => m_anim.SetTrigger("JumpUp");

    private void SetLandingAnimation() => m_anim.SetTrigger("Landing");

    private void SetAttackAnimation()
    {
        if (m_playerWeaponHandler.IsAttacking) return;

        m_anim.SetTrigger("Attack");
        m_playerWeaponHandler.IsAttacking = true;
    }

    /// <summary>
    /// Called from Animation event
    /// </summary>
    private void AttackStart()
    {
        if (m_playerWeaponHandler.EquippedWeapon && m_playerWeaponHandler.EquippedWeapon.AttackActive) return;

        AttackStarted?.Invoke();
    }

    /// <summary>
    /// Called from Animation event
    /// </summary>
    private void AttackEnd()
    {
        AttackEnded?.Invoke();
    }

    private void SetThrowAnimation()
    {
        m_anim.SetTrigger("Throw");
    }

    /// <summary>
    /// Called from Animation event
    /// </summary>
    private void ThrowObject()
    {
        ThrowEquipped?.Invoke();
    }
    

    #region cleanup

    private void OnDestroy()
    {
        m_playerMovement.Jumping -= SetJumpAnimation;    
        m_playerMovement.Landing -= SetLandingAnimation;
        m_playerAttackInput.Attack -= SetAttackAnimation;
        m_playerAttackInput.Throw -= SetThrowAnimation;
    }
    #endregion
}
