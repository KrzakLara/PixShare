using System;

namespace PixShareLIB.Models
{
    public class FilterCriteria
    {
        public int? AuthorId { get; set; }
        public string Username { get; set; } // Add this property
        public string Hashtags { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? MinSize { get; set; }
        public long? MaxSize { get; set; }
    }


}
