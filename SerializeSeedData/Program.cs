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

namespace CodingTrainer.SerializeSeedData
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Loading chapter and exercise data from the database... ");

            var ent = new ApplicationDbContext();
            ent.Configuration.ProxyCreationEnabled = false;

            // Load chapters and exercises
            var chapters = ent.Chapters
                .Include(c => c.Exercises)
                .Include(c => c.Exercises.Select(e => e.Assessments))
                .ToList();

            Console.WriteLine("done");

            Console.Write("Saving data in XML format... ");

            Type[] referencedTypes =
            {
                typeof(Exercise),
                typeof(AlwaysPassAssessment),
                typeof(CheckAllOutputAssessment),
                typeof(CheckLastLineOfOutputAssessment),
                typeof(VariableTypeAssessment)
            };

            DataContractSerializer ser = new DataContractSerializer(typeof(List<Chapter>), referencedTypes);

            var writer = XmlWriter.Create(@"..\..\..\CodingTrainerEntityFramework\Data\SeedData.xml", new XmlWriterSettings { Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates });
            ser.WriteObject(writer, chapters);
            writer.Close();


            Console.WriteLine("Done");
            Console.WriteLine();
            Console.WriteLine("Data has been saved in SeedData.xml in the CodingTrainerEntityFramework project");
            Console.WriteLine("Run 'update-database' in the Package Manager Console to reset data in database");
        }
    }
}
