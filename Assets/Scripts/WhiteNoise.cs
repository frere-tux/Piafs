using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    class WhiteNoise : Modulator
    {
        private Random randomizer = new Random();

        public override float GetValue()
        {
            double randValue = randomizer.NextDouble();
            return ((float)randValue * 2.0f - 1.0f) * GetModulatedAmp();
        }

        public override float GetPositiveValue()
        {
            double randValue = randomizer.NextDouble();
            return (float)randValue * GetModulatedAmp();
        }
    }
}
