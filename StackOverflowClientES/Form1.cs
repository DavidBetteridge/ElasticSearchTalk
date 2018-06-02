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

        private readonly ElasticClient _elasticClient;

        private string _scollID;

        private const string Event_Log_Source = "Stack Overflow Client (ES)";
        private const string Highlight_Token = "%^%";

        public Form1()
        {
            InitializeComponent();
            cmdNoOfResults.Text = "10";

            var settings = new ConnectionSettings(new Uri(End_Point)).DefaultIndex("stackoverflow");
            _elasticClient = new ElasticClient(settings);
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

            this.Text = "Stack Overflow client (Elasticsearch) - searching...";
            var sw = new Stopwatch();
            sw.Start();

            var searchResponse = _elasticClient.Search<post>(s => s
               .Size(int.Parse(numberOfResults))
               .Scroll(new Time(TimeSpan.FromMinutes(1)))
               .Highlight(highlightSelector)
               .Query(q => q
                    .Match(m => m
                       .Field(f => f.body)
                      .Query(searchTerm)
                    )
               )
           );
            this.Text = "Stack Overflow client (Elasticsearch) - drawing...";
            sw.Stop();

            _scollID = searchResponse.ScrollId;

            var results = DisplayResults(searchResponse);

            this.Text = $"Stack Overflow Client (Elasticsearch) - {searchResponse.Documents.Count} results found in {Math.Round(sw.Elapsed.TotalSeconds, 2)}s";
            EventLog.WriteEntry(Event_Log_Source, $"Search for {searchTerm} found {results} results found in {Math.Round(sw.Elapsed.TotalSeconds, 2)}s.");
        }

        private IHighlight highlightSelector(HighlightDescriptor<post> arg)
        {
            return arg
                    .PreTags(Highlight_Token)
                    .PostTags(Highlight_Token)
                    .Encoder(HighlighterEncoder.Default)
                    .Fields(
                            fs => fs.Field(p => p.body),
                            fs => fs.Field(p => p.title)
                            );
        }

        private int DisplayResults(ISearchResponse<post> searchResponse)
        {
            var g = this.panelResults.CreateGraphics();
            g.Clear(panelResults.BackColor);
            
            var top = 30;
            var shaded = false;
            var results = 0;
            foreach (var document in searchResponse.Hits)
            {
                // As we displaying a summary we only need to take the first highlight segment.  In real-life you will 
                // want to take all the segments.
                var text = document.Highlights.First().Value.Highlights.First();

                text = text.Replace("\n", "");

                // Split the words up into a array,  where every other entry will require highlighting.
                var words = text.Split(new string[] { Highlight_Token }, StringSplitOptions.None);
                var highlightText = false;
                var x = 16;
                foreach (var word in words)
                {
                    var t = word;
                    if (t.Length > 100) t = word.Substring(0, 100);
                    var stringSize = g.MeasureString(t, this.Font);

                    if (highlightText)
                    {
                        var rect = new Rectangle(x, top, (int)stringSize.Width, (int)stringSize.Height);
                        g.FillRectangle(Brushes.LightYellow, rect);
                    }

                    g.DrawString(t, this.Font, Brushes.Black, x, top);

                    highlightText = !highlightText;
                    x += (int)stringSize.Width;
                }

                shaded = !shaded;
                top += 40;
                results++;
            }

            return results;
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

        /// <summary>
        /// Scroll to the next page of results.  Note:  This isn't really how scroll is intended to be used,  as the results
        /// can only be accessed until the scroll time has expired.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdNext_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var searchResponse = _elasticClient.Scroll<post>(new Time(TimeSpan.FromMinutes(1)), _scollID);
            _scollID = searchResponse.ScrollId;  //The scroll ID can change with each batch of results returned.

            sw.Stop();

            DisplayResults(searchResponse);
            this.Text = $"Stack Overflow Client (Elasticsearch) - {searchResponse.Documents.Count} results found in {Math.Round(sw.Elapsed.TotalSeconds, 2)}s";

        }
    }

}
