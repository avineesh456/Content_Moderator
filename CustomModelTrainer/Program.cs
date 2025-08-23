using Content_Moderator;
using Microsoft.ML;
using static Content_Moderator.MLModel;

public class Program
{
    // --- CONFIGURE YOUR FILE PATHS HERE ---
    private static readonly string TRAIN_DATA_FILEPATH = @"C:\Users\akl_r\OneDrive\Desktop\System Design C#\Content Moderation\Content_Moderator\TrainingData\Sample_Final.txt";
    private static readonly string MODEL_FILEPATH = @"C:\Users\akl_r\OneDrive\Desktop\System Design C#\Content Moderation\Content_Moderator\Custom_model.zip";
    // ------------------------------------

    public static void Main(string[] args)
    {

        // Use the new test file that includes the correct labels
        string testDataPath = @"C:\Users\akl_r\OneDrive\Desktop\System Design C#\Content Moderation\Content_Moderator\TrainingData\test_converted.txt";

        var mlContext = new MLContext();

        // 2. Load the trained model
        Console.WriteLine("Loading model...");
        ITransformer trainedModel = mlContext.Model.Load(MODEL_FILEPATH, out var modelInputSchema);

        // 3. Load the test data
        Console.WriteLine("Loading test data...");
        IDataView testDataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                                            path: testDataPath,
                                            hasHeader: true,
                                            separatorChar: '\t');

        // 4. Make predictions on the test data
        Console.WriteLine("Making predictions on test data...");
        var testMetrics = mlContext.BinaryClassification.Evaluate(trainedModel.Transform(testDataView), "Sentiment");

        // 5. Display the evaluation metrics
        Console.WriteLine("==================================================");
        Console.WriteLine($"* Model Evaluation Metrics      *");
        Console.WriteLine("==================================================");
        Console.WriteLine($"Accuracy:          {testMetrics.Accuracy:P2}");
        Console.WriteLine($"AUC:               {testMetrics.AreaUnderRocCurve:P2}");
        Console.WriteLine($"F1 Score:          {testMetrics.F1Score:P2}");
        Console.WriteLine($"Precision:         {testMetrics.PositivePrecision:P2}");
        Console.WriteLine($"Recall:            {testMetrics.PositiveRecall:P2}");
        Console.WriteLine("==================================================");



        //// Create a new MLContext
        //MLContext mlContext = new MLContext();

        //Console.WriteLine("=============== Training the model ===============");

        //// Load the training data
        //IDataView trainingDataView = mlContext.Data.LoadFromTextFile<MLModel.ModelInput>(
        //    path: TRAIN_DATA_FILEPATH,
        //    hasHeader: true,
        //    separatorChar: '\t',
        //    allowQuoting: true,
        //    allowSparse: false);

        //// Call the BuildPipeline and TrainModel methods from the auto-generated file
        //ITransformer trainedModel = MLModel.RetrainModel(mlContext, trainingDataView);
        ////                               ^^^^^^^^^^^^^ IMPORTANT: Change this to the class name from your .training.cs file

        //Console.WriteLine("=============== Finished training ===============");

        //// Save the trained model to a .zip file
        //mlContext.Model.Save(trainedModel, trainingDataView.Schema, MODEL_FILEPATH);

        //Console.WriteLine($"The model is saved to: {MODEL_FILEPATH}");
    }
}