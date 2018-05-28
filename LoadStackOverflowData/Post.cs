using System;

namespace LoadStackOverflowData
{
    internal class Post
    {
        public int ID { get; set; }
        public int Score { get; set; }

        public string Title { get; set; }
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


