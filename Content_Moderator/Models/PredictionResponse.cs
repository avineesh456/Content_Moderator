namespace Content_Moderator.Models
{
    public class PredictionResponse
    {
        public string Text { get; set; }
        public uint IsToxic { get; set; }
        public float Score { get; set; }
    }
}
