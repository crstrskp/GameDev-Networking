using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHandler : MonoBehaviourPun
{
    
    [SerializeField] private GameObject m_playerUIPrefab;

    [SerializeField] private GameObject m_playerCamera;
    private void Start()
    {
        CreatePlayerCamera();

        CreateUI();
    }

    private void CreatePlayerCamera()
    {
        if (this.m_playerCamera != null)
        {
            GameObject playerCam = Instantiate(this.m_playerCamera, transform);
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
            _uiGo.GetComponent<PlayerHealthDisplay>().SetTarget(this);
        }
        else
        {
            Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }
}
