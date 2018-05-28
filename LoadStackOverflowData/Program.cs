using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LoadStackOverflowData
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:9200");
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var sb = new SqlConnectionStringBuilder
            {
                InitialCatalog = "StackOverflow2010",
                DataSource = ".",
                IntegratedSecurity = true
            };
            var cs = sb.ToString();

            using (var cn = new SqlConnection(cs))
            {
                cn.Open();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = $@"SELECT 
                                                P.Id, 
                                                [AnswerCount], 
                                                IsNull([Body],'') as Body, 
                                                [ClosedDate], 
                                                [CommentCount], 
                                                [CommunityOwnedDate], P.[CreationDate], 
	                                            [FavoriteCount], [LastActivityDate], [LastEditDate], 
                                                [LastEditorDisplayName], [LastEditorUserId], U.DisplayName AS OwnerDisplayName, 
	                                            PT.[Type] as PostType, 
                                                [Score], 
                                                [Tags], 
                                                IsNull([Title],'') as Title,
                                                [ViewCount]
                                      FROM Posts P
                                      JOIN [dbo].[PostTypes] PT ON PT.Id = P.PostTypeId
                                      JOIN [dbo].[Users] U ON U.Id = P.OwnerUserId";


                    using (var dr = cmd.ExecuteReader())
                    {
                        var totalLoaded = 0;
                        var posts = new List<Post>();
                        var sw = new Stopwatch();
                        sw.Start();

                        while (dr.Read())
                        {
                            totalLoaded++;
                            posts.Add(new Post()
                            {
                                ID = (int)dr["Id"],
                                Body = (string)dr["Body"],
                                Title = (string)dr["Title"],
                                AnswerCount = (int)dr["AnswerCount"],
                                CommentCount = (int)dr["CommentCount"],
                                FavoriteCount = (int)dr["FavoriteCount"],
                                ViewCount = (int)dr["ViewCount"],
                                ClosedDate = dr["ClosedDate"] as DateTime?,
                                CommunityOwnedDate = dr["CommunityOwnedDate"] as DateTime?,
                                CreationDate = dr["CreationDate"] as DateTime?,
                                LastActivityDate = dr["LastActivityDate"] as DateTime?,
                                LastEditDate = dr["LastEditDate"] as DateTime?,
                                LastEditorDisplayName = dr["LastEditorDisplayName"] as string,
                                OwnerDisplayName = dr["OwnerDisplayName"] as string,
                                PostType = dr["PostType"] as string,
                                Score = (int)dr["Score"] ,
                                Tags = dr["Tags"] as string
                            });


                            if (posts.Count == 10000)
                            {
                                sw.Stop();
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(totalLoaded);
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write("  Time to fetch :" + Math.Round(sw.Elapsed.TotalSeconds, 2) + "s");
                                sw.Restart();
                                LoadPosts(posts, client);
                                sw.Stop();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("  Time to load:" + Math.Round(sw.Elapsed.TotalSeconds, 2) + "s");
                                sw.Restart();
                                posts.Clear();
                            }
                        }

                        LoadPosts(posts, client);
                    }

                }
            }
        }

        private static void LoadPosts(List<Post> posts, HttpClient client)
        {
            var sb = new StringBuilder();
            foreach (var post in posts)
            {
                sb.AppendLine(@"{ ""create"": { ""_index"": ""stackoverflow"", ""_type"": ""post"", ""_id"": """ + post.ID + @""" }}");

                var payload = JsonConvert.SerializeObject(post);
                sb.AppendLine(payload);
                //sb.AppendLine(@" {""body"":""" + CleanForJSON(post.Body) + @""",  ""title"":""" + CleanForJSON(post.Title) + @"""} ");
            }

            var content = new StringContent(sb.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync("/_bulk", content).Result;
            var what = result.Content.ReadAsStringAsync().Result;
        }

        public static string CleanForJSON(string s)
        {
            if (s == null || s.Length == 0)
            {
                return "";
            }

            char c = '\0';
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

            for (i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + String.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
