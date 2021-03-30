using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    [SerializeField] private Health m_health;
    [SerializeField] private GameObject m_destroyedCratePrefab;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_audioClip;
    [SerializeField] private float m_volume = 0.5f;

    void Awake() => m_health.OnDeath += DestroyCrate;

    void OnDestroy() => m_health.OnDeath -= DestroyCrate;

    private void DestroyCrate()
    {
        if (m_destroyedCratePrefab)
        {
            m_audioSource.PlayOneShot(m_audioClip, m_volume);
            Instantiate(m_destroyedCratePrefab, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}

