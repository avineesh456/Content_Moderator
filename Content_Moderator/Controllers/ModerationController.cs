using Content_Moderator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using static Content_Moderator.MLModel;
using static TorchSharp.torch.nn;

namespace Content_Moderator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModerationController : ControllerBase
    {

        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEngine;

        public ModerationController(PredictionEnginePool<ModelInput, ModelOutput> predictionEngine)
        {
            _predictionEngine = predictionEngine;
        }

        [HttpPost("predict")]
        public ActionResult<PredictionResponse> Predict([FromBody] PredictionRequest request)
        {
            // Map the incoming API request to the format the ML model expects
            var modelInput = new ModelInput
            {
                SentimentText = request.Text
            };

            // Use the prediction engine pool to get a prediction
            //ModelOutput prediction = _predictionEngine.Predict(modelInput); // Assuming _predictionEnginePool

            ModelOutput prediction = _predictionEngine.Predict(example: modelInput);
            // Map the model's output to your clean API response object
            var response = new PredictionResponse
            {
                Text = request.Text,
                IsToxic = prediction.PredictedLabel, // true/false prediction
                Probability = prediction.Probability, // The confidence score (0.0 to 1.0)
                Score = prediction.Score // The raw score (a single float)
            };

            return Ok(response);
        }
    }
}
