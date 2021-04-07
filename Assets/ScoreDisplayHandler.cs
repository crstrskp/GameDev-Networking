using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ScoreDisplayHandler : MonoBehaviour
{
    [SerializeField] private ScoreHandler m_scoreHandler;
    [SerializeField] private PlayerAttackInput m_playerAttackInput;

    [SerializeField] private GameObject m_scoreboard;

    [SerializeField] private TMP_Text m_numberOfPlayers;
    

    private void OnEnable()
    {
        m_playerAttackInput.ShowScoreStart += ShowScoreStart;
        m_playerAttackInput.ShowScoreEnd += ShowScoreEnd;
    }

    private void OnDestroy()
    {
        m_playerAttackInput.ShowScoreStart -= ShowScoreStart;
        m_playerAttackInput.ShowScoreEnd -= ShowScoreEnd;
    }

    private void ShowScoreStart() => SetScoreboardActive(true);
    private void ShowScoreEnd() => SetScoreboardActive(false);

    private void SetScoreboardActive(bool v)
    {
        
    }

    private void UpdateScoreboard()
    {
        //foreach (PhotonPlayer netPlayer in PhotonNetwork.playerList)
        //{

        //}
    }
}
