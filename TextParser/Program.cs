using Newtonsoft.Json;

namespace TextParser
{
    internal class Program
    {
        const string JSON_DIR = "JSON";

        static void Main(string[] args)
        {
            ReadUserInput();
        }

        public static void ReadUserInput()
        {
            // Create folder for parse output
            EnsureFolderCreated();

            Console.WriteLine("! This app parses text file to json file.");
            Console.WriteLine("! Parser supports only 2 columns (key = value).");

            // Continue parsing after completion
            while (true)
            {
                Console.Write("Write path to input file: ");
                string inputFile = Console.ReadLine() ?? "";

                // Read lines from the text file
                string[] text = ReadFile(inputFile);

                // Read user input for text separator
                Console.Write("Write text delimiter: ");
                char delimiter = '=';

                try
                {
                    delimiter = char.Parse(Console.ReadLine() ?? "=");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Entered invalid delimiter, using '='. \n");
                }

                ParseToJson(text, delimiter);
            }
        }

        static void EnsureFolderCreated()
        {
            if (!Directory.Exists(JSON_DIR))
            {
                Directory.CreateDirectory(JSON_DIR);
            }
        }

        static string[] ReadFile(string path)
        {
            try
            {
                return File.ReadAllLines(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return [];
        }

        static char DetectDelimiter(string[] text)
        {
            char[] delimiters = { ',', ';', '=', ':' }; // Add more delimiters as needed

            Dictionary<char, int> delimiterCounts = new Dictionary<char, int>();

            if (text == null)
            {
                throw new Exception("File is empty. No lines to read.");
            }

            // This value can't be null
            string lines = text.ToString()!;

            foreach (char delimiter in delimiters)
            {
                int count = lines.Count(c => c == delimiter);

                delimiterCounts.Add(delimiter, count);
            }

            char mostFrequentDelimiter = delimiterCounts.OrderByDescending(x => x.Value).First().Key;

            return mostFrequentDelimiter;
        }

        static void ParseToJson(string[] text, char delimiter)
        {
            // Create a list to store the parsed data
            List<Dictionary<string, string>> dataList = new List<Dictionary<string, string>>();

            // Track cuurent line
            string currentLine = "";
            int lineCount = 0;

            // Output file name and location
            string outputFile = $"JSON/file_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.json";

            try
            {
                // Parse each line and convert it to a dictionary
                foreach (string line in text)
                {
                    lineCount++;

                    // Skip empty lines
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Read current line
                    currentLine = line;

                    string[] fields = line.Split(delimiter);

                    // Create a dictionary for the current record
                    Dictionary<string, string> record = new Dictionary<string, string>();

                    // Assuming the first field is a unique identifier, you can modify this as needed
                    string key = "";
                    string value = "";

                    try
                    {
                        key = fields[0].Trim();
                        value = fields[1].Trim();
                    }
                    catch (Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Can't read key or value, skipping this line.");
                        Console.ResetColor();
                        continue;
                    }

                    Console.WriteLine($"Line: {lineCount} | Text : {line} | Key : {key} | Value : {value}");

                    // Populate the dictionary with field names and values
                    for (int i = 1; i < fields.Length; i++)
                    {
                        // You can customize field names as needed
                        string fieldName = key;
                        record[fieldName] = value;
                    }

                    // Add the record to the list
                    dataList.Add(record);
                }

                // Convert the list to JSON
                string jsonOutput = JsonConvert.SerializeObject(dataList, Formatting.Indented);

                // Write the JSON data to the output file
                File.WriteAllText(outputFile, jsonOutput);

                Console.WriteLine("Conversion completed. JSON data written to: " + outputFile + "\n");
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed to read line: {(currentLine == "" ? "empty line" : "empty line")} \n");
            }
        }
    }
}
