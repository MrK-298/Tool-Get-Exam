using MongoDB.Bson;

namespace ConsoleApp1.Data
{
    public class Answer
    {
        public ObjectId Id { get; set; }
        public string text { get; set; }
        public bool isTrue { get; set; }
    }
}
