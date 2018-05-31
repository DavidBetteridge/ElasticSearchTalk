using Nest;
using Newtonsoft.Json;
using ScintillaNET;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace NESTClient
{
    public partial class Form1 : Form
    {
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

        private void button1_Click(object sender, EventArgs e)
        {

            var assm = Generate(@"using Nest;
using System;  
using NESTClient;
public class DemoCode
{
public ISearchResponse<post> RunDemo(ElasticClient client)
{
" + scintilla.Text + @"
return searchResponse;
}
}");

            var settings = new ConnectionSettings(new Uri("http://ipv4.fiddler:9200"))
                    .DefaultIndex("stackoverflow");

            var client = new ElasticClient(settings);

            var searchResponse = Func<ISearchResponse<post>>(assm, "RunDemo", client);


            //var searchResponse = client.Search<Post>(s => s
            //    .Size(1)
            //    .Query(q => q
            //         .Match(m => m
            //            .Field(f => f.Body)
            //           .Query("unicorn")
            //         )
            //    )
            //);

            this.panelResults.Controls.Clear();
            var top = 30;
            var shaded = false;

            foreach (var document in searchResponse.Documents)
            {
                var lbl = new Label
                {
                    Location = new Point(16, top),
                    Size = new Size(panelResults.Width - 32, 30),
                    TabIndex = 0,
                    Text = Summary(document.Title, document.Body),
                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                };
                if (shaded) lbl.BackColor = Color.LightGray;
                this.panelResults.Controls.Add(lbl);
                shaded = !shaded;
                top += 40;
            }

        }

        private Assembly Generate(string sourceCode)
        {
            // Environment.SetEnvironmentVariable("ROSLYN_COMPILER_LOCATION", @"C:\Code\P2P\Features\Web Site\P2P.Web\bin", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("ROSLYN_COMPILER_LOCATION", @"C:\Users\david.betteridge\.nuget\packages\microsoft.net.compilers\2.7.0\tools", EnvironmentVariableTarget.Process);
            var _codeDomProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
            // Environment.SetEnvironmentVariable("ROSLYN_COMPILER_LOCATION", null, EnvironmentVariableTarget.Process);

            var _compilerParameters = new CompilerParameters();
            _compilerParameters.ReferencedAssemblies.Add("system.dll");
            _compilerParameters.ReferencedAssemblies.Add("system.xml.dll");
            _compilerParameters.ReferencedAssemblies.Add("system.core.dll");
            _compilerParameters.ReferencedAssemblies.Add("system.linq.dll");
            _compilerParameters.ReferencedAssemblies.Add(@"C:\Code\Elastic Search Talk\ElasticSearchRESTDemo\packages\NEST.6.1.0\lib\net46\Nest.dll");
            _compilerParameters.ReferencedAssemblies.Add(@"C:\Code\Elastic Search Talk\NESTClient\bin\Debug\NESTClient.exe");
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
                    throw new InvalidScriptException(firstFrameWithALineNumber.GetFileLineNumber(), firstFrameWithALineNumber.GetFileColumnNumber(), baseException.Message, "", "GetXML");
                }
                else
                {
                    throw new InvalidScriptException(0, 0, baseException.Message, "", methodName);
                }
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
    }

    public class post
    {
        public int ID { get; set; }
        public int Score { get; set; }

        public string Title { get; set; }

        [JsonProperty("Body")]
        public string Body { get; set; }


        public string LastEditorDisplayName { get; set; }
        public string OwnerDisplayName { get; set; }
        public string PostType { get; set; }
        public string Tags { get; set; }

        public int AnswerCount { get; set; }
        public int CommentCount { get; set; }
        public int FavoriteCount { get; set; }
        public int ViewCount { get; set; }

        public DateTime? ClosedDate { get; set; }
        public DateTime? CommunityOwnedDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastEditDate { get; set; }
    }

}
