using System;
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
        }

        private void FixedUpdate()
        {
            if(GameManager.Instance==null) return;
            timer.text = TimeSpan.FromSeconds(GameManager.Instance.MaxTime - GameManager.Time).ToString(@"mm\:ss");
        }
    }
}