using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Configuration;

namespace ScriptExecutor
{
    /// <summary>
    /// Test program for executing customizable scripts from appsettings.json.
    /// </summary>
    internal class Program
    {
        // Available names for input parameters.
        private static readonly IList<string> Names = ["Alice", "Dave", "George", "Mary", "Lucy"];

        // Constants for script parameters.
        private const string NAME_PARAM = "name";
        private const string AGE_PARAM = "age";
        private const string SCRIPTNAME_PARAM = "scriptName";

        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <returns></returns>
        static async Task Main()
        {
            // Load configuration from appsettings.json.
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .Build();

            // Read our script configurations into a dictionary.
            IDictionary<string, ScriptConfiguration>? scriptConfigurations = configuration
                .GetSection("ScriptConfigurations")
                .Get<Dictionary<string, ScriptConfiguration>>();

            // Execute each of our scripts.
            foreach (ScriptConfiguration scriptConfiguration in scriptConfigurations?.Values!)
            {
                // Validate the configuration.
                scriptConfiguration.Validate();

                // Build options for the configuration.
                ScriptOptions scriptOptions = ScriptHelper.BuildOptions(scriptConfiguration.InputTypes);

                // Get our input parameters.
                Dictionary<string, object> inputParameters = new()
                {
                    { SCRIPTNAME_PARAM, scriptConfiguration.Name! },
                    { NAME_PARAM, Names[Random.Shared.Next(0, Names.Count - 1)] },
                    { AGE_PARAM, Random.Shared.Next(15, 50) }
                };

                // Execute the script which returns a dynamic object.
                dynamic result = await ScriptHelper.ExecuteScriptAsync(scriptConfiguration.Script!,
                                                                       inputParameters,
                                                                       scriptConfiguration.ReturnType!,
                                                                       scriptOptions);

                // If the return result is correct we'll output the message.
                if (result is Test testResult && testResult is not null)
                {
                    Console.WriteLine(testResult.Message);
                }
            }
        }
    }
}
