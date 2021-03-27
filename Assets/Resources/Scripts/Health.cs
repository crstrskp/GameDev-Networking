using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviour
{
    public event Action<int, int> HealthChanged;

    [SerializeField] private int m_currentHealth;
    [SerializeField] private int m_maxHealth = 100;

    private void Awake()
    {
        m_currentHealth = m_maxHealth;
        HealthChanged?.Invoke(m_currentHealth, m_maxHealth);
    }

    public void TakeDamage(float damage)
    {
        m_currentHealth = (int)Mathf.Max(m_currentHealth - damage, 0);
        HealthChanged?.Invoke(m_currentHealth, m_maxHealth);
    }

    public void Heal(float heal)
    {
        m_currentHealth = (int)Mathf.Min(m_currentHealth + heal, m_maxHealth);
        HealthChanged?.Invoke(m_currentHealth, m_maxHealth);
    }

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
    //             if (stream.isWriting) {
    //                 stream.SendNext ();
    //             } else if (stream.isReading) {
    //                 m_currentHealth = (float)stream.ReceiveNext(); // TODO: Consider if this really should be streamed or if the UI value is enough? 
    //             }
    //         }
}
