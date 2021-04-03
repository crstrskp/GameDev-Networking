using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class PlayerHandler : MonoBehaviourPun
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    [SerializeField] private float m_respawnTime = 2.5f;
    [SerializeField] private GameObject m_cameraPrefab;
    public GameObject PlayerObject;

    private GameObject m_camera;

    private void Awake() 
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }
        
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!photonView.IsMine) return;

        if (this.m_camera == null)
            CreatePlayerCamera();
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
        var spawnPos = new Vector3(10, 10, 0);
        //PlayerObject = (GameObject)Instantiate(Resources.Load("Prefabs\\Player\\PlayerPun"), spawnPos, Quaternion.identity);
        if (!photonView.IsMine) return;

        PlayerObject = PhotonNetwork.Instantiate("PlayerPun", spawnPos, Quaternion.identity);
        Debug.Log($"PlayerObject spawned: {PlayerObject}");
        PlayerObject.transform.parent = transform;

        m_camera.GetComponent<WowCamera>().Target = PlayerObject.transform;
        Debug.Log($"Setting camera target to: {PlayerObject.transform.name}");
    }

    [PunRPC]
    public void DelayedRespawn()
    {
        StartCoroutine(SpawnAfterDelay(m_respawnTime));
    }

    private IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnPlayer();
    }
}
