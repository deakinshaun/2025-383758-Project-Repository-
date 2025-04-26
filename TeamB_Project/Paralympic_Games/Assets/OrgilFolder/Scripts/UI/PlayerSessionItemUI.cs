using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OrgilFolder.Scripts.UI
{
    public class PlayerSessionItemUI : NetworkBehaviour
    {
        public TMP_Text usernameText;
        public Image avatar;
        private PlayerObject _player = null;

        PlayerObject Player
        {
            get
            {
                if (_player == null) _player = PlayerRegistry.GetPlayer(Object.InputAuthority);
                return _player;
            }
        }
        public override void Spawned()
        {
            Init();
            UpdateStats();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            Player.OnStatChanged -= UpdateStats;
        }

        private void Init()
        {
            Player.OnStatChanged += UpdateStats;
        }
        void UpdateStats()
        {
            usernameText.text = Player.Nickname;
            SetColor(Player.Color);

        }
        public void SetColor(Color color)
        {
            usernameText.color = color;
            avatar.color = color;
        }
    }
}