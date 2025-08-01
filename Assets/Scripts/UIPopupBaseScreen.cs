using System.Collections.Generic;
using UnityEngine;

public class UIPopupBaseScreen : UIScreen
{
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
