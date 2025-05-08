using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace OrgilFolder.Scripts.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private TMP_Text countDownText;


        private void Start()
        {
            StartCoroutine(CountDown(5f));
            if (GameManager.State.Current == GameState.EGameState.Intro)
            {
            }
        }

        IEnumerator CountDown(float timer)
        {
            float t = timer;
            while (t > 1)
            {
                t -= Time.deltaTime;
                yield return null;
                countDownText.SetText(Mathf.RoundToInt(t).ToString());
            }
            countDownText.SetText("Start");
            yield return new WaitForSeconds(1f);
            countDownText.SetText("");
        }
    }
}