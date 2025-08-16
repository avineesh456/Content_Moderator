using Content_Moderator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.ML;
using System.Text.Json;
using static Content_Moderator.MLModel;
using Microsoft.Extensions.Logging;

namespace Content_Moderator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModerationController : ControllerBase
    {

        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEngine;
        private readonly IDistributedCache _cache;
        private readonly ILogger<ModerationController> _logger;


        public ModerationController(PredictionEnginePool<ModelInput, ModelOutput> predictionEngine, IDistributedCache Cache, ILogger<ModerationController> Logger)
        {
            _predictionEngine = predictionEngine;
            _cache = Cache;
            _logger = Logger;
        }

        [HttpPost("predict")]
        public async Task<ActionResult<PredictionResponse>> PredictAsync([FromBody] PredictionRequest request)
        {
            try
            {
                //first checking in cache
                string cacheKey = $"prediction:{request.Text.ToLowerInvariant().GetHashCode()}";

                if (!string.IsNullOrEmpty(cacheKey))
                {

                    string cachedPredictionJson = await _cache.GetStringAsync(cacheKey);

                    if (cachedPredictionJson != null)
                    {
                        //logging a cache hit.
                        _logger.LogInformation("Cache HIT for key: {CacheKey}", cacheKey);
                        var cachedResponse = JsonSerializer.Deserialize<PredictionResponse>(cachedPredictionJson);
                        return Ok(cachedResponse);
                    }
                }

                //logging cache miss
                _logger.LogInformation("Cache MISS for key: {CacheKey}. Calling model.", cacheKey);

                // Map the incoming API request to the format the ML model expects
                var modelInput = new ModelInput
                {
                    SentimentText = request.Text
                };

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

                //log success
                _logger.LogInformation("Response successfully obtained from model");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during model prediction for text: {Text}", request.Text);
                return StatusCode(500, "An internal error occurred.");
            }
        }
    }
}
