using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

/*
 * Things to note:
 * 
 * The properties in the post class start with lowercase letters
 * The Tags value is converted into an array.  e.g  "<c#><javascript>" becomes ["c#","javascript"]
 * The data is loaded in blocks of 10000 using the direct rest calls
 */

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
                                      LEFT JOIN [dbo].[PostTypes] PT ON PT.Id = P.PostTypeId
                                      LEFT JOIN [dbo].[Users] U ON U.Id = P.OwnerUserId";


                    using (var dr = cmd.ExecuteReader())
                    {
                        var totalLoaded = 0;
                        var posts = new List<post>();
                        var sw = new Stopwatch();
                        sw.Start();

                        while (dr.Read())
                        {
                            totalLoaded++;
                            posts.Add(new post()
                            {
                                id = (int)dr["Id"],
                                body = (string)dr["Body"],
                                title = (string)dr["Title"],
                                answerCount = (int)dr["AnswerCount"],
                                commentCount = (int)dr["CommentCount"],
                                favoriteCount = (int)dr["FavoriteCount"],
                                viewCount = (int)dr["ViewCount"],
                                closedDate = dr["ClosedDate"] as DateTime?,
                                communityOwnedDate = dr["CommunityOwnedDate"] as DateTime?,
                                creationDate = dr["CreationDate"] as DateTime?,
                                lastActivityDate = dr["LastActivityDate"] as DateTime?,
                                lastEditDate = dr["LastEditDate"] as DateTime?,
                                lastEditorDisplayName = dr["LastEditorDisplayName"] as string,
                                ownerDisplayName = dr["OwnerDisplayName"] as string,
                                postType = dr["PostType"] as string,
                                score = (int)dr["Score"],
                                tags = CleanTags(dr["Tags"] as string).Split('+')
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

        private static string CleanTags(string tags) => string.IsNullOrWhiteSpace(tags) ? "" : tags.Replace("><", "+").Replace("<", "").Replace(">", "");


        private static void LoadPosts(List<post> posts, HttpClient client)
        {
            var sb = new StringBuilder();
            foreach (var post in posts)
            {
                sb.AppendLine(@"{ ""create"": { ""_index"": ""stackoverflow"", ""_type"": ""post"", ""_id"": """ + post.id + @""" }}");

                var payload = JsonConvert.SerializeObject(post);
                sb.AppendLine(payload);
            }

            var content = new StringContent(sb.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync("/_bulk", content).Result;
            var what = result.Content.ReadAsStringAsync().Result;
        }


    }
}
