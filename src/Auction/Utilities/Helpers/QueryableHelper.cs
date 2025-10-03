using System.Linq.Dynamic.Core;

namespace Utilities.Helpers
{
    public static class QueryableHelper
    {

        public static IQueryable<TDTO> Ordering<TDTO>(this IQueryable<TDTO> queryable, string order, string sortField, int numPage = 1, int records = 10, bool pagination = false) where TDTO : class
        {
            var direction = order?.ToLower() == "desc" ? "descending" : "ascending";

            IQueryable<TDTO> queryDto = queryable.OrderBy($"{sortField} {direction}");

            if (pagination)
            {
                queryDto = queryDto.Paginate(numPage, records);
            }

            return queryDto;
        }
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, int numPage, int records)
        {
            return queryable.Skip((numPage - 1) * records).Take(records);
        }
    }
}
