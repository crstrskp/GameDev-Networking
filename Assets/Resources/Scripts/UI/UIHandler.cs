using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour, IDestructible
{
    [SerializeField] private GameObject m_playerUIPrefab;
    [SerializeField] private Health m_health;
    public PlayerHealthDisplay playerHealthDisplay;

    public Health GetHealth() => m_health;

    private void LateUpdate()
    {
        if (playerHealthDisplay == null)
            CreateUI();
    }

    private void Start()
    {
        if (playerHealthDisplay == null)
            CreateUI();
    }
    private void CreateUI()
    {
        if (this.m_playerUIPrefab != null)
        {
            GameObject _uiGo = Instantiate(this.m_playerUIPrefab);
            //GameObject _uiGo = PhotonNetwork.Instantiate("UI\\HealthDisplay", Vector3.zero, Quaternion.identity);
            playerHealthDisplay = _uiGo.GetComponent<PlayerHealthDisplay>();
            playerHealthDisplay.SetPlayerHandler(transform.parent.GetComponent<PlayerHandler>());
        }
        else
        {
            Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }

    public void OnDestruction(GameObject destroyer)
    {
        Debug.Log($"OnDestruction called on UIHandler. Destroyer: {destroyer}, fixing to destroy UI element");
       
        if (!GetComponentInParent<PhotonView>().IsMine) return;

        //playerHealthDisplay.transform.SetParent(transform);
        //Debug.Log($"playerHealthDisplay parent: {playerHealthDisplay.transform.parent.name}");
        PhotonNetwork.Destroy(playerHealthDisplay.gameObject);
    }
}