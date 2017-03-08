namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks method to be used for PUT requests
    /// </summary>
    public class PutAttribute : MethodAttribute
    {
        public PutAttribute()
            : base(HttpMethod.Put)
        {
        }
    }
}
