using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;

public class PlayerHandler : MonoBehaviourPun
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    public GameObject PlayerHealthDisplayGO;

    [SerializeField] private float m_respawnTime = 2.5f;
    [SerializeField] private GameObject m_cameraPrefab;
    [SerializeField] private GameObject m_model;
    [SerializeField] private Health m_health;

    private Collider m_collider;
    private CharacterController m_characterController; 

    private GameObject m_camera;
    

    private void Awake() 
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }

        m_collider = GetComponent<Collider>();
        m_characterController = GetComponent<CharacterController>();
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
        if (PlayerHealthDisplayGO == null) return; 

        if (PlayerHealthDisplayGO.activeSelf) return;

        PlayerHealthDisplayGO.SetActive(true);
    }

    private void CreatePlayerCamera()
    {
        if (this.m_cameraPrefab != null)
        {
            if (m_camera != null) return;

            this.m_camera = Instantiate(this.m_cameraPrefab, transform);
            this.m_camera.GetComponent<WowCamera>().Target = transform;
        }
        else 
        {
            Debug.LogWarning("Missing PlayerCamera reference on player prefab.", this);
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

        var playerHealthDisplay = PlayerHealthDisplayGO.GetComponent<PlayerHealthDisplay>();
        playerHealthDisplay.SetTarget(gameObject);
        playerHealthDisplay.gameObject.SetActive(true);
    }

    [PunRPC]
    public void PlayerDeath()
    {
        DisablePlayer();
        DelayedRespawn();
    }

    private void DelayedRespawn()
    {
        DeactivateUI();
        StartCoroutine(SpawnAfterDelay(m_respawnTime));
    }

    private IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetPlayer();
        ReenablePlayer();
        ReactivateUI();
    }

    private void ResetPlayer()
    {
        if (!photonView.IsMine) return;

        m_health.Heal(float.MaxValue);

        Restock();

        SetSpawnPoint();
    }

    private void SetSpawnPoint()
    {
        Debug.LogWarning("Refactor: RespawnPoints should be singleton");
        var spawnPoint = GameObject.Find("RespawnPoints").GetComponent<RespawnPoints>().GetSpawnPoint();

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    private void Restock()
    {
        Debug.LogWarning("Refactor Restock method!");
        var inv = GetComponent<Inventory>();

        PhotonNetwork.Instantiate("Weapons\\Metal Sword", transform.position, Quaternion.identity);
        inv.TryPickUp();
        PhotonNetwork.Instantiate("Weapons\\Wooden Shield", transform.position, Quaternion.identity);
        inv.TryPickUp();
    }

    private void ReenablePlayer()
    {
        m_characterController.enabled = true;
        m_collider.enabled = true;
        m_model.SetActive(true);
    }

    private void DisablePlayer()
    {
        m_model.SetActive(false);
        m_collider.enabled = false;
        m_characterController.enabled = false;
    }

    public GameObject GetModel() => m_model;
}
