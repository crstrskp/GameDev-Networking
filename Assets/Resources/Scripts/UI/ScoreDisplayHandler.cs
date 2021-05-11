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

    private void UpdateScoreboard()
    {
        m_numberOfPlayers.text = PhotonNetwork.PlayerList.Length + " / 20";

        var yPos = 0;

        var myList = m_playerUIStats.ToList();

        myList.Sort((pair1, pair2) => pair1.Value.PlayerKills.text.CompareTo(pair2.Value.PlayerKills.text));

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (m_playerUIStats.ContainsKey(player.ActorNumber))
            {
                var playerUIStats = m_playerUIStats[player.ActorNumber];

                playerUIStats.PlayerName.text = player.NickName;
                playerUIStats.PlayerKills.text = player.CustomProperties["Kills"].ToString();
                playerUIStats.PlayerCreepKills.text = player.CustomProperties["CreepKills"].ToString();
                playerUIStats.PlayerDeaths.text = player.CustomProperties["Deaths"].ToString();

                var rect = playerUIStats.GetComponent<RectTransform>();
                rect.localPosition = new Vector2(0, yPos);
                
                yPos -= 50;
            }
            else
            {
                Debug.Log("Player not seen on Scoreboard before. Creating new PlayerUIStats obj for it...");
                var s = Instantiate(m_playerUIStatsObject, m_playerUIStatsObject.transform.position, Quaternion.identity, m_scoreboard.transform);
                m_playerUIStats.Add(player.ActorNumber, s.GetComponent<PlayerUIStats>());
                UpdateScoreboard();
            }
        }
    }
}
