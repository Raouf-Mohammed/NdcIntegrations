using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NdcIntegrations.Domain.Indigo;

namespace NdcIntegrations.Core.Indigo
{
    public class FareConfirmData
    {
        public List<SegmentData> SegmentsData { get; set; }
        public Dictionary<string, int> Passanager { get; set; }
    }
}
