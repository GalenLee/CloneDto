using System;

namespace CloneDto
{
    [Serializable]
    public class SimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
