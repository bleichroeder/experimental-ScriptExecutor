### Experimental project for executing C# code from a string.

#### Example
``` json
{
  "ScriptConfigurations": {
    "ScriptA": {
      "Name": "ScriptA",
      "Script": "string inScriptMessage = $\"(IN-{Params[\"scriptName\"]})Name: {Params[\"name\"]}, Age: {Params[\"age\"]}\"; string outScriptMessage = $\"(OUT-{Params[\"scriptName\"]})Name: {Params[\"name\"]}, Age: {Params[\"age\"]}\"; Console.WriteLine(inScriptMessage); return new Test() { Message = outScriptMessage };",
      "InputTypes": [
        "System.Console",
        "ScriptExecutor.Test"
      ],
      "ReturnType": "ScriptExecutor.Test"
    },
    "ScriptB": {
      "Name": "ScriptB",
      "Script": "string inScriptMessage = $\"(IN-{Params[\"scriptName\"]})Name: {Params[\"name\"]}, Age: {Params[\"age\"]}\"; string outScriptMessage = $\"(OUT-{Params[\"scriptName\"]})Name: {Params[\"name\"]}, Age: {Params[\"age\"]}\"; Console.WriteLine(inScriptMessage); return new Test() { Message = outScriptMessage };",
      "InputTypes": [
        "System.Console",
        "ScriptExecutor.Test"
      ],
      "ReturnType": "ScriptExecutor.Test"
    }
  }
}
```
``` csharp
ScriptOptions scriptOptions = ScriptHelper.BuildOptions(scriptConfiguration.InputTypes);

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
```
