using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;

namespace CodingTrainer.CodingTrainerWeb.Hubs.Helpers
{
    // This class is required because if we pass the CompilationExceptionError to the client:
    // a) it can't run GetMessage()
    // b) some properties aren't serialised to the client in full
    public class CompilerError
    {
        public DiagnosticDescriptor Descriptor { get; set; }
        public string Id { get; set; }
        public bool IsSuppressed { get; set; }
        public bool IsWarningAsError { get; set; }
        public Location Location { get; set; }
        public string LocationDescription { get; set; }
        public string Message { get; set; }
        public DiagnosticSeverity Severity { get; set; }
        public int WarningLevel { get; set; }

        public CompilerError(Diagnostic diagnostic)
        {
            Descriptor = diagnostic.Descriptor;
            Id = diagnostic.Id;
            IsSuppressed = diagnostic.IsSuppressed;
            IsWarningAsError = diagnostic.IsWarningAsError;
            Location = diagnostic.Location;
            LocationDescription = diagnostic.Location.ToString();
            Message = diagnostic.GetMessage();
            Severity = diagnostic.Severity;
            WarningLevel = diagnostic.WarningLevel;
        }

        public static CompilerError[] ArrayFromDiagnostics(IEnumerable<Diagnostic> diagnostics)
        {
            List<CompilerError> errors = new List<CompilerError>();
            foreach (var error in diagnostics.OrderBy(e => e.Location.SourceSpan.Start))
            {
                errors.Add(new CompilerError(error));
            }
            return errors.ToArray();
        }

        public static CompilerError[] ArrayFromException(CompilationErrorException exception)
        {
            return ArrayFromDiagnostics(exception.Diagnostics);
        }
    }
}