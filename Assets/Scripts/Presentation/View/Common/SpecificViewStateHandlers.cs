namespace WatermelonGameClone.Presentation
{
    // MainScene Specific Handlers
    public class MainSceneLoadingViewStateHandler : MainSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(MainSceneView view, MainSceneViewStateData data)
        {
            view.Stageview.HideStage();
            view.HideMainPageMainElements();
            view.ShowLoadingPage();
        }
    }

    public class PlayingViewStateHandler : MainSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(MainSceneView view, MainSceneViewStateData data)
        {
            // Playing state is the default state set by ResetAllUI.
        }
    }

    public class PausedViewStateHandler : MainSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(MainSceneView view, MainSceneViewStateData data)
        {
            view.BackgroundPanelView.ShowPanel();
            view.PausePanelView.ShowPanel();
        }
    }

    public class GameOverViewStateHandler : MainSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(MainSceneView view, MainSceneViewStateData data)
        {
            view.BackgroundPanelView.ShowPanel();
            view.GameOverPanelView.ShowPanel(data.CurrentScore, data.Screenshot, data.ScoreContainer);
        }
    }

    public class MainSceneDetailedScoreViewStateHandler : MainSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(MainSceneView view, MainSceneViewStateData data)
        {
            view.Stageview.HideStage();
            view.HideMainPageMainElements();
            view.GameOverPanelView.HidePanel();
            view.DetailedScoreRankView.ShowPanel();
        }
    }

    // TitleScene Specific Handlers
    public class TitleSceneLoadingViewStateHandler : TitleSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(TitleSceneView view, TitleSceneViewStateData data)
        {
            view.ShowLoadingPage();
        }
    }

    public class TitleSceneDetailedScoreViewStateHandler : TitleSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(TitleSceneView view, TitleSceneViewStateData data)
        {
            view.DetailedScoreRankView.ShowPanel();
            view.DetailedScoreRankView.DisplayTopScores(data.ScoreContainer);
        }
    }

    public class SettingsViewStateHandler : TitleSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(TitleSceneView view, TitleSceneViewStateData data)
        {
            view.SettingsPanelView.ShowPanel();
        }
    }

    public class LicenseViewStateHandler : TitleSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(TitleSceneView view, TitleSceneViewStateData data)
        {
            view.LicenseModalView.ShowModal();
            view.LicenseModalView.DisplayLicenses(data.Licenses);
        }
    }

    public class TitleViewStateHandler : TitleSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(TitleSceneView view, TitleSceneViewStateData data)
        {
            view.ShowTitlePageMainElements();
        }
    }
}
