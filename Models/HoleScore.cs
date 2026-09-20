namespace GolfTracker.Models
{
    public class HoleScore
    {
        public int HoleScoreId { get; set; }
        public int RoundId { get; set; }
        public int PlayerId { get; set; }
        public int HoleNumber { get; set; }
        public int Score { get; set; }
    }
}
