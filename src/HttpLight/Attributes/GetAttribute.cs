namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks method to be used for GET requests
    /// </summary>
    public class GetAttribute : MethodAttribute
    {
        public GetAttribute()
            : base(HttpMethod.Get)
        {
        }
    }
}
