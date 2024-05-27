using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace WatermelonGameClone
{
    public interface INextSpherePanelView
    {
        // Methods related to UI updates
        void CreateNextSphereImages();
        void UpdateNextSphereImages(int sphereIndex);
    }
}
