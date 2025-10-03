using Application.Commons.Bases.Request;
using MongoDB.Entities;
using System.Linq.Expressions;
using System.Reflection;

namespace Infraestructure.Helpers
{
    public static class MongoOrderingHelper
    {
        public static PagedSearch<T, T> ApplyOrdering<T>(this PagedSearch<T, T> query, BasePaginationRequest request,
            string defaultSortField) where T : Entity
        {
            if (string.IsNullOrWhiteSpace(request.Sort))
                request.Sort = defaultSortField;

            var type = typeof(T);

            // Verifica si sortField existe (ignora mayúsculas)
            var propertyInfo = type.GetProperty(request.Sort, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var fieldInfo = type.GetField(request.Sort, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            // Si no existe, usar el defaultSortField
            if (propertyInfo == null && fieldInfo == null)
                request.Sort = defaultSortField;

            var param = Expression.Parameter(type, "x");
            var property = Expression.PropertyOrField(param, request.Sort);
            var converted = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(converted, param);

            var itemsOrdered = request.Order?.ToLower() == "desc"
                ? query.Sort(x => x.Descending(lambda))
                : query.Sort(x => x.Ascending(lambda));

            return itemsOrdered;
        }
    }
}