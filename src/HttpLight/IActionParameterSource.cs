using System.Collections.Generic;

namespace HttpLight
{
    public interface IActionParameterSource
    {
        string[] GetValues(string name);
        IEnumerable<KeyValuePair<string, string[]>> GetAllValues();
    }
}
