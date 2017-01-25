using System;
using System.Configuration;

namespace Common
{
    public static class Config
    {
        public static string AppSetting(string key) => GetOrThrow<string>(key, ConfigurationManager.AppSettings[key], "appsetting");

        public static string ConnectionString(string name) => GetOrThrow<string>(name, ConfigurationManager.ConnectionStrings[name]?.ConnectionString, "connection string");

        static T GetOrThrow<T>(string name, string value, string what)
        {
            if (value == null)
            {
                throw new ConfigurationErrorsException($"Could not find {what} with name '{name}'");
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception exception)
            {
                throw new ConfigurationErrorsException($"Could not get '{value}' from {what} '{name}' as {typeof(T)}", exception);
            }
        }
    }
}