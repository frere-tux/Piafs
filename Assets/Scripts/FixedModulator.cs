using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    class FixedModulator : Modulator
    {
        public override float GetValue()
        {
            return GetModulatedAmp();
        }

        public override float GetPositiveValue()
        {
            return GetValue();
        }
    }
}
