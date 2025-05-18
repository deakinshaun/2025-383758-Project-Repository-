using System;
using OrgilFolder.Scripts.GamePlay.GameModes.Basketball;
using TMPro;
using UnityEngine;

namespace OrgilFolder.Scripts.GamePlay
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField] private TMP_Text timer;
        [SerializeField] private TMP_Text team1ScoreText;
        [SerializeField] private TMP_Text team2ScoreText;

        private void Start()
        {
            timer.text = TimeSpan.FromSeconds(GameManager.Instance.MaxTime).ToString(@"mm\:ss");

            BasketballGameRule.Instance.OnScored += UpdateScore;
        }

        private void OnDestroy()
        {
            BasketballGameRule.Instance.OnScored -= UpdateScore;
        }

        private void UpdateScore(int team, int points)
        {
            if (team == 1)
            {
                team1ScoreText.SetText(points.ToString());
            }
            else
            {
                team2ScoreText.SetText(points.ToString());
            }
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance == null) return;
            timer.text = TimeSpan.FromSeconds(GameManager.Instance.MaxTime - GameManager.Time).ToString(@"mm\:ss");
        }
    }
}