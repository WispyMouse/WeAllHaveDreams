using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    public abstract class GambitCost
    {
        public abstract float GetMagnitude(AIConfiguration aiConfiguration);

        public abstract bool CostCanBePaid(WorldContext worldContextInstance);
    }
}
