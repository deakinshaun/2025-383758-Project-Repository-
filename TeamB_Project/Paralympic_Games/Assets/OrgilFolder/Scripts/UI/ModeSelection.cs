using UnityEngine;

namespace OrgilFolder.Scripts.UI
{
    public class ModeSelection : MonoBehaviour
    {
        public void SetNickName(string name)
        {
            UserData.Nickname = name;
        }
    }
}