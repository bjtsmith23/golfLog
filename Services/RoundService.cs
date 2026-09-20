using GolfTracker.Data;
using GolfTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace GolfTracker.Services
{
    public class RoundService
    {
        public int GetNextRoundId()
        {
            using var db = new GolfContext();
            db.Database.EnsureCreated();

            var highestRoundId = db.Rounds.Max(r => (int?)r.RoundId) ?? 0;
            var highestScoreRoundId = db.HoleScores.Max(s => (int?)s.RoundId) ?? 0;
            var highestId = Math.Max(highestRoundId, highestScoreRoundId);

            if (highestId == 0)
                return 1;

            return highestId + 1;
        }

        public int GetLatestRoundId()
        {
            using var db = new GolfContext();
            db.Database.EnsureCreated();

            var latestRoundId = db.Rounds.Max(r => (int?)r.RoundId) ?? 0;
            var latestScoreRoundId = db.HoleScores.Max(s => (int?)s.RoundId) ?? 0;
            return Math.Max(latestRoundId, latestScoreRoundId);
        }

        public int CreateRound(DateTime date, int p1, int p2)
        {
            using var db = new GolfContext();
            db.Database.EnsureCreated();

            var round = new Round { DatePlayed = date, Player1Id = p1, Player2Id = p2 };
            db.Rounds.Add(round);
            db.SaveChanges();
            return round.RoundId;
        }
    }
}
