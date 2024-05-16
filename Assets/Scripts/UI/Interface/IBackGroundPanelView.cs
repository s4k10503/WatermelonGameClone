using System;
using UniRx;
using UnityEngine;

namespace WatermelonGameClone
{
    public interface IBackgroundPanelView
    {
        void ShowPanel();
        void HidePanel();
    }
}