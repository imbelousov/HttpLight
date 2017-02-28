using System.Text;

namespace HttpLight.Attributes
{
    /// <summary>
    /// Determines encoding in case when action returns <see cref="string"/> (UTF-8 by default)
    /// </summary>
    public class EncodingAttribute : ActionAttribute
    {
        public Encoding Encoding { get; }

        public EncodingAttribute(Encoding encoding)
        {
            Encoding = encoding;
        }

        internal override void Apply(ActionInfo actionInfo)
        {
            actionInfo.Encoding = Encoding;
        }
    }
}
