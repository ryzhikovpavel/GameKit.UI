using System;
using GameKit.UI.Implementation;
using UnityEngine;

namespace GameKit.UI
{
    public abstract class ViewDialog: ViewBlockable
    {
        private DialogHandler _handler;
        
        private void Close()
        {
            if (IsDisplayed == false)
            {
                Debug.LogWarning($"Dialog {name} is not displayed");
            }
            else
            {
                HideInternal(null);
            }
        }

        protected THandler Show<THandler>(THandler handler) where THandler: DialogHandler
        {
            _handler = handler;
            Service<UiManagementSystem>.Instance.NotifyDialogOpen(this);
            ShowInternal();
            return handler;
        }
        
        protected DialogHandler Show() => Show(new DialogHandler(this));

        protected internal override void HideComplete()
        {
            if (Loop.IsQuitting == false) Service<UiManagementSystem>.Instance.NotifyDialogClose(this);
        }

        private void OnDisable()
        {
            if (State is ViewState.Displayed or ViewState.Showing or ViewState.Suspended) 
                CloseAbort($"{name} view disable");
        }

        public void CloseAbort(string error)
        {
            Close();
            ((IDialogHandlerCommand)_handler).Abort(error);
        }

        public void CloseCancel()
        {
            Close();
            ((IDialogHandlerCommand)_handler).Cancel();
        }

        protected void CloseComplete()
        {
            Close();
            ((IDialogHandlerCommand)_handler).Complete();
        }
    }
    
    public abstract class ViewDialogWithResult<TResult> : ViewDialog, IDialogResult<TResult>
    {
        public TResult Result { get; protected set; }
        protected new DialogHandler<TResult> Show() => Show(new DialogHandler<TResult>(this, this));
    }
}