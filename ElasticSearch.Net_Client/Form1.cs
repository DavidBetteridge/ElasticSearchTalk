using Elasticsearch.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;

namespace ElasticSearch.Net_Client
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

        private void cmdInsert_Click(object sender, EventArgs e)
        {
            var settings = new ConnectionConfiguration(EndPoint())
                .RequestTimeout(TimeSpan.FromMinutes(2));

            var lowlevelClient = new ElasticLowLevelClient(settings);

            var person = new Person
            {
                firstName = "David",
                lastName = "Betteridge"
            };

            var indexResponse = lowlevelClient.Index<BytesResponse>("people", "person", "David", PostData.Serializable(person));
            var responseBytes = indexResponse.Body;
            txtResult.Text = PrettyPrintJSON(BytesToString(responseBytes));

            /************************************************************************************************
                                    We can also get the result in string format
                                                           \/
            *************************************************************************************************/
            //var indexResponse = lowlevelClient.Index<StringResponse>("people", "person", "David", PostData.Serializable(person));
            //var responseString = indexResponse.Body;
            //txtResult.Text = PrettyPrintJSON(responseString);
        }

        private static string BytesToString(byte[] responseBytes)
            => System.Text.Encoding.UTF8.GetString(responseBytes, 0, responseBytes.Length);

        private async void cmdInsertAsync_Click(object sender, EventArgs e)
        {
            var settings = new ConnectionConfiguration(EndPoint())
                .RequestTimeout(TimeSpan.FromMinutes(2));

            var lowlevelClient = new ElasticLowLevelClient(settings);

            var person = new Person
            {
                firstName = "Rebecca",
                lastName = "Betteridge"
            };

            var asyncIndexResponse = await lowlevelClient.IndexAsync<StringResponse>("people", "person", "Rebecca", PostData.Serializable(person));
            var responseString = asyncIndexResponse.Body;
            txtResult.Text = PrettyPrintJSON(responseString);
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            var settings = new ConnectionConfiguration(EndPoint())
                //.ThrowExceptions()
                .RequestTimeout(TimeSpan.FromMinutes(2));

            var lowlevelClient = new ElasticLowLevelClient(settings);

            var searchResponse = lowlevelClient.Search<StringResponse>("people", "person", PostData.Serializable(new
            {
                size = 10,
                query = new
                {
                    match = new
                    {
                        firstName = new { query = "David" }
                    }
                }
            }));

            var successful = searchResponse.Success;
            var responseJson = searchResponse.Body;
            var successOrKnownError = searchResponse.SuccessOrKnownError;
            var exception = searchResponse.OriginalException;

            txtResult.Text = PrettyPrintJSON(responseJson);
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

        public Form1()
        {
            InitializeComponent();
        }
    }
}
