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
        [Header("-- Interface --")]
        public GameObject movingPart;
        public float fullDragWorldDistance;
        public float sliderSmoothing = 0.0033f;
        public float sliderSmoothThreshold = 0.001f;
        [Header("-- Level design --")]
        public float rightValue;
        public float min, max, step;

        private Collider2D col;
        private float yDragStart;
        private Vector3 movingPartOrigin;
        private float fullDragDistance;
        private float steppedValue;
        private float smoothedValue;

        void Start()
        {
            col = GetComponent<Collider2D>();
            movingPartOrigin = movingPart.transform.position;
            fullDragDistance = Camera.main.WorldToScreenPoint(Vector3.up * fullDragWorldDistance).y * 0.22f;
            if(fixedModulator != null)fixedModulator.SetSmoothing(sliderSmoothing,(max-min)* sliderSmoothThreshold);
            Refresh();
        }

        void Update()
        {
            sliderValue = Toolkit.Damp(sliderValue,steppedValue / (max- min),1f,Time.deltaTime);
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

        public void RefreshSteppedValue()
        {
            steppedValue = Mathf.Round(sliderValue * (max - min) / step) * step;
        }

        public void RefreshVisual()
        {
            movingPart.transform.position = movingPartOrigin + Vector3.up * fullDragWorldDistance * smoothedValue;
        }

        public void RefreshFixedModulator()
        {
            if(fixedModulator != null)fixedModulator.SetValueSmooth(Mathf.Lerp(min, max, smoothedValue));
        }

        public void SolveValue()
        {
            steppedValue = rightValue - min;
            sliderValue = steppedValue / (max - min);
            smoothedValue = sliderValue;
        }

        public override bool IsSolved()
        {
            return steppedValue+min == rightValue;
        }
    }
}

