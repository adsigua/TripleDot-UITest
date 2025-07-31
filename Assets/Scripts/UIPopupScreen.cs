using System.Collections.Generic;
using UnityEngine;

public class UIPopupScreen : UIScreen
{
    [SerializeField] private readonly List<UIScreen> _popupScreens;


    public override void RegisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUISettingsScreenEventsListener listener)
            return;
    }

    public override void UnregisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUISettingsScreenEventsListener listener)
            return;
    }
}
