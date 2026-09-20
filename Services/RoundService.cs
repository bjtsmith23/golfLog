using GolfTracker.Data;
using GolfTracker.Models;

namespace GolfTracker.Services
{
    public class RoundService
    {
        public int CreateRound(DateTime date, int p1, int p2)
        {
            using var db = new GolfContext();
            var round = new Round { DatePlayed = date, Player1Id = p1, Player2Id = p2 };
            db.Rounds.Add(round);
            db.SaveChanges();
            return round.RoundId;
        }
    }
}
