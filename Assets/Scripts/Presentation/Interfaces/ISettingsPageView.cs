using System;
using UniRx;

namespace WatermelonGameClone.Presentation
{
    public interface ISettingsPageView
    {
        IReadOnlyReactiveProperty<float> ValueBgm { get; }
        IReadOnlyReactiveProperty<float> ValueSe { get; }
        IObservable<Unit> OnUserNameChange { get; }
        IObservable<Unit> OnBack { get; }

        void SetBgmSliderValue(float value);
        void SetSeSliderValue(float value);
        void SetUserName(string userName);
        void ShowPage();
        void HidePage();
    }
}
