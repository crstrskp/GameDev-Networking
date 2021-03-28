using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHandler : MonoBehaviourPun
{
    
    [SerializeField] private GameObject m_playerUIPrefab;

    [SerializeField] private GameObject m_cameraPrefab;
    [SerializeField] private Health m_health;

    private PlayerHealthDisplay m_playerHealthDisplay;
    private GameObject m_camera;

    public Health GetHealth() => m_health;

    private void Start()
    {
        CreatePlayerCamera();

       CreateUI();
    }

    private void CreatePlayerCamera()
    {
        if (this.m_cameraPrefab != null)
        {
            this.m_camera = Instantiate(this.m_cameraPrefab, transform);
        }
        else 
        {
            Debug.LogWarning("Missing PlayerCamera reference on player prefab.", this);
        }
    }

    private void CreateUI()
    {
        if (this.m_playerUIPrefab != null)
        {
            GameObject _uiGo = Instantiate(this.m_playerUIPrefab);
            m_playerHealthDisplay = _uiGo.GetComponent<PlayerHealthDisplay>();
            m_playerHealthDisplay.SetTarget(this);
            m_playerHealthDisplay.SetPlayerHandler(this);
        }
        else
        {
            Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }
}
