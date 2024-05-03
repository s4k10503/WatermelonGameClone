using UnityEngine;
using System;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class ScoreRankViewInstaller : MonoInstaller
    {
        [SerializeField] GameObject _rankingPanel;

        [SerializeField] GameObject _textScoreRank1;
        [SerializeField] GameObject _textScoreRank2;
        [SerializeField] GameObject _textScoreRank3;
        [SerializeField] GameObject _textScoreCurrent;

        public override void InstallBindings()
        {
            Container
                .Bind<GameObject>()
                .WithId("RankingPanel")
                .FromInstance(_rankingPanel);

            Container
                .Bind<GameObject>()
                .WithId("TextScoreRank1")
                .FromInstance(_textScoreRank1);

            Container
                .Bind<GameObject>()
                .WithId("TextScoreRank2")
                .FromInstance(_textScoreRank2);

            Container
                .Bind<GameObject>()
                .WithId("TextScoreRank3")
                .FromInstance(_textScoreRank3);

            Container
                .Bind<GameObject>()
                .WithId("TextScoreCurrent")
                .FromInstance(_textScoreCurrent);
        }

    }
}