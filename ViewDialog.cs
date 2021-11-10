using System;
using GameKit;
using GameKit.UI.Core;
using UnityEngine;

namespace GameKit.UI
{
    public abstract class ViewDialogBlank: ViewBlockable
    {
        public void Close(){ Close(null);}
        public void Close(Action onDisable)
        {
            if (IsDisplayed == false)
            {
                Debug.LogWarning($"Dialog {name} is not displayed");
            }
            else
            {
                Service<UiManagementSystem>.Instance.NotifyDialogClose(this);
                HideInternal(onDisable);
            }
        }

        protected void Show()
        {
            Service<UiManagementSystem>.Instance.NotifyDialogOpen(this);
            ShowInternal();
        }
    }
    
    public abstract class ViewDialog : ViewDialogBlank
    {
        public void Open()
        {
            Show();
        }
    }
    
    public abstract class ViewDialog<TParam> : ViewDialogBlank
    {
        public void Open(TParam param)
        {
            Param = param;
            Show();
        }
        
        public TParam Param { get; private set; }
    }
}