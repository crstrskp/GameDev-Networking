using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ScoreDisplayHandler : MonoBehaviour
{
    [SerializeField] private ScoreHandler m_scoreHandler;

    [SerializeField] private GameObject m_scoreboard;

    [SerializeField] private GameObject m_playerUIStatsObject;

    [SerializeField] private TMP_Text m_numberOfPlayers;
    
    private PlayerAttackInput m_playerAttackInput;

    private Dictionary<int, PlayerUIStats> m_playerUIStats = new Dictionary<int, PlayerUIStats>();

    private void Start()
    {
        m_playerAttackInput = PlayerHandler.LocalPlayerInstance.GetComponent<PlayerAttackInput>();

        m_playerAttackInput.ShowScoreStart += ShowScoreStart;
        m_playerAttackInput.ShowScoreEnd += ShowScoreEnd;

        UpdateScoreboard();
    }

    private void OnDestroy()
    {
        m_playerAttackInput.ShowScoreStart -= ShowScoreStart;
        m_playerAttackInput.ShowScoreEnd -= ShowScoreEnd;
    }

    private void ShowScoreStart()
    {
        UpdateScoreboard();
        SetScoreboardActive(true);
    }

    private void ShowScoreEnd() => SetScoreboardActive(false);

    private void SetScoreboardActive(bool v) => m_scoreboard.SetActive(v);

    /// <summary>
    /// There are many ways this method could be improved!
    ///     o The Dictionary "m_playerUIStats" might not really be necessary
    ///     o The myList of the Dictionary screams that both data types are poor choices. 
    ///     o Finding the player variable when looping through kvp's in myList defeats the purpose of having the key in the first place. 
    /// </summary>
    private void UpdateScoreboard()
    {
        m_numberOfPlayers.text = PhotonNetwork.PlayerList.Length + " / 20";

        var yPos = 0;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (m_playerUIStats.ContainsKey(player.ActorNumber)) continue;
            
            Debug.Log("Player not seen on Scoreboard before. Creating new PlayerUIStats obj for it...");
            var s = Instantiate(m_playerUIStatsObject, m_playerUIStatsObject.transform.position, Quaternion.identity, m_scoreboard.transform);
            m_playerUIStats.Add(player.ActorNumber, s.GetComponent<PlayerUIStats>());
        }
        
        var myList = m_playerUIStats.ToList();

        myList.Sort((pair1, pair2) => pair1.Value.PlayerKills.text.CompareTo(pair2.Value.PlayerKills.text));
        myList.Reverse();

        foreach (var kvp in myList)
        {
            var player = PhotonNetwork.PlayerList.Where(x => x.ActorNumber == kvp.Key).FirstOrDefault();
            
            kvp.Value.PlayerName.text = player.NickName;
            kvp.Value.PlayerKills.text = player.CustomProperties["Kills"].ToString();
            kvp.Value.PlayerCreepKills.text = player.CustomProperties["CreepKills"].ToString();
            kvp.Value.PlayerDeaths.text = player.CustomProperties["Deaths"].ToString();

            var rect = kvp.Value.GetComponent<RectTransform>();
            rect.localPosition = new Vector2(0, yPos);

            yPos -= 50;
        }
    }
}
