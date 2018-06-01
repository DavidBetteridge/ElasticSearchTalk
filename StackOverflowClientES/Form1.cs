using Nest;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StackOverflowClient
{
    public partial class Form1 : Form
    {
        private const string End_Point = "http://localhost:9200";

        private const string Event_Log_Source = "Stack Overflow Client (ES)";
        public Form1()
        {
            InitializeComponent();
            cmdNoOfResults.Text = "10";
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            var searchTerm = this.txtSearchTerm.Text.Trim();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                MessageBox.Show("Please enter a search term", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var numberOfResults = cmdNoOfResults.Text;

            this.Text = "Stack Overflow client (Elastic Search) - searching...";
            var results = 0;
            var sw = new Stopwatch();
            sw.Start();

            var settings = new ConnectionSettings(new Uri(End_Point)).DefaultIndex("stackoverflow");

            var client = new ElasticClient(settings);

             var searchResponse = client.Search<post>(s => s
                .Size(int.Parse(numberOfResults))
                .Query(q => q
                     .Match(m => m
                        .Field(f => f.body)
                       .Query(searchTerm)
                     )
                )
            );
            this.Text = "Stack Overflow client (Elastic Search) - drawing...";
            sw.Stop();
            this.panelResults.Controls.Clear();

            var top = 30;
            var shaded = false;
            foreach (var document in searchResponse.Documents.Take(100))
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
                results++;
            }

            
            this.Text = $"Stack Overflow Client (Elastic Search) - {searchResponse.Documents.Count}results found in {Math.Round(sw.Elapsed.TotalSeconds, 2)}s";
            EventLog.WriteEntry(Event_Log_Source, $"Search for {searchTerm} found {results}results found in {Math.Round(sw.Elapsed.TotalSeconds, 2)}s.");
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

}
