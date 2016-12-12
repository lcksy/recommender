
namespace NReco.CF.Taste.Common
{
    public sealed class NoSuchItemException : TasteException
    {
        public NoSuchItemException() { }

        public NoSuchItemException(long itemID)
            : this(itemID.ToString())
        {

        }

        public NoSuchItemException(string message)
            : base(message)
        {
        }
    }
}