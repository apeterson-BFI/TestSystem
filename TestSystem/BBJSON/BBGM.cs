using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace TestSystem.BBJSON
{
    public class BBGM
    {
        public static void test()
        {
            LeagueFile lf = JsonConvert.DeserializeObject<LeagueFile>(File.ReadAllText(@"c:\users\aap\downloads\BBGM_HP_Ravenclaw_2076_before_draft.json"));

            var data = lf.players.Select(p => new { pid = p.pid, ratings = p.ratings, stats = p.stats })
                      .Where(p => p.stats != null)
                      .SelectMany(p => p.ratings.Join(p.stats, r => r.season, s => s.season, (r, s) => new { pid = p.pid, tid=s.tid, ratings = r, stats = s }))
                      .Where(d => !d.stats.playoffs)
                      .Select(d => new List<object>() 
                        {
                           d.pid, d.tid, d.ratings.season,
                           d.ratings.hgt, d.ratings.stre, d.ratings.spd, d.ratings.jmp, d.ratings.endu, d.ratings.ins, d.ratings.dnk, d.ratings.ft, d.ratings.fg, d.ratings.tp, 
                           d.ratings.blk, d.ratings.stl, d.ratings.drb, d.ratings.pss, d.ratings.reb, d.ratings.ovr, d.ratings.pot, string.Join(" ", d.ratings.skills), d.ratings.pos, 
                           d.stats.gp, d.stats.gs, d.stats.min, d.stats.fg, d.stats.fga, d.stats.fgAtRim, d.stats.fgaAtRim, d.stats.fgLowPost, d.stats.fgaLowPost, d.stats.fgMidRange, 
                           d.stats.fgaMidRange, d.stats.tp, d.stats.tpa, d.stats.ft, d.stats.fta, d.stats.pm, d.stats.orb, d.stats.drb, d.stats.trb, d.stats.ast, d.stats.tov, d.stats.stl,
                           d.stats.blk, d.stats.ba, d.stats.pf, d.stats.pts, d.stats.per, d.stats.ewa, d.stats.yearsWithTeam
                        })
                     .Select(d => string.Join(",", d));

            using(var f = File.CreateText(@"c:\users\aap\desktop\hpbbgm.csv"))
            {
                f.WriteLine("pid,tid,season,hgt,stre,spd,jmp,endu,ins,dnk,ft,fg,tp,blk,stl,drb,pss,reb,ovr,pot,skills,pos,gp,gs,min,fg,fga,fgAtRim,fgaAtRim,fgLowPost,fgaLowPost,fgMidRange,fgaMidRange,tp,tpa,ft,fta,pm,orb,drb,trb,ast,tov,stl,blk,ba,pf,pts,per,ewa,yearswithTeam");

                foreach(var d in data)
                {
                    f.WriteLine(d);
                }
            }

            Console.WriteLine("woah 2");
        }


    }
}
