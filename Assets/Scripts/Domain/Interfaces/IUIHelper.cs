using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace WatermelonGameClone
{
    public interface IUIHelper
    {
        void UpdateCurrentScoreText(TextMeshProUGUI textMesh, int score);
        void UpdateScoreRankPanelTexts(GameObject[] rankTexts, List<int> scores);
        void UpdateTitlePanelText(GameObject textPanelTitle, string titleText);
    }
}
