namespace HttpLight.Attributes
{
    /// <summary>
    /// Determines custom HTTP path instead of using method name as path
    /// </summary>
    public class PathAttribute : ActionAttribute
    {
        public string Path { get; }

        public PathAttribute(string path)
        {
            Path = path;
        }

        internal override void Apply(ActionInfo actionInfo)
        {
            actionInfo.Paths.Add(Path);
        }
    }
}
