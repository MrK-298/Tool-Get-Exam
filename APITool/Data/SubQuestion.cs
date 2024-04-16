

using MongoDB.Bson;

namespace ConsoleApp1.Data
{
    public class SubQuestion
    {
        public ObjectId Id { get; set; }
        public string text { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
