namespace Content_Moderator.Models
{
    public class PredictionResponse
    {
        public string Text { get; set; }
        public bool IsToxic { get; set; }

        public float Probability { get; set; }
        
        public float Score { get; set; }

    }
}
