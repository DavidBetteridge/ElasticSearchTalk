using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElasticSearch.Net_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private  void button1_Click(object sender, EventArgs e)
        {
            //var settings = new ConnectionConfiguration(new Uri("http://ipv4.fiddler:9200"))
            var settings = new ConnectionConfiguration(new Uri("http://localhost:9200"))
                //.ThrowExceptions()
                .RequestTimeout(TimeSpan.FromMinutes(2));

            var lowlevelClient = new ElasticLowLevelClient(settings);

            var searchResponse = lowlevelClient.Search<StringResponse>("stackoverflow","post", PostData.Serializable(new
            {
                size = 10,
                query = new
                {
                    match = new
                    {
                        body = new { query = "unicorn" }
                    }
                }
            }));

            var successful = searchResponse.Success;
            var responseJson = searchResponse.Body;
            var successOrKnownError = searchResponse.SuccessOrKnownError;
            var exception = searchResponse.OriginalException;
        }

        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        private async void cmdInsert_Click(object sender, EventArgs e)
        {
            //var settings = new ConnectionConfiguration(new Uri("http://ipv4.fiddler:9200"))
            var settings = new ConnectionConfiguration(new Uri("http://localhost:9200"))
                .RequestTimeout(TimeSpan.FromMinutes(2));

            var lowlevelClient = new ElasticLowLevelClient(settings);

            var person = new Person
            {
                FirstName = "Martijn",
                LastName = "Laarman"
            };

            var indexResponse = lowlevelClient.Index<BytesResponse>("people", "person", "1", PostData.Serializable(person));
            byte[] responseBytes = indexResponse.Body;

            var asyncIndexResponse = await lowlevelClient.IndexAsync<StringResponse>("people", "person", "1", PostData.Serializable(person));
            string responseString = asyncIndexResponse.Body;


        }
    }
}
