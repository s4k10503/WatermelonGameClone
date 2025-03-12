using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Presentation.Interfaces
{
    public interface IUIHelper
    {
        void UpdateCurrentScoreText(TextMeshProUGUI textMesh, int score);
        void UpdateScoreRankPanelTexts(GameObject[] rankTexts, List<int> scores);
        void UpdateTitlePanelText(GameObject textPanelTitle, string titleText);
    }
}
