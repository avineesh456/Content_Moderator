using Content_Moderator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using static Content_Moderator.MLModel;

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
            // Map PredictionRequest to ModelInput
            var modelInput = new ModelInput
            {
                SentimentText = request.Text
                // IsToxic is ignored in prediction, so we don’t set it
            };

            var modelOutput = _predictionEngine.Predict(modelInput);

            // Map model output to clean response
            var response = new PredictionResponse
            {
                Text = request.Text,
                IsToxic = modelOutput.Sentiment, // or whatever ML.NET named it
                Score = modelOutput.Score[1]
            };

            return Ok(response);

        }
    }
}
