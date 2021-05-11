using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPunCallbacks, IPunObservable, IAttackable
{
    public event Action<int, int> HealthChanged;

    [SerializeField] private int m_currentHealth;
    [SerializeField] private int m_maxHealth = 100;
    [SerializeField] private bool m_invulnerable = false;


    public int GetCurrentHealth() => m_currentHealth;
    public int GetMaxHealth() => m_maxHealth;

    private void OnEnable()
    {
        
        if (!photonView.IsMine) return;

        m_invulnerable = false;

        m_currentHealth = m_maxHealth;
        HealthChanged?.Invoke(m_currentHealth, m_maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (!photonView.IsMine) return;

        m_currentHealth = (int)Mathf.Max(m_currentHealth - damage, 0);
        HealthChanged?.Invoke(m_currentHealth, m_maxHealth);
    }

    public void Heal(float heal)
    {
        if (!photonView.IsMine) return;

        m_currentHealth = (int)Mathf.Min(m_currentHealth + heal, m_maxHealth);
        HealthChanged?.Invoke(m_currentHealth, m_maxHealth);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.m_currentHealth);
        }
        else
        {
            // Network player, receive data
            this.m_currentHealth = (int)stream.ReceiveNext();
        }
    }

    public void OnAttack(GameObject attacker, Attack attack)
    {
        if (attacker == gameObject) return; // can't hit self. 

        if (m_invulnerable) return;

        TakeDamage(attack.Damage);

        if (m_currentHealth <= 0)
        {
            var destructibles = GetComponents(typeof(IDestructible));
            foreach (IDestructible d in destructibles)
                d.OnDestruction(attacker);

            var playerHandler = transform.GetComponent<PlayerHandler>();

            if (playerHandler == null) return;

            if (!photonView.IsMine) return;
            
            playerHandler.photonView.RPC("PlayerDeath", RpcTarget.All);

            //var attackerScore = attacker.GetComponent<PlayerScore>();
            Debug.Log($"Should be adding kill to {attacker.GetComponent<PhotonView>().Owner.NickName}");
            attacker.GetComponent<PhotonView>().RPC("AddKillScore", RpcTarget.All);
            //if (attackerScore == null) return;

            //attackerScore.AddKill();
        }
    }

    public bool Invulnerable() => m_invulnerable;
    public bool Invulnerable(bool b) => m_invulnerable = b; 
}
