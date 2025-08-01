using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "InitGameDataSO", menuName = "Scriptable Objects/InitGameDataSO")]
public class InitGameDataSO : ScriptableObject
{
    [FormerlySerializedAs("moneyCount")] public int coinsCount = 300;
    public int livesCount = 5;
    public int maxLivesCount = 5;
    public int starCount = 8;

    public bool[] footerButtonsLockStates = {true, false, false, false, true};

    public bool soundOn = true;
    public bool musicOn = true;
    public bool vibrationOn = true;
    public bool notifsOn = true;
    public LanguageEnum languageIndex = 0;
}
