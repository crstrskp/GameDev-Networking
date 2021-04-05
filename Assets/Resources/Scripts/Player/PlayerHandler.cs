using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;

public class PlayerHandler : MonoBehaviourPun
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    public GameObject PlayerObject;
    public GameObject PlayerHealthDisplayGO;

    [SerializeField] private float m_respawnTime = 2.5f;
    [SerializeField] private GameObject m_cameraPrefab;

    private GameObject m_camera;
    private int m_playerObjectViewId;

    private void Awake() 
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
            photonView.RPC("SetParentOfPlayerModel", RpcTarget.All, PlayerObject.GetPhotonView().ViewID);
        }

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (!photonView.IsMine) return;

        if (this.m_camera == null)
            CreatePlayerCamera();
    }

    private void LateUpdate()
    {
        if (PlayerObject == null)
        {
            // This is terrible design! 
            // TODO: ensure PlayerObject is set, prior to all clients receiving a call to update it!
            photonView.RPC("SetPlayerObject", RpcTarget.All);
            if (PlayerObject == null)
                Debug.Log("PlayerObject could not be found.");
        }
        else
        {
            if (PlayerHealthDisplayGO == null) return; 

            if (PlayerHealthDisplayGO.activeSelf) return;

            PlayerHealthDisplayGO.SetActive(true);
        }
    }

    private void CreatePlayerCamera()
    {
        if (this.m_cameraPrefab != null)
        {
            if (m_camera != null) return;

            this.m_camera = Instantiate(this.m_cameraPrefab, transform);
            m_camera.GetComponent<WowCamera>().Target = PlayerObject.transform;
        }
        else 
        {
            Debug.LogWarning("Missing PlayerCamera reference on player prefab.", this);
        }
    }

    public void SpawnPlayer()
    {
        if (photonView.IsMine)
        {
            var spawnPos = new Vector3(10, 10, 0);
            GameObject player = PhotonNetwork.Instantiate("PlayerPun", spawnPos, Quaternion.identity);
            photonView.RPC("SetParentOfPlayerModel", RpcTarget.All, player.GetPhotonView().ViewID);

            m_camera.GetComponent<WowCamera>().Target = player.transform;
        }
    }

    private void DeactivateUI()
    {
        PlayerHealthDisplayGO.SetActive(false);
    }

    private void ReactivateUI()
    {
        if (!PlayerHealthDisplayGO)
        {
            Debug.Log("playerHealthDisplay not set");
            return;
        }

        if (PlayerObject == null) return;

        var playerHealthDisplay = PlayerHealthDisplayGO.GetComponent<PlayerHealthDisplay>();
        playerHealthDisplay.SetTarget(PlayerObject);
        playerHealthDisplay.gameObject.SetActive(true);
    }

    [PunRPC]
    private void SetPlayerObject()
    {
        PhotonView playerView = PhotonView.Find(m_playerObjectViewId);

        if (playerView != null)
            PlayerObject = PhotonView.Find(m_playerObjectViewId).gameObject;
    }

    [PunRPC]
    private void SetParentOfPlayerModel(int viewId)
    {
        m_playerObjectViewId = viewId;

        var go = PhotonView.Find(viewId).transform;
        go.SetParent(transform);
    }

    [PunRPC]
    public void DelayedRespawn()
    {
        DeactivateUI();
        StartCoroutine(SpawnAfterDelay(m_respawnTime));
    }

    private IEnumerator SpawnAfterDelay(float delay)
    {

        yield return new WaitForSeconds(delay);
        SpawnPlayer();
        ReactivateUI();
    }
}
