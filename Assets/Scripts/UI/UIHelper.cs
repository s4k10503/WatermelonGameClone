using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace WatermelonGameClone
{
    public class UIHelper : IUIHelper
    {
        public void UpdateCurrentScoreText(TextMeshProUGUI textMesh, int score)
        {
            textMesh.SetText(score.ToString());
        }

        public void UpdateScoreRankPanelTexts(GameObject[] rankTexts, List<int> scores)
        {
            for (int i = 0; i < rankTexts.Length; i++)
            {
                string scoreText = (scores.Count > i && rankTexts[i] != null) ? scores[i].ToString() : "--";
                rankTexts[i]
                    .GetComponent<TextMeshProUGUI>()
                    .SetText(scoreText);
            }
        }

        public void UpdateTitlePanelText(GameObject textPanelTitle, string titleText)
        {
            textPanelTitle
                .GetComponent<TextMeshProUGUI>()
                .SetText(titleText);
        }
    }
}
