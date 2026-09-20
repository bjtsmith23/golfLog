using GolfTracker.Data;

namespace GolfTracker.Services
{
    public class LeaderboardService
    {
        public IEnumerable<(int RoundId, DateTime DatePlayed, string Player1, int Player1Score, string Player2, int Player2Score, int StrokeDifference)> GetYearRoundResults(int? year)
        {
            using var db = new GolfContext();

            var rounds = db.Rounds.ToList();
            var scores = db.HoleScores.ToList();
            var roundsById = rounds.ToDictionary(r => r.RoundId);
            var roundIds = rounds
                .Where(r => !year.HasValue || r.DatePlayed.Year == year.Value)
                .Select(r => r.RoundId)
                .Union(scores.Select(s => s.RoundId))
                .Where(roundId => !year.HasValue || !roundsById.ContainsKey(roundId) || roundsById[roundId].DatePlayed.Year == year.Value)
                .OrderBy(roundId => roundId)
                .ThenBy(roundId => roundsById.TryGetValue(roundId, out var round) ? round.DatePlayed : DateTime.Today)
                .ToList();

            var playerNames = db.Players.ToDictionary(p => p.PlayerId, p => p.Name);
            var results = new List<(int, DateTime, string, int, string, int, int)>();

            foreach (var roundId in roundIds)
            {
                var roundScores = scores.Where(s => s.RoundId == roundId).ToList();
                if (!roundsById.TryGetValue(roundId, out var round))
                {
                    var playerIds = roundScores.Select(s => s.PlayerId).Distinct().Take(2).ToArray();
                    round = new Models.Round
                    {
                        RoundId = roundId,
                        DatePlayed = DateTime.Today,
                        Player1Id = playerIds.ElementAtOrDefault(0),
                        Player2Id = playerIds.ElementAtOrDefault(1)
                    };
                }

                var player1Score = roundScores
                    .Where(s => s.PlayerId == round.Player1Id)
                    .Sum(s => s.Score);
                var player2Score = roundScores
                    .Where(s => s.PlayerId == round.Player2Id)
                    .Sum(s => s.Score);

                var player1 = GetPlayerName(round.Player1Id, playerNames);
                var player2 = GetPlayerName(round.Player2Id, playerNames);

                results.Add((
                    round.RoundId,
                    round.DatePlayed,
                    player1,
                    player1Score,
                    player2,
                    player2Score,
                    Math.Abs(player1Score - player2Score)));
            }

            return results;
        }

        private static string GetPlayerName(int playerId, IReadOnlyDictionary<int, string> playerNames)
        {
            if (playerNames.TryGetValue(playerId, out var name))
                return name;

            return playerId switch
            {
                1 => "Phil",
                2 => "Brian",
                _ => $"Player {playerId}"
            };
        }

        public int GetLatestRoundId()
        {
            using var db = new GolfContext();
            var latestRoundId = db.Rounds.Max(r => (int?)r.RoundId) ?? 0;
            var latestScoreRoundId = db.HoleScores.Max(s => (int?)s.RoundId) ?? 0;
            return Math.Max(latestRoundId, latestScoreRoundId);
        }

        public IEnumerable<(string Player, int HoleNumber, int Score)> GetRoundScores(int roundId)
        {
            using var db = new GolfContext();

            var query =
                from hs in db.HoleScores
                join p in db.Players on hs.PlayerId equals p.PlayerId into players
                from p in players.DefaultIfEmpty()
                where hs.RoundId == roundId
                orderby hs.PlayerId, hs.HoleNumber
                select new
                {
                    Player = p == null ? (hs.PlayerId == 1 ? "Phil" : hs.PlayerId == 2 ? "Brian" : $"Player {hs.PlayerId}") : p.Name,
                    HoleNumber = hs.HoleNumber,
                    Score = hs.Score
                };

            return query.ToList().Select(x => (x.Player, x.HoleNumber, x.Score)).ToList();
        }

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
