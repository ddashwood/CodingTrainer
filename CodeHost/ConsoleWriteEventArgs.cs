using System;


namespace CodingTrainer.CSharpRunner.CodeHost
{
    public delegate void ConsoleWriteEventHandler(object sender, ConsoleWriteEventArgs e);

    public class ConsoleWriteEventArgs: EventArgs
    {
        public string Message { get; private set; }
        public bool Highlight { get; set; }

        public ConsoleWriteEventArgs(string message, bool highlight = false)
        {
            Message = message;
            Highlight = highlight;
        }
    }
}
