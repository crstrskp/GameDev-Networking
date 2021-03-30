using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnimationHandler : MonoBehaviourPun
{
    [SerializeField] private Animator m_anim;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAttackInput m_playerAttackInput;
    [SerializeField] private PlayerWeaponHandler m_playerWeaponHandler;

    //public event Action Attack;
    public event Action AttackStarted;
    public event Action AttackEnded;

    private void Awake() 
    {
        if (!photonView.IsMine) return;

        playerMovement.Jumping += SetJumpAnimation;    
        playerMovement.Landing += SetLandingAnimation;
        m_playerAttackInput.Attack += SetAttackAnimation;
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
        m_anim.SetBool("Running", playerMovement.IsSprinting());

        m_anim.SetBool("Falling", playerMovement.isFalling());

        m_anim.SetBool("isGrounded", playerMovement.IsGrounded());

    }

    private void SetJumpAnimation() => m_anim.SetTrigger("JumpUp");

    private void SetLandingAnimation() => m_anim.SetTrigger("Landing");

    private void SetAttackAnimation()
    {
        if (m_playerWeaponHandler.IsAttacking)
        {
            Debug.Log("Already attacking - ignoring input");
            return;
        }

        m_anim.SetTrigger("Attack");
        m_playerWeaponHandler.IsAttacking = true;
    }

    private void AttackStart()
    {
        if (m_playerWeaponHandler.EquippedWeapon && m_playerWeaponHandler.EquippedWeapon.AttackActive) return;

        AttackStarted?.Invoke();
    }

    private void AttackEnd()
    {
        AttackEnded?.Invoke();
    }


    #region cleanup

    private void OnDestroy()
    {
        playerMovement.Jumping -= SetJumpAnimation;    
        playerMovement.Landing -= SetLandingAnimation;
        m_playerAttackInput.Attack -= SetAttackAnimation;
    }
    #endregion
}
