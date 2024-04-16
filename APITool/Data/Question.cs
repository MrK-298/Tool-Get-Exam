using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Data
{
    public class Question
    {
        public ObjectId Id { get; set; }
        public string type { get; set; }
        public string questionText { get; set; }
        [BsonIgnoreIfNull]
        public List<Answer> Answers { get; set; }
        [BsonIgnoreIfNull]
        public string imageUrl { get; set; }
        [BsonIgnoreIfNull]
        public List<SubQuestion> SubQuestions { get; set; }
    }

}
