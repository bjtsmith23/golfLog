using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var databasePath = Environment.GetEnvironmentVariable("GOLF_DB_PATH")
	?? Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "golftracker.db"));

Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
EnsureDatabase();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/rounds/next-id", () => Results.Ok(new { roundId = GetNextRoundId() }));

app.MapPost("/api/rounds", (RoundRequest request) =>
{
	var roundId = GetNextRoundId();
	using var connection = OpenConnection();
	using var command = connection.CreateCommand();
	command.CommandText = "INSERT INTO Rounds (RoundId, DatePlayed, Player1Id, Player2Id) VALUES ($id, $date, 1, 2)";
	command.Parameters.AddWithValue("$id", roundId);
	command.Parameters.AddWithValue("$date", request.DatePlayed.Date.ToString("O"));
	command.ExecuteNonQuery();
	return Results.Ok(new { roundId });
});

app.MapPost("/api/rounds/{roundId:int}/scores", (int roundId, ScoreRequest request) =>
{
	if (request.Scores is null || request.Scores.Count != 18 || request.Scores.Any(score => score < 1 || score > 20))
		return Results.BadRequest(new { message = "Enter a score from 1 to 20 for every hole." });

	using var connection = OpenConnection();
	using var transaction = connection.BeginTransaction();
	using var delete = connection.CreateCommand();
	delete.Transaction = transaction;
	delete.CommandText = "DELETE FROM HoleScores WHERE RoundId = $roundId AND PlayerId = $playerId";
	delete.Parameters.AddWithValue("$roundId", roundId);
	delete.Parameters.AddWithValue("$playerId", request.PlayerId);
	delete.ExecuteNonQuery();

	for (var index = 0; index < request.Scores.Count; index++)
	{
		using var insert = connection.CreateCommand();
		insert.Transaction = transaction;
		insert.CommandText = "INSERT INTO HoleScores (RoundId, PlayerId, HoleNumber, Score) VALUES ($roundId, $playerId, $hole, $score)";
		insert.Parameters.AddWithValue("$roundId", roundId);
		insert.Parameters.AddWithValue("$playerId", request.PlayerId);
		insert.Parameters.AddWithValue("$hole", index + 1);
		insert.Parameters.AddWithValue("$score", request.Scores[index]);
		insert.ExecuteNonQuery();
	}

	transaction.Commit();
	return Results.Ok();
});

app.MapGet("/api/leaderboard", (int? year) =>
{
	var selectedYear = year ?? DateTime.Today.Year;
	using var connection = OpenConnection();

	var rounds = new Dictionary<int, (DateTime DatePlayed, int Player1Id, int Player2Id)>();
	using var command = connection.CreateCommand();
	command.CommandText = "SELECT RoundId, DatePlayed, Player1Id, Player2Id FROM Rounds";
	using (var reader = command.ExecuteReader())
	{
		while (reader.Read())
		{
			var roundId = reader.GetInt32(0);
			rounds[roundId] = (DateTime.Parse(reader.GetString(1)), reader.GetInt32(2), reader.GetInt32(3));
		}
	}

	var scores = new List<(int RoundId, int PlayerId, int Score)>();
	using (var scoreCommand = connection.CreateCommand())
	{
		scoreCommand.CommandText = "SELECT RoundId, PlayerId, Score FROM HoleScores";
		using var scoreReader = scoreCommand.ExecuteReader();
		while (scoreReader.Read())
			scores.Add((scoreReader.GetInt32(0), scoreReader.GetInt32(1), scoreReader.GetInt32(2)));
	}

	var roundIds = rounds.Keys
		.Where(roundId => rounds[roundId].DatePlayed.Year == selectedYear)
		.Union(scores.Select(score => score.RoundId))
		.Where(roundId => !rounds.ContainsKey(roundId) || rounds[roundId].DatePlayed.Year == selectedYear)
		.OrderByDescending(roundId => rounds.TryGetValue(roundId, out var round) ? round.DatePlayed : DateTime.Today)
		.ThenByDescending(roundId => roundId)
		.ToList();

	var results = new List<object>();
	foreach (var roundId in roundIds)
	{
		var roundScores = scores.Where(score => score.RoundId == roundId).ToList();
		if (!rounds.TryGetValue(roundId, out var round))
		{
			var playerIds = roundScores.Select(score => score.PlayerId).Distinct().Take(2).ToArray();
			round = (DateTime.Today, playerIds.ElementAtOrDefault(0), playerIds.ElementAtOrDefault(1));
		}

		var player1Id = round.Player1Id;
		var player2Id = round.Player2Id;
		var player1Score = roundScores.Where(score => score.PlayerId == player1Id).Sum(score => score.Score);
		var player2Score = roundScores.Where(score => score.PlayerId == player2Id).Sum(score => score.Score);
		var difference = Math.Abs(player1Score - player2Score);

		results.Add(new
		{
			roundId,
			datePlayed = round.DatePlayed.ToString("yyyy-MM-dd"),
			player1 = PlayerName(player1Id),
			player1Score,
			player2 = PlayerName(player2Id),
			player2Score,
			difference,
			result = player1Score == player2Score ? "TIE" : player1Score < player2Score
				? $"{PlayerName(player1Id)} beat {PlayerName(player2Id)} by {difference} strokes"
				: $"{PlayerName(player2Id)} beat {PlayerName(player1Id)} by {difference} strokes"
		});
	}

	return Results.Ok(results);
});

app.MapFallbackToFile("index.html");
app.Run();

void EnsureDatabase()
{
	using var connection = OpenConnection();
	using var command = connection.CreateCommand();
	command.CommandText = """
		CREATE TABLE IF NOT EXISTS Players (PlayerId INTEGER PRIMARY KEY, Name TEXT NOT NULL);
		CREATE TABLE IF NOT EXISTS Rounds (RoundId INTEGER PRIMARY KEY, DatePlayed TEXT NOT NULL, Player1Id INTEGER NOT NULL, Player2Id INTEGER NOT NULL);
		CREATE TABLE IF NOT EXISTS HoleScores (HoleScoreId INTEGER PRIMARY KEY AUTOINCREMENT, RoundId INTEGER NOT NULL, PlayerId INTEGER NOT NULL, HoleNumber INTEGER NOT NULL, Score INTEGER NOT NULL);
		""";
	command.ExecuteNonQuery();
}

SqliteConnection OpenConnection()
{
	var connection = new SqliteConnection($"Data Source={databasePath}");
	connection.Open();
	return connection;
}

int GetNextRoundId()
{
	using var connection = OpenConnection();
	using var command = connection.CreateCommand();
	command.CommandText = "SELECT MAX(RoundId) FROM Rounds UNION SELECT MAX(RoundId) FROM HoleScores";
	using var reader = command.ExecuteReader();
	var highest = 0;
	while (reader.Read())
		if (!reader.IsDBNull(0)) highest = Math.Max(highest, reader.GetInt32(0));
	return highest + 1;
}

int GetPlayerScore(SqliteConnection connection, int roundId, int playerId)
{
	using var command = connection.CreateCommand();
	command.CommandText = "SELECT COALESCE(SUM(Score), 0) FROM HoleScores WHERE RoundId = $roundId AND PlayerId = $playerId";
	command.Parameters.AddWithValue("$roundId", roundId);
	command.Parameters.AddWithValue("$playerId", playerId);
	return Convert.ToInt32(command.ExecuteScalar());
}

string PlayerName(int playerId) => playerId switch
{
	1 => "Phil",
	2 => "Brian",
	_ => $"Player {playerId}"
};

record RoundRequest(DateTime DatePlayed);
record ScoreRequest(int PlayerId, List<int> Scores);
