using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.Evol
{
    public class LocationSet
    {
        public List<Org> orgList { get; set; }

        public LocationSet parentLocationSet { get; set; }
        public LocationSet[,] subLocationSets { get; set; }

        public LocationSet(List<Org> orgList)
        {
            this.orgList = orgList;
        }

        public LocationSet(List<Org> orgList, LocationSet parent)
        {
            this.orgList = orgList;
            this.parentLocationSet = parent;
        }

        public void classify(int maxPerLSet)
        {
            if(orgList.Count > maxPerLSet)
            {
                double xmean = orgList.Average(o => o.x);
                double ymean = orgList.Average(o => o.y);

                if(orgList.All(o => o.x == xmean) && orgList.All(o => o.y == ymean))
                {
                    // everything is the same so we can't classify
                    return;
                }

                subLocationSets = new LocationSet[2, 2];

                List<Org> llList = new List<Org>();
                List<Org> luList = new List<Org>();
                List<Org> ulList = new List<Org>();
                List<Org> uuList = new List<Org>();

                Org org;

                for(int i = 0; i < orgList.Count; i++)
                {
                    org = orgList[i];

                    if(org.x < xmean)
                    {
                        if(org.y < ymean)
                        {
                            llList.Add(org);
                        }
                        else
                        {
                            luList.Add(org);
                        }
                    }
                    else
                    {
                        if(org.y < ymean)
                        {
                            ulList.Add(org);
                        }
                        else
                        {
                            uuList.Add(org);
                        }
                    }
                }

                subLocationSets[0,0] = new LocationSet(llList, this);
                subLocationSets[0,1] = new LocationSet(luList, this);
                subLocationSets[1,0] = new LocationSet(ulList, this);
                subLocationSets[1,1] = new LocationSet(uuList, this);

                subLocationSets[0, 0].classify(maxPerLSet);
                subLocationSets[0, 1].classify(maxPerLSet);
                subLocationSets[1, 0].classify(maxPerLSet);
                subLocationSets[1, 1].classify(maxPerLSet);
            }
        }
    }
}
