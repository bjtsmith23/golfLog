namespace GolfTracker.Models
{
    public class Round
    {
        public int RoundId { get; set; }
        public DateTime DatePlayed { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
    }
}
