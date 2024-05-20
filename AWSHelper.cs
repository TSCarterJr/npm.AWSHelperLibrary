using System;
using System.Collections.Generic;
using System.Text.Json;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Orion.Common.Helpers.AWS;

public static class AWSHelper
{
    /// <summary>
    /// All AWS EC2 related calls that are useful to Vigil Core Portal
    /// </summary>
    public static class EC2
    {
        /// <summary>
        /// Returns the instance id of the machine this is run on.
        /// </summary>
        /// <returns>Ex: 'i-1234567890abcdef0'</returns>
        public static string GetInstanceID()
        {
            return AWSEC2Helper.GetEc2InstanceId();
        }
    }

    /// <summary>
    /// All AWS Secrets related calls that are useful to Vigil Core Portal.
    /// </summary>
    public static class Secrets
    {
        /// <summary>
        /// Gets the specified key's actual value.
        /// </summary>
        /// <param name="folderName">The 'folder' of the key (Generally the category)</param>
        /// <param name="secretName">The AWS secret name</param>
        /// <param name="keyName">The key within the secret, as secrets are KVP</param>
        /// <returns>string of the key within the secret, that lives within the folder.</returns>
        public static string? GetSecretByKey(string folderName, string secretName, string keyName)
        {
            return AWSSecretsHelper.GetSecretByKey(folderName, secretName, keyName);
        }

        /// <summary>
        /// Gets the entirety of the secret's KVP
        /// </summary>
        /// <param name="folderName">The 'folder' of the key (Generally the category)</param>
        /// <param name="secretName">The AWS secret name</param>
        /// <returns>Dictionary conversion of the KVP</returns>
        public static Dictionary<string, string>? GetSecretAsDictionary(string folderName, string secretName)
        {
            return AWSSecretsHelper.GetSecretAsDictionary(folderName, secretName);
        }
    }
}
