using System;
using System.Collections.Generic;

namespace CloneDto
{
    [Serializable]
    public class ComplexDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public List<Detail> Details { get; set; }
    }

    [Serializable]
    public class Detail
    {
        public string Name { get; set; }
    }
}
