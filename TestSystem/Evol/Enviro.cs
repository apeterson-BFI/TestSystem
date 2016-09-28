using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.Evol
{
    public class Enviro
    {
        public extree refexp;
        public double xstart;
        public double xend;

        public bool watch;

        public List<Org> orgs;

        public Random rnm;

        public int regenRate;

        public const int spawnRate = 1;
        public const double speed = 5.0;
        public const int nodeMax = 99;

        public double minDim = 0.0;
        public double maxDim = 500.0;
        public const double findDist = 2.0;

        public const int comboRate = 1;
        public const int basicRate = 1;
        public const int constRate = 1;
        public const int xRate = 1;

        public const double jitter = 0.0001;

        public Enviro(extree refexp, double xstart, double xend)
        {
            this.refexp = refexp;
            this.xstart = xstart;
            this.xend = xend;
            this.rnm = new Random();
            this.watch = true;

            this.regenRate = 1;

            orgs = new List<Org>();
            
            for(int i = 0; i < 20; i++)
            {
                spawnRandom();
            }
        }

        public Tuple<double, double> findCentroid()
        {
            double x = orgs.Average(o => o.x);
            double y = orgs.Average(o => o.y);

            return new Tuple<double, double>(x, y);
        }

        public Tuple<double, double> findDeadCentroid()
        {
            if(orgs.Count(o => !o.alive) == 0)
            {
                return new Tuple<double, double>(0.0, 0.0);
            }
            else
            {
                var od = orgs.Where(o => !o.alive);

                double x = od.Average(o => o.x);
                double y = od.Average(o => o.y);

                return new Tuple<double, double>(x, y);
            }
        }

        public void run()
        {
            int t = 1;
            int sleep = 0;

            string stext;

            int a;
            int d;

            while(Org.maxID < 1000000)
            {
                a = orgs.Count(x => x.alive);
                d = orgs.Count(x => !x.alive);

                if (watch)
                {
                    Console.WriteLine(string.Format("Turn: {0}, {1} alive, {2} dead", t, a, d));
                    
                    stext = Console.ReadLine();

                    if(stext == "all")
                    {
                        foreach(Org o in orgs)
                        {
                            o.basis.ScoreSqrt(refexp, xstart, xend, 0.01);

                            Console.WriteLine(string.Format("Org# {0} : {1} ({2})", o.id, o.basis, o.basis.true_score));
                        }

                        continue;
                    }
                    else if (stext == "harvesters")
                    {
                        foreach (Org o in orgs)
                        {
                            if (o.opMode == OpMode.corpsefind)
                            {
                                o.basis.ScoreSqrt(refexp, xstart, xend, 0.01);

                                Console.WriteLine(string.Format("Org# {0} : {1} ({2})", o.id, o.basis, o.basis.true_score));
                            }
                        }

                        continue;
                    }
                    else if(stext == "dead")
                    {
                        foreach (Org o in orgs)
                        {
                            if (!o.alive && !o.harvested)
                            {
                                o.basis.ScoreSqrt(refexp, xstart, xend, 0.01);

                                Console.WriteLine(string.Format("Org# {0} : {1} ({2})", o.id, o.basis, o.basis.true_score));
                            }
                        }

                        continue;
                    }
                    else if(stext.StartsWith("spawn"))
                    {
                        string rem = stext.Substring(6);

                        int spc = 0; 

                        try
                        {
                            spc = Int32.Parse(rem);
                        }
                        catch(FormatException)
                        {
                            Console.WriteLine("Invalid Spawn");
                        }
                        
                        for(int i = 0; i < spc; i++)
                        {
                            spawnRandom();
                        }

                        continue;
                    }
                    else if(stext.StartsWith("regen"))
                    {
                        string rem = stext.Substring(6);

                        int rc = -1;

                        try
                        {
                            rc = Int32.Parse(rem);
                        }
                        catch(FormatException)
                        {
                            Console.WriteLine("Invalid Spawn");
                        }

                        if(rc >= 0)
                        {
                            regenRate = rc;
                        }

                        continue;
                    }
                    else if(stext.StartsWith("synthesize"))
                    {
                        string rem = stext.Substring(11);

                        double x = rnm.NextDouble() * (maxDim - minDim) + minDim;
                        double y = rnm.NextDouble() * (maxDim - minDim) + minDim;

                        var syno = Org.synthesize(rem, x, y);
                        orgs.Add(syno);

                        continue;
                    }
                    else if(stext == "wipe")
                    {
                        orgs.Clear();
                        continue;
                    }
                    else if(stext.StartsWith("min"))
                    {
                        string rem = stext.Substring(4);

                        double m = minDim;

                        try
                        {
                            m = double.Parse(rem);
                        }
                        catch(FormatException)
                        {
                            Console.WriteLine("Invalid min");
                        }

                        if(m != minDim)
                        {
                            minDim = m;
                        }

                        continue;
                    }
                    else if (stext.StartsWith("max"))
                    {
                        string rem = stext.Substring(4);

                        double m = maxDim;

                        try
                        {
                            m = double.Parse(rem);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid max");
                        }

                        if (m != maxDim)

                        {
                            maxDim = m;
                        }

                        continue;
                    }
                    else if(stext == "alive")
                    {
                        foreach (Org o in orgs)
                        {
                            if (o.alive)
                            {
                                o.basis.ScoreSqrt(refexp, xstart, xend, 0.01);

                                Console.WriteLine(string.Format("Org# {0} : {1} ({2})", o.id, o.basis, o.basis.true_score));
                            }
                        }

                        continue;
                    }
                    else if(stext.StartsWith("make"))
                    {
                        string rem = stext.Substring(5);
                        var ids = rem.Split(',');

                        if(ids == null || ids.Length != 3)
                        {
                            Console.WriteLine("Invalid Make");
                        }
                        else
                        {
                            int[] nids = new int[3];

                            try
                            {
                                nids[0] = Int32.Parse(ids[0]);
                                nids[1] = Int32.Parse(ids[1]);
                                nids[2] = Int32.Parse(ids[2]);

                                Org maker = orgs.Find(x => x.id == nids[0]);
                                Org left = orgs.Find(x => x.id == nids[1]);
                                Org right = orgs.Find(x => x.id == nids[2]);

                                if(maker == null || maker.orgType != OrgType.combiner || left == null || right == null)
                                {
                                    Console.WriteLine("Invalid Make");
                                }
                                else
                                {
                                    left.harvested = true;
                                    right.harvested = true;
                                    maker.alive = true;
                                    maker.basis.left = left.basis;
                                    maker.basis.right = right.basis;
                                    maker.opMode = OpMode.flee;
                                    maker.secsToModeChange = 3;
                                }
                            }
                            catch(FormatException)
                            {
                                Console.WriteLine("Invalid Make");
                            }
                        }

                        continue;
                    }

                    if(Int32.TryParse(stext, out sleep))
                    {
                        watch = false;

                    }
                }

                if(!watch)
                {
                    sleep--;
                    
                    if(sleep == 0)
                    {
                        watch = true;
                        continue;
                    }
                }

                doSec();

                for (int i = 0; i < regenRate; i++)
                {
                    spawnRandom();
                }
                    
                t++;
            }

            Console.WriteLine("Max ID Reached");
            Console.ReadLine();
        }

        public void doSec()
        {
            var c = findCentroid();
            var dc = findDeadCentroid();

            for(int i = 0; i < orgs.Count; i++)
            {
                if(!orgs[i].alive)
                {
                    continue;
                }

                move(orgs[i], c, dc);
                act(orgs[i]);
            }

            for(int i = orgs.Count - 1; i >= 0; i--)
            {
                if(orgs[i].harvested)
                {
                    orgs.RemoveAt(i);
                }
                else
                {
                    orgs[i].secsToModeChange--;

                    if(orgs[i].secsToModeChange <= 0)
                    {
                        switch(orgs[i].opMode)
                        {
                            case OpMode.corpsefind:
                                orgs[i].alive = false;
                                break;
                            case OpMode.flee:
                                orgs[i].opMode = OpMode.pursue;
                                orgs[i].secsToModeChange = 9;
                                break;
                            case OpMode.pursue:
                                orgs[i].opMode = OpMode.flee;
                                orgs[i].secsToModeChange = 3;
                                break;
                        }
                    }
                }
            }
        }

        public void move(Org o, Tuple<double, double> centroid, Tuple<double, double> deadCentroid)
        {
            double dx;
            double dy;

            if(o.opMode == OpMode.corpsefind)
            {
                dx = o.x - deadCentroid.Item1;
                dy = o.y - deadCentroid.Item2;
            }
            else
            {
                dx = o.x - centroid.Item1;
                dy = o.y - centroid.Item2;
            }

            double sq = Math.Sqrt(dx * dx + dy * dy);

            if(sq > speed)
            {
                dx *= speed / sq;
                dy *= speed / sq;
            }

            switch(o.opMode)
            {
                case OpMode.pursue:
                case OpMode.corpsefind:
                    o.x -= dx;
                    o.y -= dy;
                    break;
                case OpMode.flee:
                    o.x += dx;
                    o.y += dy;
                    break;
                default:
                    break;
            }
        }

        public void act(Org o)
        {
            Org target;

            switch(o.opMode)
            {
                case OpMode.pursue:
                    target = findAlive(o);

                    if(target != null)
                    {
                        if(o.orgType == OrgType.combiner && (o.basis.left == null || o.basis.right == null))
                        {
                            o.basis.true_score = double.MaxValue;
                        }
                        else
                        {
                            o.basis.ScoreSqrt(refexp, xstart, xend, 0.01);
                        }

                        if(target.orgType == OrgType.combiner && (target.basis.left == null || target.basis.right == null))
                        {
                            target.basis.true_score = double.MaxValue;
                        }
                        else
                        {
                            target.basis.ScoreSqrt(refexp, xstart, xend, 0.01);
                        }

                        //if (o.basis.true_score <= 7.0)
                        //{
                        //    watch = true;
                        //}

                        if(o.basis.true_score < target.basis.true_score)
                        {
                            // kill
                            o.wins++;
                            target.alive = false;

                            if (watch)
                            {

                                Console.WriteLine(string.Format("Org# {0} : {1} ({2}) killed Org# {3} : {4} ({5}).", o.id, o.basis, o.basis.true_score, target.id, target.basis, target.basis.true_score));
                                Console.ReadLine();
                            }
                        }
                        else if(o.basis.true_score > target.basis.true_score)
                        {
                            // killed
                            target.wins++;
                            o.alive = false;

                            if (watch)
                            {
                                Console.WriteLine(string.Format("Org# {0} : {1} ({2}) killed Org# {3} : {4} ({5}).", target.id, target.basis, target.basis.true_score, o.id, o.basis, o.basis.true_score));
                                Console.ReadLine();
                            }
                        }
                        else
                        {
                            // mutual kill
                            target.alive = false;
                            o.alive = false;

                            if (watch)
                            {
                                Console.WriteLine(string.Format("Org# {0} : {1} ({2}) and Org# {3} : {4} ({5}) killed each other.", o.id, o.basis, o.basis.true_score, target.id, target.basis, target.basis.true_score));
                                Console.ReadLine();
                            }
                        }

                        if(o != null && !o.alive)
                        {
                            if(o.basis.nodeCount() > nodeMax)
                            {
                                o.harvested = true;     // delete organism and don't allow them to be reused by a combiner
                            }
                        }

                        if(target != null && !target.alive)
                        {
                            if(target.basis.nodeCount() > nodeMax)
                            {
                                target.harvested = true;    // delete organism and don't allow them to be reused by a combiner
                            }
                        }
                    }

                    break;

                case OpMode.corpsefind:
                    target = findDead(o);

                    if(target != null)
                    {
                        if (watch)
                        {
                            Console.WriteLine(string.Format("Org# {0} : {1} harvested Org# {2} : {3} for parts.", o.id, o.basis, target.id, target.basis));
                            Console.ReadLine();
                        }
                            
                        target.harvested = true;

                        if(o.basis.left == null)
                        {
                            o.basis.left = target.basis;
                            o.secsToModeChange = 100000;
                        }
                        else
                        {
                            o.basis.right = target.basis;
                            o.opMode = OpMode.flee;
                            o.secsToModeChange = 3;
                        }
                    }

                    break;

                case OpMode.flee:

                    if(o.wins >= 2)
                    {
                        for(int i = 0; i < 3; i++)
                        {
                            Org n = new Org(o);
                            n.x = rnm.NextDouble() * (maxDim - minDim) + minDim;
                            n.y = rnm.NextDouble() * (maxDim - minDim) + minDim;
                            n.mutate(rnm, jitter);

                            orgs.Add(n);

                            if (watch)
                            {
                                Console.WriteLine(string.Format("Org# {0} has produced Org# {1}.", o.id, n.id));
                                Console.ReadLine();
                            }
                        }

                        o.wins = 0;
                    }

                    break;
                    
                default:
                    break;
            }
        }

        public Org findAlive(Org o)
        {
            return
                orgs.Where(org => Math.Sqrt((org.x - o.x) * (org.x - o.x) + (org.y - o.y) * (org.y - o.y)) <= findDist && org.id != o.id && org.alive)
                    .FirstOrDefault();
        }

        public Org findDead(Org o)
        {
            return
                orgs.Where(org => Math.Sqrt((org.x - o.x) * (org.x - o.x) + (org.y - o.y) * (org.y - o.y)) <= findDist && org.id != o.id && !org.alive && !org.harvested 
                                    && (org.orgType == OrgType.basic || (org.basis.left != null && org.basis.right != null) ))
                    .FirstOrDefault();
        }

        public void spawnRandom()
        {
            double x = rnm.NextDouble() * (maxDim - minDim) + minDim;
            double y = rnm.NextDouble() * (maxDim - minDim) + minDim;

            Org o;
            extree tree;
            expr_type et;
            double val;

            expr_type[] unaryTypes = new expr_type[] { expr_type.ln, expr_type.exp, expr_type.sin, expr_type.cos, expr_type.tan, expr_type.neg, expr_type.recip, expr_type.acos, expr_type.atan,
                                                       expr_type.sqrt, expr_type.abs, expr_type.cosh, expr_type.tanh, expr_type.sinh, expr_type.powinvsqrt2, expr_type.powsqrt2 };

            expr_type[] binaryTypes = new expr_type[] { expr_type.plus, expr_type.minus, expr_type.times, expr_type.divides, expr_type.pow, expr_type.root, expr_type.compose, expr_type.mean, expr_type.gmean };

            if(rnm.Next(comboRate + basicRate) < basicRate)
            {    
                if(rnm.Next(constRate + xRate) < constRate)
                {
                    et = expr_type.constant;
                }
                else
                {
                    et = unaryTypes[rnm.Next(16)];
                }

                if (rnm.Next(2) == 0)
                {
                    val = double.NaN;
                }
                else
                {
                    val = rnm.NextDouble() * 2.0 - 1.0;
                }

                tree = new extree(et, val);
                o = new Org(tree, OrgType.basic, x, y);
            }
            else
            {
                et = binaryTypes[rnm.Next(9)];
                tree = new extree();
                tree.type = et;
                tree.left = null;
                tree.right = null;

                o = new Org(tree, OrgType.combiner, x, y);
            }

            orgs.Add(o);
        }
    }
}
