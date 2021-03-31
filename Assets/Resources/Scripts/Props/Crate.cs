using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour, IDestructible
{
    [SerializeField] private GameObject m_destroyedCratePrefab;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_audioClip;
    [SerializeField] private float m_volume = 0.5f;

    public void OnDestruction(GameObject destroyer)
    {
        if (m_destroyedCratePrefab)
        {
            m_audioSource.PlayOneShot(m_audioClip, m_volume);
            Instantiate(m_destroyedCratePrefab, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}

