namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks method to be used for HEAD requests
    /// </summary>
    public class HeadAttribute : MethodAttribute
    {
        public HeadAttribute()
            : base(HttpMethod.Head)
        {
        }
    }
}
