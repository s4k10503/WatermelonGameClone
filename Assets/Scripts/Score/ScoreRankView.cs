using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Zenject;

namespace WatermelonGameClone
{
    public class ScoreRankView : MonoBehaviour, IScoreRankView
    {
        // Objects
        private GameObject _rankingPanel;

        private GameObject _textScoreRank1;
        private GameObject _textScoreRank2;
        private GameObject _textScoreRank3;
        private GameObject _textScoreCurrent;

        // Subjects

        // Observables


        [Inject]
        public void Construct(
            [Inject(Id = "CanvasTransform")] Transform canvasTransform,
            [Inject(Id = "RankingPanel")] GameObject rankingPanel,
            [Inject(Id = "TextScoreRank1")] GameObject textScoreRank1,
            [Inject(Id = "TextScoreRank2")] GameObject textScoreRank2,
            [Inject(Id = "TextScoreRank3")] GameObject textScoreRank3,
            [Inject(Id = "TextScoreCurrent")] GameObject textScoreCurrent)
        {
            _rankingPanel = rankingPanel;
            _textScoreRank1 = textScoreRank1;
            _textScoreRank2 = textScoreRank2;
            _textScoreRank3 = textScoreRank3;
            _textScoreCurrent = textScoreCurrent;
        }

        private void OnDestroy()
        {

        }

        public void UpdateCurrentScore(int currentScore)
        {
            _textScoreCurrent
                .GetComponent<TextMeshProUGUI>()
                .SetText(currentScore.ToString());
        }

        public void DisplayTopScores(List<int> scores)
        {
            GameObject[] rankTexts = { _textScoreRank1, _textScoreRank2, _textScoreRank3 };

            for (int i = 0; i < rankTexts.Length; i++)
            {
                string scoreText = (scores.Count > i && rankTexts[i] != null) ? scores[i].ToString() : "--";
                rankTexts[i].GetComponent<TextMeshProUGUI>().SetText(scoreText);
            }
        }
    }
}
