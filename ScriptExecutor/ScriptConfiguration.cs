namespace ScriptExecutor
{
    /// <summary>
    /// The script configuration class.
    /// </summary>
    public class ScriptConfiguration
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the script.
        /// </summary>
        public string? Script { get; set; }

        /// <summary>
        /// Gets or sets the input types.
        /// </summary>
        public string[] InputTypes { get; set; } = [];

        /// <summary>
        /// Gets or sets the output type.
        /// </summary>
        public string? ReturnType { get; set; }

        /// <summary>
        /// Validates the script configuration.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public void Validate()
        {
            // Is there a script?
            if (string.IsNullOrEmpty(Script))
            {
                throw new ArgumentNullException(nameof(Script));
            }

            // Are input types specified?
            if (InputTypes is null || InputTypes.Length is 0)
            {
                throw new ArgumentNullException(nameof(InputTypes));
            }

            // Is a return type specified?
            if (ReturnType is null || ReturnType.Length is 0)
            {
                throw new ArgumentNullException(nameof(ReturnType));
            }
        }
    }
}
