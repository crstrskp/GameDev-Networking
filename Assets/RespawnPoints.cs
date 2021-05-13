using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoints : MonoBehaviour
{
    [SerializeField] private List<Transform> m_spawnPoints = new List<Transform>();

    public enum SpawnRotationMode { Random, RoundRobin, Teams };
    [SerializeField] private SpawnRotationMode m_spawnRotationMode = RespawnPoints.SpawnRotationMode.RoundRobin;

    private int i = 0;

    public Transform GetSpawnPoint() => GetSpawnPoint(m_spawnRotationMode);
    public Transform GetSpawnPoint(SpawnRotationMode spawnRotationMode)
    {
        switch(spawnRotationMode)
        {
            default:
            case SpawnRotationMode.RoundRobin:
                return m_spawnPoints[i + 1 > m_spawnPoints.Count - 1 ? 0 : i++];
            case SpawnRotationMode.Random:
                return m_spawnPoints[UnityEngine.Random.Range(0, m_spawnPoints.Count)];
            case SpawnRotationMode.Teams:
                throw new NotImplementedException();
        }   
    }
}
