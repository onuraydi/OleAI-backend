using System.Linq.Dynamic.Core;
using System.Text;

namespace OleAI.Backend.Core.Repositories.Dynamic
{
    public static class IQueryableDynamicFilterExtensions
    {
        public static IQueryable<T> ToDynamic<T>(this IQueryable<T> query, DynamicQuery dynamicQuery)
        {
            if (dynamicQuery.Filter is not null)
            {
                IList<Filter> filters = GetAllFilters(dynamicQuery.Filter);
                string? val = transform(dynamicQuery.Filter, filters);

                if (!string.IsNullOrEmpty(val))
                {
                    query = query.Where(val, filters.Select(f => f.Value).ToArray());
                }
            }

            if (dynamicQuery.Sort is not null && dynamicQuery.Sort.Any())
            {
                string ordering = string.Join(",", dynamicQuery.Sort.Select(s => $"{s.Field} {s.Dir}"));
                query = query.OrderBy(ordering);
            }

            return query;
        }


        private static List<Filter> GetAllFilters(Filter filter)
        {
            List<Filter> filters = new();
            GetFilters(filter, filters);
            return filters;
        }

        private static void GetFilters(Filter filter, IList<Filter> filters)
        {
            filters.Add(filter);
            if (filter.Filters is not null && filter.Filters.Any())
                foreach (var item in filter.Filters)
                    GetFilters(item, filters);
        }

        private static string transform(Filter filter, IList<Filter> filters)
        {
            int index = filters.IndexOf(filter);
            string comparison = Operators[filter.Operator];
            StringBuilder where = new();

            // Null veya is not null durumları için özel kontrol
            if (!string.IsNullOrEmpty(filter.Value))
            {
                if (filter.Operator == "doesnotcontain")
                    where.Append($"(!np({filter.Field}).{comparison}(@{index}))");
                else if (comparison == "StartsWith" || comparison == "EndsWith" || comparison == "Contains")
                    where.Append($"(np({filter.Field}).{comparison}(@{index}))");
                else
                    where.Append($"np({filter.Field}) {comparison} @{index}");
            }
            else if (filter.Operator == "isnull" || filter.Operator == "isnotnull")
            {
                where.Append($"np({filter.Field}) {comparison}");
            }

            // AND / OR mantığı varsa iç içe filtreleri birleştir
            if (filter.Logic is not null && filter.Filters is not null && filter.Filters.Any())
            {
                return $"{where} {filter.Logic} ({string.Join($" {filter.Logic} ", filter.Filters.Select(f => transform(f, filters)).ToArray())})";
            }

            return where.ToString();
        }

        private static readonly Dictionary<string, string> Operators = new Dictionary<string, string>
        {
            { "eq", "==" },
            { "neq", "!=" },
            { "lt", "<" },
            { "lte", "<=" },
            { "gt", ">" },
            { "gte", ">=" },
            { "isnull", "== null" },
            { "isnotnull", "!= null" },
            { "startswith", "StartsWith" },
            { "endswith", "EndsWith" },
            { "contains", "Contains" },
            { "doesnotcontain", "Contains" }
        };
    }
}
