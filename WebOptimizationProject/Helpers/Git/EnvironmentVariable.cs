namespace WebOptimizationProject.Helpers.Git
{
    public class EnvironmentVariable
    {
        public string Key { get; }
        public string Value { get; }

        public EnvironmentVariable(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}