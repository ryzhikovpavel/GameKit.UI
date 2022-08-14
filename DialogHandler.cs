using System;
using System.Threading.Tasks;
using GameKit.UI.Implementation;
using UnityEngine;

namespace GameKit.UI
{ 
    public interface IDialogHandlerCommand
    {
        void Abort(string error);
        void Cancel();
        void Complete();
    }
    
    public interface IDialogResult<T>
    {
        T Result { get; }
    }
    
    public class DialogHandler: TransitionHandler<ViewDialog>, IDialogHandlerCommand
    {
        public event Action EventCompleted;
        
        public string Error { get; protected set; }
        public bool IsCanceled { get; private set; }
        public bool IsFaulted { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsCompletedSuccessfully => IsCompleted && !IsFaulted;
        public bool SkipAnimation { get; set; } = false;

        protected override bool KeepWaiting => !(IsCompleted && (SkipAnimation || !IsTransition) );

        public DialogHandler(ViewDialog view)
        {
            SetView(view);
        }
        
        void IDialogHandlerCommand.Abort(string error)
        {
            Error = error;
            IsFaulted = true;
            IsCompleted = true;
        }

        void IDialogHandlerCommand.Cancel()
        {
            IsCanceled = true;
            IsCompleted = true;
        }

        void IDialogHandlerCommand.Complete()
        {
            IsCompleted = true;
            DoComplete();
        }

        protected virtual void DoComplete()
        {
            EventCompleted?.Invoke();
        }
    }
    
    public class DialogHandler<T>: DialogHandler, IDialogResult<T>
    {
        public new event Action<T> EventCompleted;
        
        private IDialogResult<T> _dialogResult;
        public T Result {get; private set; }
        
        public DialogHandler(ViewDialog dialog, IDialogResult<T> dialogResult) : base(dialog)
        {
            _dialogResult = dialogResult;
        }

        protected override void DoComplete()
        {
            Result = _dialogResult.Result;
            _dialogResult = null;
            EventCompleted?.Invoke(Result);
            base.DoComplete();
        }
    }
}