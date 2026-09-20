using GolfTracker.Data;

namespace GolfTracker.Services
{
    public class LeaderboardService
    {
        public IEnumerable<(string Player, int TotalStrokes)> GetYearTotals(int year)
        {
            using var db = new GolfContext();

            var query =
                from hs in db.HoleScores
                join r in db.Rounds on hs.RoundId equals r.RoundId
                join p in db.Players on hs.PlayerId equals p.PlayerId
                where r.DatePlayed.Year == year
                group hs by p.Name into g
                select new { Player = g.Key, TotalStrokes = g.Sum(x => x.Score) };

            return query.ToList().Select(x => (x.Player, x.TotalStrokes)).ToList();
        }
    }
}
