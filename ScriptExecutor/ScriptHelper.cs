using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ScriptExecutor
{
    /// <summary>
    /// Contains methods for building and executing scripts.
    /// </summary>
    public class ScriptHelper
    {
        /// <summary>
        /// Builds <see cref="ScriptOptions"/>.
        /// Uses the <see cref="ScriptOptions.Default"/> as base options.
        /// Adds references and imports for all <see cref="Type"/> in the provided collection.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static ScriptOptions BuildOptions(IEnumerable<Type> types) => BuildOptions(types, null);

        /// <summary>
        /// Builds <see cref="ScriptOptions"/>.
        /// Uses the provided <see cref="ScriptOptions"/> as base options.
        /// Adds references and imports for all <see cref="Type"/> in the provided collection.
        /// </summary>
        /// <param name="types"></param>
        /// <param name="baseOptions"></param>
        /// <returns></returns>
        public static ScriptOptions BuildOptions(IEnumerable<Type> types, ScriptOptions? baseOptions)
        {
            // Build the script options.
            ScriptOptions options = baseOptions ?? ScriptOptions.Default;

            // Get all specified type assemblies and add references for them.
            options.AddReferences(types.Select(t => t.Assembly).ToArray());

            // Get all specified type namespaces and import them.
            options.AddImports(types.Select(t => t.Namespace).ToArray());

            // Return the built options.
            return options;
        }

        /// <summary>
        /// Builds <see cref="ScriptOptions"/>.
        /// Uses the <see cref="ScriptOptions.Default"/> as base options.
        /// Adds references and imports for all <see cref="Type"/> in the provided collection.
        /// </summary>
        /// <param name="typeNames"></param>
        /// <returns></returns>
        public static ScriptOptions BuildOptions(IEnumerable<string> typeNames) => BuildOptions(typeNames, null);

        /// <summary>
        /// Builds <see cref="ScriptOptions"/>.
        /// Uses the provided <see cref="ScriptOptions"/> as base options.
        /// Adds references and imports for all <see cref="Type"/> in the provided collection.
        /// </summary>
        /// <param name="typeNames"></param>
        /// <param name="baseOptions"></param>
        /// <returns></returns>
        public static ScriptOptions BuildOptions(IEnumerable<string> typeNames, ScriptOptions? baseOptions)
        {
            // Build the script options.
            ScriptOptions options = baseOptions ?? ScriptOptions.Default;

            // Resolve type names to Type objects.
            List<Type> types = [];
            foreach (string typeName in typeNames)
            {
                // Try and get the type by name.
                Type? type = Type.GetType(typeName) ?? AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes())
                        .FirstOrDefault(t => t.FullName == typeName);

                // If we have the type, add a reference and import the namespace.
                if (type is not null)
                {
                    types.Add(type);

                    options = options.AddReferences(type.Assembly)
                                     .AddImports(type.Namespace);
                }
            }

            // Return the options.
            return options;
        }

        /// <summary>
        /// Executes the specified script using the provided parameter collection.
        /// Uses <see cref="ScriptOptions.Default"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codeBlock"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<T> ExecuteScriptAsync<T>(string codeBlock, Dictionary<string, object> parameters)
            => await ExecuteScriptAsync<T>(codeBlock, parameters, null);

        /// <summary>
        /// Executes the specified script using the provided parameter collection.
        /// Uses the provided <see cref="ScriptOptions"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codeBlock"></param>
        /// <param name="parameters"></param>
        /// <param name="scriptOptions"></param>
        /// <returns></returns>
        public static async Task<T> ExecuteScriptAsync<T>(string codeBlock, Dictionary<string, object> parameters, ScriptOptions? scriptOptions)
        {
            // Build the global parameters.
            Globals globals = new() { Params = parameters };

            // Execute the script.
            object returnValue = await CSharpScript.EvaluateAsync(code: codeBlock,
                                                                  globals: globals,
                                                                  options: scriptOptions);
            // Return as the generic type.
            return (T)returnValue;
        }

        /// <summary>
        /// Executes the specified script using the provided parameter collection and specified return type.
        /// </summary>
        /// <param name="codeBlock">The C# code to execute.</param>
        /// <param name="parameters">A dictionary of parameters to pass to the script.</param>
        /// <param name="returnTypeName">The fully qualified name of the return type.</param>
        /// <returns>The result of the script execution cast to the specified return type.</returns>
        public static async Task<dynamic> ExecuteScriptAsync(string codeBlock, Dictionary<string, object> parameters, string returnTypeName)
            => await ExecuteScriptAsync(codeBlock, parameters, returnTypeName, null);

        /// <summary>
        /// Executes the specified script using the provided parameter collection and specified return type.
        /// </summary>
        /// <param name="codeBlock">The C# code to execute.</param>
        /// <param name="parameters">A dictionary of parameters to pass to the script.</param>
        /// <param name="returnTypeName">The fully qualified name of the return type.</param>
        /// <param name="scriptOptions">Optional script options.</param>
        /// <returns>The result of the script execution cast to the specified return type.</returns>
        public static async Task<dynamic> ExecuteScriptAsync(string codeBlock, Dictionary<string, object> parameters, string returnTypeName, ScriptOptions? scriptOptions)
        {
            // Determine the return type.
            Type returnType = Type.GetType(returnTypeName) ?? throw new ArgumentException("Return type not found.", nameof(returnTypeName));

            // Were script options specified.
            scriptOptions ??= ScriptOptions.Default;

            // Add the return type to the options.
            scriptOptions.AddReferences(returnType.Assembly)
                         .AddImports(returnType.Namespace);

            // Create the script.
            Script<object> script = CSharpScript.Create(codeBlock, scriptOptions, typeof(Globals));

            // Execute the script.
            ScriptState<object> result = await script.RunAsync(new Globals { Params = parameters });

            // Return as the return type.
            return Convert.ChangeType(result.ReturnValue, returnType);
        }

        /// <summary>
        /// Global parameter collection.
        /// </summary>
        public class Globals
        {
            /// <summary>
            /// Gets or sets the parameters.
            /// </summary>
            public Dictionary<string, object> Params = [];
        }
    }
}
