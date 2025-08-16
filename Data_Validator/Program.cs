using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello!");

        string inputFilePath = @"C:\path\to\your\Training_Complete.txt";
        string outputFilePath = @"C:\path\to\your\Training_Complete_fixed.txt";

        try
        {
            FixUnclosedQuotes(inputFilePath, outputFilePath);
            Console.WriteLine("Successfully processed the file!");
            Console.WriteLine($"Cleaned data saved to: {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
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






