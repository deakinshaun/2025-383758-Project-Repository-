using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OrgilFolder.Scripts.UI
{
    public class CarouselView : MonoBehaviour
    {
        [SerializeField] private Button nextButton;
        [SerializeField] private Button prevButton;
        [SerializeField] private RectTransform contentRect;
        public int currentIndex;

        private int childCount;
        private float childWidth;

        private IEnumerator Start()
        {
            yield return null;
            // scrollRect.content.anchoredPosition;
            nextButton.onClick.AddListener(Next);
            prevButton.onClick.AddListener(Prev);
            childCount = contentRect.childCount;
            childWidth = contentRect.sizeDelta.x / (childCount - 1);
        }

        public void Next()
        {
            currentIndex = (currentIndex + 1) % childCount;
            contentRect.anchoredPosition = new Vector2(-currentIndex * childWidth, 0);
        }

        public void Prev()
        {
            currentIndex = (currentIndex - 1 + childCount) % childCount;
            contentRect.anchoredPosition = new Vector2(-currentIndex * childWidth, 0);
        }
    }
}