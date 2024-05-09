using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace WatermelonGameClone
{
    public interface IScorePanelView
    {
        void UpdateCurrentScore(int score);
        void UpdateBestScore(int score);
    }
}