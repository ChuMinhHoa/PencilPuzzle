using System;
using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Other
{
    public class UIOfferShow : MonoBehaviour
    {
        [field: SerializeField] private ScrollRectDragCheck ScrollRectDragCheck { get; set; }
        [field: SerializeField] public ScrollRect BundlesScrollRect { get; private set; }
        [field: SerializeField] private int BundleCount { get; set; }
        private MotionHandle MoveBundleHandle { get; set; }

        private void Start()
        {
            RunBundlesAnimation();
        }

        private void RunBundlesAnimation()
        {
            LMotion.Create(0f, 1f, 5f)
                .WithLoops(-1)
                .WithOnLoopComplete(TryChangeBundle)
                .RunWithoutBinding()
                .AddTo(this);
        }

        private void TryChangeBundle(int loopCount)
        {
            if (ScrollRectDragCheck.IsDragging) return;
            float space = 1f / (BundleCount - 1);
            int bundleIndex = loopCount % BundleCount;
            float currentPos = BundlesScrollRect.horizontalNormalizedPosition;
            float nextPos = bundleIndex * space;
            MoveBundleHandle.TryCancel();
            MoveBundleHandle = LMotion.Create(currentPos, nextPos, 0.5f)
                .WithEase(Ease.InOutCubic)
                .Bind(UpdateBundlePosition)
                .AddTo(this);
        }

        private void UpdateBundlePosition(float value)
        {
            if (ScrollRectDragCheck.IsDragging)
            {
                MoveBundleHandle.TryCancel();
                return;
            }

            BundlesScrollRect.horizontalNormalizedPosition = value;
        }
    }
}