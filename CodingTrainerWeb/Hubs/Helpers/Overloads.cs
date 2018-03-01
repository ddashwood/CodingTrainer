using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingTrainer.CodingTrainerWeb.Hubs.Helpers
{
    public class Overloads : IReadOnlyCollection<Overload>
    {
        private List<Overload> overloads = new List<Overload>();
        public string Name { get; set; }
        public string ReturnType { get; set; }

        public int Count => overloads.Count();

        public IEnumerator<Overload> GetEnumerator()
        {
            return overloads.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return overloads.GetEnumerator();
        }

        public Overloads(IEnumerable<ISymbol> overloadSymbols)
        {
            // Name and return type should be the same for all overloads
            var firstMethodSymbol = overloadSymbols.First(s => s is IMethodSymbol) as IMethodSymbol;
            Name = firstMethodSymbol.Name;
            ReturnType = RemoveNamespace(firstMethodSymbol.ReturnType.ToString());

            foreach (var symbol in overloadSymbols)
            {
                if (symbol is IMethodSymbol method)
                {
                    var overload = new Overload();
                    foreach (var paramSymbol in method.Parameters)
                    {
                        string type = paramSymbol.Type.ToString();
                        type = RemoveNamespace(type);
                        overload.parameters.Add(new Parameter()
                        {
                            Name = paramSymbol.Name,
                            Type = type
                        });
                    }
                    overloads.Add(overload);
                }
            }
        }

        private static string RemoveNamespace(string type)
        {
            int dotPosition = type.LastIndexOf('.');
            if (dotPosition != -1)
            {
                type = type.Substring(dotPosition + 1);
            }

            return type;
        }
    }

    public class Overload
    {
        internal List<Parameter> parameters = new List<Parameter>();
        public IReadOnlyCollection<Parameter> Parameters
        {
            get
            {
                return parameters;
            }
        }
    }

    public class Parameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}