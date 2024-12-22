namespace WatermelonGameClone.Presentation
{
    // Base class for MainScene ViewState Handlers
    public abstract class MainSceneViewStateHandlerBase : IMainSceneViewStateHandler
    {
        public virtual void Apply(MainSceneView view, MainSceneViewStateData data)
        {
            ResetAllUI(view);
            ApplyCustomState(view, data);
        }

        protected virtual void ResetAllUI(MainSceneView view)
        {
            view.HideLoadingPage();
            view.BackgroundPanelView.HidePanel();
            view.PausePanelView.HidePanel();
            view.GameOverPanelView.HidePanel();
            view.DetailedScoreRankView.HidePanel();
            view.Stageview.ShowStage();
            view.ShowMainPageMainElements();
        }

        protected abstract void ApplyCustomState(MainSceneView view, MainSceneViewStateData data);
    }

    // Base class for TitleScene ViewState Handlers
    public abstract class TitleSceneViewStateHandlerBase : ITitleSceneViewStateHandler
    {
        public virtual void Apply(TitleSceneView view, TitleSceneViewStateData data)
        {
            ResetAllUI(view);
            ApplyCustomState(view, data);
        }

        protected virtual void ResetAllUI(TitleSceneView view)
        {
            view.HideLoadingPage();
            view.DetailedScoreRankView.HidePanel();
            view.SettingsPanelView.HidePanel();
            view.LicenseModalView.HideModal();
            view.ShowTitlePageMainElements();
        }

        protected abstract void ApplyCustomState(TitleSceneView view, TitleSceneViewStateData data);
    }
}
