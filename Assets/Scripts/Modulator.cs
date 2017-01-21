using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    public abstract class Modulator : MonoBehaviour
    {
        public abstract float GetValue();
        public abstract float GetPositiveValue();
    }
}
