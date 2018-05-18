using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElasticSearchRESTDemo
{

    public partial class Form1 : Form
    {
        private readonly HttpClient _client;
        public Form1()
        {
            InitializeComponent();
            SetupMenus();

            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:9200");
            _client.DefaultRequestHeaders
                   .Accept
                   .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async void cmdExecute_Click(object sender, EventArgs e)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                switch (this.cbMethod.Text)
                {
                    case "GET":
                        await DoGet();
                        break;

                    case "PUT":
                        await DoPut();
                        break;

                    default:
                        break;
                }

                sw.Stop();
                lblTimeTaken.Text = $"{Math.Round(sw.Elapsed.TotalSeconds, 2)}s";
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private async Task DoGet()
        {
            var url = txtURL.Text;
            var result = await _client.GetStringAsync(url);
            txtResult.Text = PrettyPrintJSON(result);
        }

        private async Task DoPut()
        {
            var url = this.txtURL.Text;
            var command = this.txtCommand.Text;

            var content = new StringContent(command, Encoding.UTF8, "application/json");
            var result = await _client.PutAsync(url, content);
            var json = await result.Content.ReadAsStringAsync();

            txtResult.Text = PrettyPrintJSON(json);
        }



        private void SetupMenus()
        {
            var systemDemos = AddDemoCategory("System Demos");
            var defaultDefinition = new DemoDefinition() { Title = "Overview", Method = "GET", URL = "/" };
            AddDemo(systemDemos, defaultDefinition);
            AddDemo(systemDemos, new DemoDefinition() { Title = "Cluster Health", Method = "GET", URL = "/_cluster/health" });

            var gettingStarted = AddDemoCategory("Getting Started");
            AddDemo(gettingStarted, new DemoDefinition() { Title = "Add Document", Method = "PUT", URL = "/meetups/dotnetyork/june2018", Content = @"{
                                                                                            ""title"": ""Double Bill - Elasticsearch and JavaScript Performance"",
                                                                                            ""speakers"":  ""David Betteridge and Benjamin Howarth"",
                                                                                            ""date"":  ""Friday, June 1, 2018""
                                                                                            }" });


            DisplayDefinition(defaultDefinition);
        }

        private ToolStripMenuItem AddDemoCategory(string text)
        {
            var parentMenuItem = new ToolStripMenuItem();
            parentMenuItem.Size = new Size(97, 20);
            parentMenuItem.Text = text;
            this.menuStrip1.Items.Add(parentMenuItem);
            return parentMenuItem;
        }

        private void AddDemo(ToolStripMenuItem parentMenuItem, DemoDefinition demoDefinition)
        {
            var menuItem = new ToolStripMenuItem();
            menuItem.Size = new Size(152, 22);
            menuItem.Text = demoDefinition.Title;
            menuItem.Click += MenuItem_Click;
            menuItem.Tag = demoDefinition;
            parentMenuItem.DropDownItems.Add(menuItem);
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menu)
            {
                if (menu.Tag is DemoDefinition demo)
                {
                    DisplayDefinition(demo);
                }
            }
        }

        private void DisplayDefinition(DemoDefinition demo)
        {
            this.txtURL.Text = demo.URL;
            this.cbMethod.Text = demo.Method;
            this.txtCommand.Text = PrettyPrintJSON(demo.Content);
            this.txtResult.Text = "";
            this.lblTitle.Text = demo.Title;
        }

        private static string PrettyPrintJSON(string result)
        {
            if (string.IsNullOrWhiteSpace(result))
                return "";
            else
                return JToken.Parse(result).ToString(Formatting.Indented);
        }

        private class DemoDefinition
        {
            public string Method { get; set; }
            public string URL { get; set; }
            public string Content { get; set; }
            public string Title { get; set; }
        }

    
    }
}
