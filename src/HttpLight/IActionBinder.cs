using System;

namespace HttpLight
{
    public interface IActionBinder
    {
        object Bind(Type parameterType, string parameterName, HttpRequest request);
    }
}
