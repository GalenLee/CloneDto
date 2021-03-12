using System;

namespace CloneDto
{
    public class CloneableDto : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }

        public object Clone()
        {
            return new CloneableDto
            {
                Id = this.Id,
                Name = this.Name,
                BirthDate = this.BirthDate
            };
        }
    }
}