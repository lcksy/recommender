
namespace NReco.CF.Taste.Common
{
    public sealed class NoSuchUserException : TasteException
    {
        public NoSuchUserException() { }

        public NoSuchUserException(long userID)
            : this(string.Format("No such user: {0}", userID))
        {
        }

        public NoSuchUserException(string message)
            : base(message)
        {
        }
    }
}