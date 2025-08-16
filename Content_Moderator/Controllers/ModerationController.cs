using Content_Moderator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.ML;
using System.Text.Json;
using static Content_Moderator.MLModel;
using static TorchSharp.torch.nn;

namespace Content_Moderator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModerationController : ControllerBase
    {

        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEngine;
        private readonly IDistributedCache _cache;

        public ModerationController(PredictionEnginePool<ModelInput, ModelOutput> predictionEngine, IDistributedCache Cache)
        {
            _predictionEngine = predictionEngine;
            _cache = Cache;
        }

        [HttpPost("predict")]
        public async Task<ActionResult<PredictionResponse>> PredictAsync([FromBody] PredictionRequest request)
        {
            //first checking in cache
            string cacheKey = $"prediction:{request.Text.ToLowerInvariant().GetHashCode()}";

            if (!string.IsNullOrEmpty(cacheKey))
            {
                
                string cachedPredictionJson = await _cache.GetStringAsync(cacheKey);

                if (cachedPredictionJson != null)
                {
                    // Cache Hit! Deserialize and return the cached response
                    var cachedResponse = JsonSerializer.Deserialize<PredictionResponse>(cachedPredictionJson);
                    return Ok(cachedResponse);
                }
            }

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

            // 3. Save the new prediction to the cache for future requests
            string newPredictionJson = JsonSerializer.Serialize(response);
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                // Cache the result for 1 day
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) // 1 day ttl
            };
            await _cache.SetStringAsync(cacheKey, newPredictionJson, cacheEntryOptions);

            return Ok(response);
        }
    }
}
