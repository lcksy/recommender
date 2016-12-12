using System;

namespace NReco.Math3.Exception
{
    /// Exception to be thrown when the argument is not greater than 0.
    ///
    /// @since 2.2
    /// @version $Id: NotStrictlyPositiveException.java 1533795 2013-10-19 17:27:34Z psteitz $
    public class NotStrictlyPositiveException : ArgumentException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public NotStrictlyPositiveException(object value)
            : base(String.Format("Argument is not positive: {0}", value))
        {
        }
    }
}