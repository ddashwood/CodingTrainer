using CodingTrainer.CSharpRunner.CodeHost;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment
{
    [Table("AssessmentsByInspection")]
    public abstract class AssessmentByInspectionBase : AssessmentMethodBase
    {
        // Not mapped onto Entity Framework
        [NotMapped]
        [IgnoreDataMember]
        public CompilationWithSource? Compilation { private get; set; }

        protected abstract Task<bool> AssessCompilationAsync(Compilation compilation);

        protected async sealed override Task<bool> DoAssessmentAsync()
        {
            if (Compilation == null || Compilation.HasValue == false) throw new InvalidOperationException("Attempt to run assessment without a compilation");

            return await AssessCompilationAsync(Compilation.Value.CompilationObject);
        }
    }
}
