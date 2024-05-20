using System;
using System.Collections.Generic;
using System.Text.Json;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

internal static class AWSSecretsHelper
{
    /// <summary>
    /// Retrieves a secret and returns it as a dictionary.
    /// </summary>
    /// <param name="folderName">The folder or prefix for the secret.</param>
    /// <param name="secretName">The name of the secret.</param>
    /// <returns>A dictionary containing all key-value pairs from the secret.</returns>
    public static Dictionary<string, string>? GetSecretAsDictionary(string folderName, string secretName)
    {
        try
        {
            var secretJson = GetSecretJson(folderName, secretName);
            var data = JsonDocument.Parse(secretJson);
            var dictionary = new Dictionary<string, string>();
            foreach (var element in data.RootElement.EnumerateObject())
            {
                dictionary[element.Name] = element.Value.GetString();
            }
            return dictionary;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    /// <summary>
    /// Retrieves a specific key from a secret.
    /// </summary>
    /// <param name="folderName">The folder or prefix for the secret.</param>
    /// <param name="secretName">The name of the secret.</param>
    /// <param name="keyName">The key to retrieve from the secret.</param>
    /// <returns>The value of the specified key.</returns>
    public static string? GetSecretByKey(string folderName, string secretName, string keyName)
    {
        try
        {
            var secretJson = GetSecretJson(folderName, secretName);
            var data = JsonDocument.Parse(secretJson);
            if (data.RootElement.TryGetProperty(keyName, out JsonElement value))
            {
                return value.GetString();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return null; //Couldn't get / parse the key at this point.
    }

    #region Private Methods
    private static string GetSecretJson(string folderName, string secretName)
    {
        using var client = new AmazonSecretsManagerClient();
        var environmentPrefix = GetEnvironmentPrefix();
        var request = new GetSecretValueRequest { SecretId = $"{environmentPrefix}/{folderName}/{secretName}" };
        var response = client.GetSecretValueAsync(request).GetAwaiter().GetResult();

        if (response.SecretString == null)
            throw new InvalidOperationException("Secret string is null.");

        return response.SecretString;
    }

    private static string GetEnvironmentPrefix()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() ?? "development";
        return env switch
        {
            "development" => "Development",
            "staging" => "Staging",
            "production" => "Production",
            _ => "Development",
        };
    }
    #endregion
}