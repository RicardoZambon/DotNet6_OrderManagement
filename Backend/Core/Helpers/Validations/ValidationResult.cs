namespace Zambon.OrderManagement.Core.Helpers.Validations
{
    public class ValidationResult
    {
        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();

        public void SetError(string key, string message)
        {
            if (!Errors.ContainsKey(key))
            {
                Errors.Add(key, new[] { message });
            }
            else
            {
                var errors = Errors[key].ToList();
                errors.Add(message);
                Errors[key] = errors.ToArray();
            }
        }
    }
}