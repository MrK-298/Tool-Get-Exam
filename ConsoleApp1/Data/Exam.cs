using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Data
{
    public class Exam
    {
        public ObjectId Id { get; set; }
        public string name { get; set; }
        public List<Question> Questions { get; set; }
        public bool isDelete { get; set; }
    }
}
