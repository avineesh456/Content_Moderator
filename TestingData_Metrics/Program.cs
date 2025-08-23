using System.Data;

public class Program
{
    public static void Main(string[] args)
    {
        // 1. DEFINE YOUR FILE PATHS
        string modelPath = @"C:\path\to\your\model.zip";
        // Use the new test file that includes the correct labels
        string testDataPath = @"C:\path\to\your\labeled_test_data.tsv";

        var mlContext = new MLContext();

        // 2. Load the trained model
        Console.WriteLine("Loading model...");
        ITransformer trainedModel = mlContext.Model.Load(modelPath, out var modelInputSchema);

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
        Console.gtiWriteLine($"F1 Score:          {testMetrics.F1Score:P2}");
        Console.WriteLine($"Precision:         {testMetrics.PositivePrecision:P2}");
        Console.WriteLine($"Recall:            {testMetrics.PositiveRecall:P2}");
        Console.WriteLine("==================================================");
    }
}