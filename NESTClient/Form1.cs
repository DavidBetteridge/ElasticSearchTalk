﻿using Nest;
using ScintillaNET;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace NESTClient
{
    public partial class Form1 : Form
    {
        static class Endpoints
        {
            public const string Direct = "http://localhost:9200";
            public const string Fiddler = "http://ipv4.fiddler:9200";
        }

        Uri EndPoint()
        {
            if (cbTraceTraffic.Checked)
                return new Uri(Endpoints.Fiddler);
            else
                return new Uri(Endpoints.Direct);
        }


        private void cmdSimple_Click(object sender, EventArgs e)
        {
            var settings = new ConnectionSettings(EndPoint())
                     .DefaultIndex("stackoverflow");

            var client = new ElasticClient(settings);

            var searchResponse = client.Search<post>(s => s
                .Size(5)
                .Query(q => q
                     .Match(m => m
                        .Field(f => f.body)
                       .Query("c#")
                     )
                )
            );

            DisplayResults(searchResponse.Documents);
        }


        private void DisplayResults(System.Collections.Generic.IReadOnlyCollection<post> documents)
        {
            this.panelResults.Controls.Clear();

            var top = 30;
            var shaded = false;

            foreach (var document in documents)
            {
                var lbl = new Label
                {
                    Location = new Point(16, top),
                    Size = new Size(panelResults.Width - 32, 30),
                    TabIndex = 0,
                    Text = Summary(document.title, document.body),
                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                };
                if (shaded) lbl.BackColor = Color.LightGray;
                this.panelResults.Controls.Add(lbl);
                shaded = !shaded;
                top += 40;
            }
        }

        private string Summary(string title, string body)
        {
            title = title.Replace(System.Environment.NewLine, "");
            body = body.Replace(System.Environment.NewLine, "");

            if (!string.IsNullOrWhiteSpace(title))
                return title;
            else
            {
                if (body.Length >= 100)
                    return body.Substring(0, 100) + "...";
                else
                    return body;
            }
        }

        private void cmdExecute_Click(object sender, EventArgs e)
        {

            try
            {
                var assembly = GenerateAssembly(ScriptInAClass(scintilla.Text));

                var settings = new ConnectionSettings(EndPoint())
                        .DefaultIndex("stackoverflow");

                var client = new ElasticClient(settings);

                var searchResponse = Func<ISearchResponse<post>>(assembly, "RunDemo", client);

                var documents = searchResponse.Documents;

                DisplayResults(documents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Oh dear");
            }
        }

        private string ScriptInAClass(string code)
        {
            return @"using Nest;
using System;  
using NESTClient;
public class DemoCode
{
public ISearchResponse<post> RunDemo(ElasticClient client)
{
" + code + @"
return searchResponse;
}
}";
        }

        public Form1()
        {
            InitializeComponent();
            scintilla.Lexer = Lexer.Cpp;
            scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 10;
            scintilla.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            scintilla.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
            scintilla.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            scintilla.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
            scintilla.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
            scintilla.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
            scintilla.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
            scintilla.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
            scintilla.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
            scintilla.Styles[Style.Cpp.String].Size = 12;
            scintilla.Lexer = Lexer.Cpp;

            // Set the keywords
            scintilla.SetKeywords(0, "abstract as base break case catch checked continue default delegate do else event explicit extern false finally fixed for foreach goto if implicit in interface internal is lock namespace new null object operator out override params private protected public readonly ref return sealed sizeof stackalloc switch this throw true try typeof unchecked unsafe using virtual while");
            scintilla.SetKeywords(1, "bool byte char class const decimal double enum float int long sbyte short static string struct uint ulong ushort void");
        }

        private Assembly GenerateAssembly(string sourceCode)
        {
            Environment.SetEnvironmentVariable("ROSLYN_COMPILER_LOCATION", @"C:\Code\ElasticSearchTalk\packages\Microsoft.Net.Compilers.2.9.0\tools", EnvironmentVariableTarget.Process);
            var _codeDomProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
            var _compilerParameters = new CompilerParameters();
            _compilerParameters.ReferencedAssemblies.Add("system.dll");
            _compilerParameters.ReferencedAssemblies.Add("system.xml.dll");
            _compilerParameters.ReferencedAssemblies.Add("system.core.dll");
            _compilerParameters.ReferencedAssemblies.Add("system.linq.dll");
            _compilerParameters.ReferencedAssemblies.Add(@"C:\Code\ElasticSearchTalk\packages\NEST.6.1.0\lib\net46\Nest.dll");
            _compilerParameters.ReferencedAssemblies.Add(@"C:\Code\ElasticSearchTalk\NESTClient\bin\Debug\NESTClient.exe");
            _compilerParameters.GenerateExecutable = false;
            _compilerParameters.GenerateInMemory = true;
            _compilerParameters.IncludeDebugInformation = true;

            var _compilerResults = _codeDomProvider.CompileAssemblyFromSource(_compilerParameters, sourceCode);

            if (_compilerResults.Errors.HasErrors)
            {
                foreach (CompilerError err in _compilerResults.Errors)
                {
                    throw new InvalidScriptException(err.Line, err.Column, err.ErrorText, err.ErrorNumber);
                }
                throw new Exception("Compilation failed.");
            }

            return _compilerResults.CompiledAssembly;
        }

        private T Func<T>(Assembly compiledAssembly, string methodName, params object[] objectsToInject)
        {
            var cls = compiledAssembly.CreateInstance("DemoCode");
            if (cls == null)
            {
                throw new Exception("Your script needs to contain a public class called DemoCode");
            }

            var typeInfo = cls.GetType();
            var mtd = typeInfo.GetMethod(methodName);
            if (mtd == null)
            {
                throw new Exception("The DemoCode class in your script needs to contain a public method called " + methodName);
            }

            try
            {
                return (T)mtd.Invoke(cls, objectsToInject);
            }
            catch (TargetInvocationException tie)
            {
                var baseException = tie.GetBaseException();
                var stackTrace = new StackTrace(baseException, true);
                var firstFrameWithALineNumber = stackTrace.GetFrames().FirstOrDefault(frame => frame.GetFileLineNumber() != 0);
                if (firstFrameWithALineNumber != null)
                {
                    throw new Exception(baseException.Message);
                    //throw new InvalidScriptException(firstFrameWithALineNumber.GetFileLineNumber(), firstFrameWithALineNumber.GetFileColumnNumber(), baseException.Message, "", "GetXML");
                }
                else
                {
                    //throw new InvalidScriptException(0, 0, baseException.Message, "", methodName);
                    throw new Exception(baseException.Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbCode.Items.Add(new CodeSnippet()
            {
                Caption = "Match All",
                Code = @".MatchAll()"
            });


            cbCode.Items.Add(new CodeSnippet()
            {
                Caption = "Date Search",
                Code = @"   .Query(q => q
                            .DateRange(r => r
                                .Field(f => f.creationDate)
                                .GreaterThanOrEquals(new DateTime(2000, 01, 01))
                                .LessThan(new DateTime(2018, 01, 01))
                            ))"
            });

            cbCode.Items.Add(new CodeSnippet()
            {
                Caption = "Advanced Search",
                Code = $@"    .Query(q => q
        .Bool(b => b
            .Must(mu => mu
                .Match(m => m 
                    .Field(f => f.ownerDisplayName)
                    .Query(""David"")
                ), mu => mu
                .Match(m => m
                    .Field(f => f.lastEditorDisplayName)
                    .Query(""David"")
                )
            )
            .Filter(fi => fi
                 .DateRange(r => r
                    .Field(f => f.creationDate)
                    .GreaterThanOrEquals(new DateTime(2000, 01, 01))
                    .LessThan(new DateTime(2018, 01, 01))
                )
            )
        )
    )"
            });

        }

        private void cbCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCode.SelectedItem is CodeSnippet code)
            {
                scintilla.Text = @"var searchResponse = client.Search<post>(s => s
" + code.Code + @"
);";
            }
        }
    }


    class CodeSnippet
    {
        public string Caption { get; set; }
        public string Code { get; set; }
        public override string ToString() => Caption;

    }


}
