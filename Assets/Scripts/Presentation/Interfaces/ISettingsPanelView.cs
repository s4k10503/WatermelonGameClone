using System;
using UniRx;

namespace WatermelonGameClone.Presentation
{
    public interface ISettingsPanelView
    {
        IReadOnlyReactiveProperty<float> ValueBgm { get; }
        IReadOnlyReactiveProperty<float> ValueSe { get; }
        IObservable<Unit> OnBack { get; }

        void SetBgmSliderValue(float value);
        void SetSeSliderValue(float value);
        void ShowPanel();
        void HidePanel();
    }
}
