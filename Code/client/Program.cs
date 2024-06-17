namespace RecommendationEngineClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                OperationsHandler operationsHandler = new();
                operationsHandler.DisplayOperations();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}