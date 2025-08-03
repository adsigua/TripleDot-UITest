using AdoTrpDotTest.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdoTrpDotTest
{
    [CreateAssetMenu(fileName = "InitGameDataSO", menuName = "Scriptable Objects/InitGameDataSO")]
    public class InitGameDataSO : ScriptableObject
    {
        [FormerlySerializedAs("moneyCount")] public int coinsCount = 300;
        public int livesCount = 5;
        public int maxLivesCount = 5;
        public int starCount = 8;

        public bool[] footerButtonsLockStates = { true, false, false, false, true };

        public bool soundOn = true;
        public bool musicOn = true;
        public bool vibrationOn = true;
        public bool notifsOn = true;
        public LanguageEnum languageIndex = 0;

        public int rewardStarCount = 20;
        public int rewardCoinCount = 100;
        public int rewardCrownCount = 8;
    }
}
