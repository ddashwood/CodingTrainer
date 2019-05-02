using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    internal static class References
    {
        public static ImmutableList<MetadataReference> ReferencedAssembliesData { get; } = new List<MetadataReference>()
            {
                FromType(typeof(Console)),      // mscorlib
                FromType(typeof(System.Linq.Enumerable))
            }.ToImmutableList();

        public static MetadataReference FromType(Type type)
        {
            return MetadataReference.CreateFromFile(type.Assembly.Location);
        }
    }
}
