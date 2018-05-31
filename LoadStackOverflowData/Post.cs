using System;

namespace LoadStackOverflowData
{
    internal class post
    {
        public int id { get; set; }
        public int score { get; set; }

        public string title { get; set; }
        public string body { get; set; }
        public string lastEditorDisplayName { get; set; }
        public string ownerDisplayName { get; set; }
        public string postType { get; set; }
        public string[] tags { get; set; }

        public int answerCount { get; set; }
        public int commentCount { get; set; }
        public int favoriteCount { get; set; }
        public int viewCount { get; set; }

        public DateTime? closedDate { get; set; }
        public DateTime? communityOwnedDate { get; set; }
        public DateTime? creationDate { get; set; }
        public DateTime? lastActivityDate { get; set; }
        public DateTime? lastEditDate { get; set; }
    }
}


