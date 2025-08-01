using UnityEngine;

public class UIScreenInitData
{
    
}

public class UIHomeScreenInitData : UIScreenInitData
{
    public int moneyCount;
    public int livesCount;
    public int maxLivesCount;
    public int starCount;
    public bool[] footerButtonsLockStates;

    public void InitData()
    {
        moneyCount = 300;
        livesCount = 5;
        maxLivesCount = 5;
        starCount = 8;
        footerButtonsLockStates = new bool[] {true, false, false, false, true};
    }
    
    public void SetDataFromSO(InitGameDataSO initGameDataSo)
    {
        moneyCount = initGameDataSo.coinsCount;
        livesCount = initGameDataSo.livesCount;
        maxLivesCount = initGameDataSo.maxLivesCount;
        starCount = initGameDataSo.starCount;
        footerButtonsLockStates = initGameDataSo.footerButtonsLockStates;
    }
}

public class UISettingsScreenInitData : UIScreenInitData
{
    public bool soundOn;
    public bool musicOn;
    public bool vibrationOn;
    public bool notifsOn;
    public int languageIndex;
}

public enum LanguageEnum
{
    English, Swedish, Spanish, Philippines
}