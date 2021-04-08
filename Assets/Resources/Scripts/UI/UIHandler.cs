using Photon.Pun;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject m_playerUIPrefab;
    [SerializeField] private Health m_health;
    [HideInInspector] public PlayerHealthDisplay playerHealthDisplay;

    public Health GetHealth() => m_health;

    private PlayerHandler m_playerHandler;

    private void LateUpdate()
    {
        if (!m_playerHandler)
            m_playerHandler = transform.parent.GetComponent<PlayerHandler>();

        if (m_playerHandler.PlayerHealthDisplayGO == null)
        {
            var view = GetComponent<PhotonView>();
            if (view.IsMine)
                view.RPC("CreateUI", RpcTarget.All);
        }

        if (playerHealthDisplay == null)
        {
            playerHealthDisplay = m_playerHandler.PlayerHealthDisplayGO.GetComponent<PlayerHealthDisplay>();
        }
    }

    [PunRPC]
    private void CreateUI()
    {
        if (this.m_playerUIPrefab != null)
        {
            GameObject _uiGo = Instantiate(this.m_playerUIPrefab);

            if (!m_playerHandler)
                m_playerHandler = transform.parent.GetComponent<PlayerHandler>();

            m_playerHandler.PlayerHealthDisplayGO = _uiGo;

            _uiGo.SendMessage("SetTarget", gameObject, SendMessageOptions.RequireReceiver);
            playerHealthDisplay = _uiGo.GetComponent<PlayerHealthDisplay>();
        }
        else
        {
            Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }
}