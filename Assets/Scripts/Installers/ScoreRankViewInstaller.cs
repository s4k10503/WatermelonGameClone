using UnityEngine;
using System;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class ScoreRankViewInstaller : MonoInstaller
    {
        // Daily
        [SerializeField] GameObject _panelDailyRanking;
        [SerializeField] GameObject _textDailyScoreRank1;
        [SerializeField] GameObject _textDailyScoreRank2;
        [SerializeField] GameObject _textDailyScoreRank3;

        // Monthly
        [SerializeField] GameObject _panelMonthlyRanking;
        [SerializeField] GameObject _textMonthlyScoreRank1;
        [SerializeField] GameObject _textMonthlyScoreRank2;
        [SerializeField] GameObject _textMonthlyScoreRank3;

        // AllTime
        [SerializeField] GameObject _panelAllTimeRanking;
        [SerializeField] GameObject _textAllTimeScoreRank1;
        [SerializeField] GameObject _textAllTimeScoreRank2;
        [SerializeField] GameObject _textAllTimeScoreRank3;

        // Current
        [SerializeField] GameObject _textCurrentScore;

        public override void InstallBindings()
        {
            // Daily
            Container
                .Bind<GameObject>()
                .WithId("PanelDailyRanking")
                .FromInstance(_panelDailyRanking);
            Container
                .Bind<GameObject>()
                .WithId("TextDailyScoreRank1")
                .FromInstance(_textDailyScoreRank1);
            Container
                .Bind<GameObject>()
                .WithId("TextDailyScoreRank2")
                .FromInstance(_textDailyScoreRank2);
            Container
                .Bind<GameObject>()
                .WithId("TextDailyScoreRank3")
                .FromInstance(_textDailyScoreRank3);

            // Monthly
            Container
                .Bind<GameObject>()
                .WithId("PanelMonthlyRanking")
                .FromInstance(_panelMonthlyRanking);
            Container
                .Bind<GameObject>()
                .WithId("TextMonthlyScoreRank1")
                .FromInstance(_textMonthlyScoreRank1);
            Container
                .Bind<GameObject>()
                .WithId("TextMonthlyScoreRank2")
                .FromInstance(_textMonthlyScoreRank2);
            Container
                .Bind<GameObject>()
                .WithId("TextMonthlyScoreRank3")
                .FromInstance(_textMonthlyScoreRank3);

            // AllTime
            Container
                .Bind<GameObject>()
                .WithId("PanelAllTimeRanking")
                .FromInstance(_panelAllTimeRanking);
            Container
                .Bind<GameObject>()
                .WithId("TextAllTimeScoreRank1")
                .FromInstance(_textAllTimeScoreRank1);
            Container
                .Bind<GameObject>()
                .WithId("TextAllTimeScoreRank2")
                .FromInstance(_textAllTimeScoreRank2);
            Container
                .Bind<GameObject>()
                .WithId("TextAllTimeScoreRank3")
                .FromInstance(_textAllTimeScoreRank3);

            // Current
            Container
                .Bind<GameObject>()
                .WithId("TextCurrentScore")
                .FromInstance(_textCurrentScore);
        }
    }
}