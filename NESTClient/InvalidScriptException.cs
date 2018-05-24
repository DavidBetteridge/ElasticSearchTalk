using System;
using System.Runtime.Serialization;

namespace NESTClient
{
    [Serializable]
    internal class InvalidScriptException : Exception
    {
        private int v1;
        private int v2;
        private string message;
        private string v3;
        private string v4;
        private int column;
        private string errorText;
        private string errorNumber;

        public InvalidScriptException(int line)
        {
        }

        public InvalidScriptException(string message) : base(message)
        {
        }

        public InvalidScriptException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidScriptException(int line, int column, string errorText, string errorNumber) : this(line)
        {
            this.column = column;
            this.errorText = errorText;
            this.errorNumber = errorNumber;
        }

        public InvalidScriptException(int v1, int v2, string message, string v3, string v4)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.message = message;
            this.v3 = v3;
            this.v4 = v4;
        }

        protected InvalidScriptException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}