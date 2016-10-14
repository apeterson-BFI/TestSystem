using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.BBJSON
{
    public class FaceData
    {
        public HeadData head { get; set; }
        public List<EyebrowsData> eyebrows { get; set; }
        public List<EyesData> eyes { get; set; }
        public NoseData nose { get; set; }
        public MouthData mouth { get; set; }
        public HairData hair { get; set; }
        public double fatness { get; set; }
        public string color { get; set; }
    }
}
