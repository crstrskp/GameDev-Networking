using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour, IDestructible
{
    private int m_kills;
    private int m_creepKills;
    private int m_deaths;

    private void Awake() => UpdateCustomProperties();

    public int GetKills() => m_kills;
    public int GetCreepKills() => m_creepKills;
    public int GetDeaths() => m_deaths;

    [PunRPC]
    public void AddKillScore()
    {
        m_kills++;
        UpdateCustomProperties();
    }

    [PunRPC]
    public void AddCreepKillScore()
    {
        m_creepKills++;
        UpdateCustomProperties();
    }

    public void AddDeath()
    {
        m_deaths++;
        UpdateCustomProperties();
    }

    public void OnDestruction(GameObject destroyer)
    {
        AddDeath();
    }

    public void UpdateCustomProperties()
    {
        var view = GetComponent<PhotonView>();

        if (!view.IsMine) return;

        var owner = view.Owner;
        var customProperties = owner.CustomProperties;
        customProperties["Kills"] = m_kills;
        customProperties["CreepKills"] = m_creepKills;
        customProperties["Deaths"] = m_deaths;
        
        owner.SetCustomProperties(customProperties);
    }
}
