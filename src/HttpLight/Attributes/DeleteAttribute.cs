namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks method to be used for DELETE requests
    /// </summary>
    public class DeleteAttribute : MethodAttribute
    {
        public DeleteAttribute()
            : base(HttpMethod.Delete)
        {
        }
    }
}
