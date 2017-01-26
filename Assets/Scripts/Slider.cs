using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    public class Slider : LevelControls
    {
        [Header("-- Runtime --")]
        public float sliderValue;
        public FixedModulator fixedModulator;
        public List<FixedModulator> fixedModulators;
        [Header("-- Interface --")]
        public GameObject movingPart;
        public float fullDragWorldDistance;
        public float fullDragDistance;
        public float sliderSmoothing = 0.0033f;
        public float sliderSmoothThreshold = 0.001f;
        [Header("-- Level design --")]
        public float rightValue;
        public float min, max, step;

        private Collider2D col;
        private float yDragStart;
        private Vector3 movingPartOrigin;
        private float steppedValue;
        private int stepValue;
        public float SteppedValue
        {
            get { return steppedValue; }
        }
        private float smoothedValue;

        void Start()
        {
            col = GetComponent<Collider2D>();
            movingPartOrigin = movingPart.transform.localPosition;
            if (fixedModulators != null)
            {
                foreach(FixedModulator f in fixedModulators)
                {
                    f.SetSmoothing(sliderSmoothing, (max - min) * sliderSmoothThreshold);
                }
            }
            Refresh();
        }

        void Update()
        {
            sliderValue = steppedValue / (max- min);
            //if (Mathf.Abs(sliderValue - steppedValue / (max - min)) > 0.05f) sliderValue = steppedValue / (max - min) + 0.05f * Mathf.Sign(sliderValue - steppedValue);
            smoothedValue = Toolkit.Damp(smoothedValue, sliderValue, 0.99999f,Time.deltaTime);
            if (Mathf.Abs(smoothedValue - sliderValue) < sliderSmoothThreshold) smoothedValue = sliderValue;
            RefreshFixedModulator();
            RefreshVisual();
        }

        public void OnMouseDown()
        {
            //Debug.Log("mousedown");
            Vector2 startDragPos = Input.mousePosition;
            yDragStart = (startDragPos.y - fullDragDistance * sliderValue);
            
        }

        public void OnMouseDrag()
        {
            sliderValue = Mathf.InverseLerp(yDragStart, yDragStart + fullDragDistance, Input.mousePosition.y);
            Refresh();
        }

        public void Refresh()
        {
            RefreshVisual();
            RefreshFixedModulator();
            RefreshSteppedValue();
        }

        public void SetValueHard(float v)
        {
            v = Mathf.Clamp(v,min, max);
            steppedValue = v - min;
            sliderValue = steppedValue / (max - min);
            RefreshSteppedValue();
            sliderValue = steppedValue / (max - min);
            smoothedValue = sliderValue;
            //RefreshVisual();
            RefreshFixedModulator();
            fixedModulators.ForEach(a => a.JumpToRawValue());
        }

        public void RefreshSteppedValue()
        {
            stepValue = Mathf.RoundToInt(sliderValue * (max - min) / step);
            steppedValue = Mathf.Round(sliderValue * (max - min) / step) * step;
        }

        public void RefreshVisual()
        {
            movingPart.transform.localPosition = movingPartOrigin + Vector3.up * fullDragWorldDistance * smoothedValue;
        }

        public void RefreshFixedModulator()
        {
            if(fixedModulators != null)fixedModulators.ForEach(a => a.SetValueSmooth(Mathf.Lerp(min, max, smoothedValue)));
        }

        public void SolveValue()
        {
            steppedValue = rightValue - min;
            sliderValue = steppedValue / (max - min);
            smoothedValue = sliderValue;
            RefreshSteppedValue();
        }

        public override bool IsSolved()
        {
            bool result = stepValue  == Mathf.Round((rightValue - min) / step);
            return result;
        }
    }
}

