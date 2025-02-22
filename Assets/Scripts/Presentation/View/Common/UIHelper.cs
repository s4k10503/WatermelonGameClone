using Presentation.Interfaces;

using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Presentation.View.Common
{
    public sealed class UIHelper : IUIHelper
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
            if (textPanelTitle == null)
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                Debug.LogWarning("textPanelTitle is null or has been destroyed.");
#endif
                return;
            }

            var textComponent = textPanelTitle.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.SetText(titleText);
            }
            else
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                Debug.LogWarning("TextMeshProUGUI component is missing.");
#endif
            }
        }
    }
}
