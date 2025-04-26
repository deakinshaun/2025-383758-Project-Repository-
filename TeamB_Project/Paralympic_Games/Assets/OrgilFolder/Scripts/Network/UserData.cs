
    using UnityEngine;
    public static class UserData
    {
        private const string KEY = "Nickname";
        public static string Nickname
        {
            get
            {
                return PlayerPrefs.GetString(KEY);
            }
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    PlayerPrefs.DeleteKey(KEY);
                else
                {
                    PlayerPrefs.SetString(KEY,value);
                }
            }
        }

        public static bool HasNickName => PlayerPrefs.HasKey(KEY);

    }
