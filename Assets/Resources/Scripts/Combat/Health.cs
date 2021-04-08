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

    public int GetCurrentHealth() => m_currentHealth;
    public int GetMaxHealth() => m_maxHealth;

    private void Awake()
    {
        if (!photonView.IsMine) return;

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

        TakeDamage(attack.Damage);

        if (m_currentHealth <= 0)
        {
            // Destroy Object
            var destructibles = GetComponents(typeof(IDestructible));
            foreach (IDestructible d in destructibles)
                d.OnDestruction(attacker);

            var playerHandler = transform.parent.GetComponent<PlayerHandler>();

            if (playerHandler == null) return;

            if (!photonView.IsMine) return;

            playerHandler.photonView.RPC("DelayedRespawn", RpcTarget.All);

            photonView.isRuntimeInstantiated = false;
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
