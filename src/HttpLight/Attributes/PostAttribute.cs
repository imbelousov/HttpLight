namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks method to be used for POST requests
    /// </summary>
    public class PostAttribute : MethodAttribute
    {
        public PostAttribute()
            : base(HttpMethod.Post)
        {
        }
    }
}
