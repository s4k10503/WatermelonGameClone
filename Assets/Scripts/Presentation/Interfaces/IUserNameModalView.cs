using System;

namespace Presentation.Interfaces
{
    public interface IUserNameModalView
    {
        IObservable<string> OnUserNameSubmit { get; }
        void ShowModal();
        void HideModal();
    }
}
