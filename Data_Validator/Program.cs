using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello!");

        string inputFilePath = @"C:\Users\akl_r\OneDrive\Desktop\System Design C#\Content Moderation\Content_Moderator\TrainingData\train.csv";
        string outputFilePath = @"C:\Users\akl_r\OneDrive\Desktop\System Design C#\Content Moderation\Content_Moderator\TrainingData\Sample_Final.txt";


        Console.WriteLine("Starting CSV processing...");
        try
        {
            ProcessCsvFile(inputFilePath, outputFilePath);
            Console.WriteLine($"\nProcessing complete! Cleaned data saved to: {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nAn error occurred: {ex.Message}");
        }

        //try
        //{
        //    FixUnclosedQuotes(inputFilePath, outputFilePath);
        //    Console.WriteLine("Successfully processed the file!");
        //    Console.WriteLine($"Cleaned data saved to: {outputFilePath}");
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"An error occurred: {ex.Message}");
        //}
    }

    /// <summary>
    /// Reads a large CSV, extracts and cleans required columns, and writes to a TSV file.
    /// </summary>
    public static void ProcessCsvFile(string inputPath, string outputPath)
    {
        // CsvHelper configuration to handle potential bad data
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // If a row has a different number of columns, this will prevent an exception
            BadDataFound = null,
            // Ensures headers are matched case-insensitively
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };

        // Use StreamReader and StreamWriter for efficient handling of large files
        using (var reader = new StreamReader(inputPath))
        using (var csv = new CsvReader(reader, config))
        using (var writer = new StreamWriter(outputPath))
        {
            // Write the header for your new training file
            writer.WriteLine("Sentiment\tSentimentText");

            // Read the CSV records one by one
            csv.Read();
            csv.ReadHeader(); // Read the header row

            long rowCount = 0;
            while (csv.Read())
            {
                // Get only the fields you need by name
                var commentText = csv.GetField<string>("comment_text");
                var isToxic = csv.GetField<int>("toxic");

                if (!string.IsNullOrWhiteSpace(commentText))
                {
                    // Clean the text: replace newlines and tabs with a space
                    // to ensure each record is on a single line in the output file.
                    string cleanedText = Regex.Replace(commentText, @"\s+", " ").Trim();

                    // Write the cleaned data in the tab-separated format ML.NET expects
                    writer.WriteLine($"{isToxic}\t{cleanedText}");
                }

                rowCount++;
                // Provide progress feedback for the large file
                if (rowCount % 100000 == 0)
                {
                    Console.Write($"\rProcessed {rowCount:N0} rows...");
                }
            }
            Console.Write($"\rProcessed {rowCount:N0} rows...");
        }

    }


    public static void FixUnclosedQuotes(string inputPath, string outputPath)
    {
        // Use a list to hold the corrected lines
        var processedLines = new List<string>();

        // Read all lines from the original file
        string[] lines = File.ReadAllLines(inputPath);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                processedLines.Add(line);
                continue;
            }

            // Find the first tab to safely separate the label from the text
            int firstTabIndex = line.IndexOf('\t');

            // If the line is malformed (no tab), add it as-is and continue
            if (firstTabIndex == -1)
            {
                processedLines.Add(line);
                continue;
            }

            // Separate the label (e.g., "0" or "1") from the text content
            string label = line.Substring(0, firstTabIndex);
            string text = line.Substring(firstTabIndex + 1);

            // THE CORE LOGIC: Check if text starts with " but doesn't end with "
            if (text.StartsWith("\"") && !text.EndsWith("\""))
            {
                // If so, append the closing quote
                text += "\"";
            }

            // Reconstruct the line with the (potentially) fixed text
            processedLines.Add($"{label}\t{text}");
        }

        // Write all the processed lines to the new output file
        File.WriteAllLines(outputPath, processedLines, Encoding.UTF8);
    }
}






