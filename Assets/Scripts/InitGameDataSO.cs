using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "InitGameDataSO", menuName = "Scriptable Objects/InitGameDataSO")]
public class InitGameDataSO : ScriptableObject
{
    public int moneyCount = 300;
    public int livesCount = 5;
    public int maxLivesCount = 5;
    public int starCount = 8;

    public bool[] footerButtonsUnlockStates = { false, true, true, true, false };

    public bool soundOn = true;
    public bool musicOn = true;
    public bool vibrationOn = true;
    public bool notifsOn = true;
    public LanguageEnum languageIndex = 0;
}
