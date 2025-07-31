using UnityEngine;

public abstract class UIScreen : MonoBehaviour
{
    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }

    public virtual void InitScreen(UIScreenInitData uiScreenInitData)
    {
        
    }

    public virtual void ShowScreen()
    {
        gameObject.SetActive(true);
    }

    public virtual void HideScreen()
    {
        gameObject.SetActive(false);
    }

    public abstract void RegisterEventsListener<T>(T objectListener) where T : class;
    public abstract void UnregisterEventsListener<T>(T objectListener) where T : class;
}
