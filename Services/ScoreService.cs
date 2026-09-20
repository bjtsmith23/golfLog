using GolfTracker.Data;
using GolfTracker.Models;

namespace GolfTracker.Services
{
    public class ScoreService
    {
        public void AddScore(int roundId, int playerId, int hole, int score)
        {
            using var db = new GolfContext();
            db.HoleScores.Add(new HoleScore
            {
                RoundId = roundId,
                PlayerId = playerId,
                HoleNumber = hole,
                Score = score
            });
            db.SaveChanges();
        }
    }
}
