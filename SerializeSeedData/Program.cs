using CodingTrainer.CodingTrainerEntityFramework.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTrainer.CSharpRunner.Assessment;
using CodingTrainer.CSharpRunner.Assessment.Methods;
using System.Data.Entity;
using System.Runtime.Serialization;
using CodingTrainer.CodingTrainerModels;
using System.Xml;
using System.IO;
using System.Data.Entity.Migrations;
using System.Reflection;

namespace CodingTrainer.SerializeSeedData
{
    class Program
    {
        static void Main(string[] args)
        {
            var ent = new ApplicationDbContext();

            Console.WriteLine("Do you want to add an Assessment to the database before serializing?");
            string reply = Console.ReadLine();
            while (reply == "y" || reply == "Y")
            {
                AddAssessment(ent);

                Console.WriteLine("Do you want to add another Assessment?");
                reply = Console.ReadLine();
            }

            Console.Write("Loading chapter and exercise data from the database... ");
            ent.Configuration.ProxyCreationEnabled = false;
            var chapters = ent.Chapters
                .Include(c => c.Exercises)
                .Include(c => c.Exercises.Select(e => e.Assessments))
                .ToList();
            Console.WriteLine("done");

            Console.Write("Saving data in XML format... ");

            List<Type> referencedTypes = new List<Type> { typeof(Exercise) };

            var baseType = typeof(AssessmentMethodBase);
            foreach (var type in baseType.Assembly.ExportedTypes)
            {
                if (baseType.IsAssignableFrom(type))
                {
                    referencedTypes.Add(type);
                }
            }

            DataContractSerializer ser = new DataContractSerializer(typeof(List<Chapter>), referencedTypes);

            var writer = XmlWriter.Create(@"..\..\..\CodingTrainerEntityFramework\Data\SeedData.xml", new XmlWriterSettings { Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates });
            ser.WriteObject(writer, chapters);
            writer.Close();


            Console.WriteLine("Done");
            Console.WriteLine();
            Console.WriteLine("Data has been saved in SeedData.xml in the CodingTrainerEntityFramework project");
            Console.WriteLine("Run 'update-database' in the Package Manager Console to reset data in database");
        }

        private static void AddAssessment(ApplicationDbContext ent)
        {
            try
            {
                Console.WriteLine("Enter the name of the Assessment class");
                string className = Console.ReadLine();

                var assembly = typeof(AssessmentMethodBase).Assembly;
                var type = assembly.GetType("CodingTrainer.CSharpRunner.Assessment.Methods." + className);
                var obj = Activator.CreateInstance(type);
                var attribute = typeof(IgnoreDataMemberAttribute);

                foreach (var member in type.GetProperties())
                {
                    if (member.GetCustomAttribute(attribute) == null)
                    {
                        if (member.PropertyType == typeof(string))
                        {
                            Console.WriteLine($"Enter the value for the {member.Name} string property");
                            Console.WriteLine("Finish a line with \\ to indicate a new line character and more on next line");

                            StringBuilder sb = new StringBuilder();
                            bool cont;

                            do
                            {
                                cont = false;
                                string input = Console.ReadLine();
                                if (input.EndsWith("\\"))
                                {
                                    input = input.Substring(0, input.Length - 1) + Environment.NewLine;
                                    cont = true;
                                }
                                sb.Append(input);
                            } while (cont);

                            member.SetValue(obj, sb.ToString());
                        }
                        else if (member.PropertyType == typeof(bool))
                        {
                            Console.WriteLine($"Enter the value for the {member.Name} bool property - T for true, anything else for false");
                            string input = Console.ReadLine();
                            bool bInput = (input == "t" || input == "T");
                            member.SetValue(obj, bInput);
                        }
                        else if (member.PropertyType == typeof(int))
                        {
                            Console.WriteLine($"Enter the value for the {member.Name} int property");
                            string input = Console.ReadLine();
                            member.SetValue(obj, Convert.ToInt32(input));
                        }
                        else
                        {
                            Console.WriteLine("Only string, bool and int are supported at the moment.");
                            Console.WriteLine($"Can't set value of {member.Name} because it is a {member.PropertyType.Name}");
                        }
                    }
                }

                ent.Assessments.Add(obj as AssessmentMethodBase);
                ent.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}
