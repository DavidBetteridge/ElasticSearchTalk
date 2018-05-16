using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StackOverflowClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cmdNoOfResults.Text = "10";
        }

        private async void cmdSearch_Click(object sender, EventArgs e)
        {
            var searchTerm = this.txtSearchTerm.Text.Trim();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                MessageBox.Show("Please enter a search term", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var topCount = "";
            var numberOfResults = cmdNoOfResults.Text;
            if (numberOfResults != "All")
                topCount = " top " + numberOfResults;

            var sb = new SqlConnectionStringBuilder
            {
                InitialCatalog = "StackOverflow2010",
                DataSource = ".",
                IntegratedSecurity = true
            };
            var cs = sb.ToString();

            using (var cn = new SqlConnection(cs))
            {
                await cn.OpenAsync();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = $@"SELECT {topCount} Id, Body, IsNull(Title,'') as Title
                                          FROM dbo.Posts
                                         WHERE Body like @SearchTerm";

                    cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    this.Text = "Stack Overflow Client - Searching...";
                    var results = 0;
                    var sw = new Stopwatch();
                    sw.Start();
                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        this.panelResults.Controls.Clear();

                        var top = 30;
                        var shaded = false;
                        while (await dr.ReadAsync())
                        {
                            var lbl = new Label
                            {
                                Location = new Point(16, top),
                                Size = new Size(panelResults.Width - 32, 30),
                                TabIndex = 0,
                                Text = Summary((string)dr["Title"], (string)dr["Body"]),
                                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                            };
                            if (shaded) lbl.BackColor = Color.LightGray;
                            this.panelResults.Controls.Add(lbl);
                            shaded = !shaded;
                            top += 40;
                            results++;
                        }
                    }

                    sw.Stop();
                    this.Text = $"Stack Overflow Client - {results}results found in {Math.Round(sw.Elapsed.TotalSeconds,2)}s";


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
}
