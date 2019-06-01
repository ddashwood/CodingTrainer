using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    public class ExceptionRunningUserCodeException : ApplicationException
    {
        public ExceptionRunningUserCodeException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public override string Message
        {
            get
            {
                if (InnerException == null) return base.Message;

                return $"{base.Message}\r\nThe error is: {InnerException.Message}";
            }
        }

        public override string StackTrace
        {
            get
            {
                if (InnerException == null) return "";

                StringBuilder sb = new StringBuilder();

                string[] stackItems = InnerException.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var stackItem in stackItems)
                {
                    var thisItem = stackItem;
                    thisItem = thisItem.Replace("in :", "");  // Remove formatting issues that arise due to no filename
                    thisItem = thisItem.Replace("CodingTrainerExercise.", "");  // Remove formatting issues that arise due to the auto-added class name
                    thisItem = thisItem.Replace("<<Initialize>>d__0.MoveNext() ", "");  // Remove formatting issues that arise due to no method name
                    sb.Append(thisItem + "\r\n");
                }

                return sb.ToString();
            }
        }
    }
}
