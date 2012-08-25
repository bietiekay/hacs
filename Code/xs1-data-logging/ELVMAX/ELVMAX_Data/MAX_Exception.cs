using System;

namespace xs1_data_logging
{
 	public abstract class AMAXException : Exception
    {
        protected AMAXException(Exception innerException = null) : base(String.Empty, innerException) { }

        /// <summary>
        /// The message which is associated to this Exception
        /// </summary>
        protected String _msg;

        /// <summary>
        /// The message property
        /// </summary>
        public override String Message
        {
            get { return _msg; }
        }

        /// <summary>
        /// The error message
        /// </summary>
        /// <returns>The error message</returns>
        public override string ToString()
        {
            return _msg;
        }
    }

	public class MAXException: AMAXException
    {
        public MAXException(string Message)
        {
			_msg = Message;
        }
    } 

}

