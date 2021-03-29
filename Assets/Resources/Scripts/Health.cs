using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPunCallbacks, IPunObservable
{
    public event Action<int, int> HealthChanged;
    public event Action OnDeath;

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

    private void Update()
    {
        if (m_currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }

        if (!photonView.IsMine) return; 

        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(6);
        }
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
}
