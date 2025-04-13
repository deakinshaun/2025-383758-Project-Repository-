using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OrgilFolder.Scripts.UI
{
    public class GameInfoUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text gameModeName;
        [SerializeField] private TMP_Text maxPlayers;
        [SerializeField] private TMP_Text gameModeDescription;
        [SerializeField] private Image previewImage;

        public void Initialize(GameProperty gameProperty)
        {
            gameModeName.SetText(gameProperty.gameModeName);
            maxPlayers.SetText($"Max Players:{gameProperty.maxPlayerCount}");
            gameModeDescription.SetText(gameProperty.description);
            previewImage.sprite =
                Sprite.Create(gameProperty.previewImage,
                    new Rect(0.0f, 0.0f, gameProperty.previewImage.width, gameProperty.previewImage.height),
                    new Vector2(0.5f, 0.5f));
        }
    }
}