using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            _client.BaseAddress = new Uri("http://ipv4.fiddler:9200");
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

                    case "POST":
                        await DoPost();
                        break;

                    case "DELETE":
                        await DoDelete();
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

        private async Task DoDelete()
        {
            var url = txtURL.Text;
            var result = await _client.DeleteAsync(url);
            var json = await result.Content.ReadAsStringAsync();

            txtResult.Text = PrettyPrintJSON(json);
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

        private async Task DoPost()
        {
            var url = this.txtURL.Text;
            var command = this.txtCommand.Text;

            var content = new StringContent(command, Encoding.UTF8, "application/json");
            var result = await _client.PostAsync(url, content);
            var json = await result.Content.ReadAsStringAsync();

            txtResult.Text = PrettyPrintJSON(json);
        }


        private void SetupMenus()
        {
            var defaultDefinition = default(DemoDefinition);
            foreach (var file in Directory.GetFiles("demos", "*.txt"))
            {
                var category = AddDemoCategory(Path.GetFileNameWithoutExtension(file));

                using (var contents = File.OpenText(file))
                {
                    var line = "";
                    while ((line = contents.ReadLine()) != null)
                    {
                        var title = line;
                        line = contents.ReadLine();
                        var method = line.Split(' ')[0];
                        var url = line.Split(new char[] { ' ', '\t' })[1];

                        var payload = "";
                        if (method.StartsWith("PUT") || method.StartsWith("POST"))
                        {
                            while ((line = contents.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
                            {
                                payload += line;
                            }
                        }

                        if (method.StartsWith("GET") || method.StartsWith("DELETE"))
                        {
                            SkipBlankLine(contents);
                        }


                        defaultDefinition = new DemoDefinition() { Title = title, Method = method, URL = url, Content = payload };
                        AddDemo(category, defaultDefinition);
                    }
                }
            }



            DisplayDefinition(defaultDefinition);
        }

        private static void SkipBlankLine(StreamReader contents)
        {
            contents.ReadLine();
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
            this.lblTitle.Text = "  Demo - " + demo.Title;
        }

        private static string PrettyPrintJSON(string result)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(result))
                    return "";
                else
                    return JToken.Parse(result).ToString(Formatting.Indented);

            }
            catch (Exception)
            {
                return result;
            }
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
