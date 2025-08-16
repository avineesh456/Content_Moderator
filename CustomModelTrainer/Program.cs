using Microsoft.ML;
using Content_Moderator;

public class Program
{
    // --- CONFIGURE YOUR FILE PATHS HERE ---
    private static readonly string TRAIN_DATA_FILEPATH = @"C:\Users\akl_r\OneDrive\Desktop\System Design C#\Content Moderation\Content_Moderator\TrainingData\Sample_Final.txt";
    private static readonly string MODEL_FILEPATH = @"C:\Users\akl_r\OneDrive\Desktop\System Design C#\Content Moderation\Content_Moderator\TrainingData\Custom_model.zip";
    // ------------------------------------

    public static void Main(string[] args)
    {
        // Create a new MLContext
        MLContext mlContext = new MLContext();

        Console.WriteLine("=============== Training the model ===============");

        // Load the training data
        IDataView trainingDataView = mlContext.Data.LoadFromTextFile<MLModel.ModelInput>(
            path: TRAIN_DATA_FILEPATH,
            hasHeader: true,
            separatorChar: '\t',
            allowQuoting: true,
            allowSparse: false);

        // Call the BuildPipeline and TrainModel methods from the auto-generated file
        ITransformer trainedModel = MLModel.RetrainModel(mlContext, trainingDataView);
        //                               ^^^^^^^^^^^^^ IMPORTANT: Change this to the class name from your .training.cs file

        Console.WriteLine("=============== Finished training ===============");

        // Save the trained model to a .zip file
        mlContext.Model.Save(trainedModel, trainingDataView.Schema, MODEL_FILEPATH);

        Console.WriteLine($"The model is saved to: {MODEL_FILEPATH}");
    }
}