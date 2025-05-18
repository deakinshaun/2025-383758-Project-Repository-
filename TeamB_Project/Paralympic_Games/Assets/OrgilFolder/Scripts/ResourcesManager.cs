using OrgilFolder.Scripts.GamePlay;
using UnityEngine;

namespace OrgilFolder.Scripts
{
    public class ResourcesManager : MonoBehaviour
    {
        public PlayerController playerControllerPrefab;
        public static ResourcesManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            if (Instance != this)
            {
                Destroy(this);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}