using _Game.Scripts.UI.Core;
using Core.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour
{
    public Button button;
    public bool instantClick;
    public bool showAnim = true;
    UnityAction action;
    
    public virtual void Start()
    {
        button ??= GetComponent<Button>();
        button.SetOnClickDestination(OnClick);
    }

    public void AddListener(UnityAction action)
    {
        this.action = action;
    }
    
    void OnClick()
    {
        if (instantClick)
        {
            if (showAnim)
            {
                UIAnimationBase.ButtonBasic(transform).Forget();
            }
            StartAction();
        }
        else
        {
            UIAnimationBase.ButtonBasic(transform, StartAction).Forget();
        }
    }

    void StartAction()
    {
        if(CheckCanAction())
            action?.Invoke();
    }

    public virtual bool CheckCanAction()
    {
        return true;
    }
    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
    }
}
