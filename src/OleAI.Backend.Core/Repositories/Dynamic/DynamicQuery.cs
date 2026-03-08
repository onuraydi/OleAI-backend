using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OleAI.Backend.Core.Repositories.Dynamic
{
    public class DynamicQuery
    {
        public IEnumerable<Sort>? Sort { get; set; }
        public Filter? Filter { get; set; }
    }

    public class Sort
    {
        public string Field { get; set; } = default!; // price
        public string Dir { get; set; } = default!; // "asc" veya "desc"
    }

    public class Filter
    {
        public string Field { get; set; } = default!; // "Quantity"
        public string Operator { get; set; } = default!; // (Greater Than or Equal)
        public string? Value { get; set; } = default!; // "50"
        public string? Logic { get; set; } = default!; // "and", "or"
        public IEnumerable<Filter>? Filters { get; set; }
    }
}
