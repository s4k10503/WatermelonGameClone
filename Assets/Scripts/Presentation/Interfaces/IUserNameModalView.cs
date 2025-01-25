using System;
using UniRx;

namespace WatermelonGameClone.Presentation
{
    public interface IUserNameModalView
    {
        IObservable<string> OnUserNameSubmit { get; }
        void ShowModal();
        void HideModal();
    }
}
