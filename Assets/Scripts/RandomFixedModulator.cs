﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    [ExecuteInEditMode]
    [AddComponentMenu("Modulator/Random Fixed Modulator", -80)]
    public class RandomFixedModulator : Modulator
    {
        public bool debug = false;

        private System.Random randomizer = new System.Random();
        private float value = 0.0f;

        protected override void Awake()
        {
            value = (float)randomizer.NextDouble();
        }

        void Update()
        {

            if (debug)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Trigger();
                }
            }
        }

        public override void Trigger()
        {
            value = (float)randomizer.NextDouble();
        }

        public override float GetValue()
        {
            return (value * 2.0f - 1.0f) * GetModulatedAmp();
        }

        public override float GetPositiveValue()
        {
            return value * GetModulatedAmp();
        }
    }
}
