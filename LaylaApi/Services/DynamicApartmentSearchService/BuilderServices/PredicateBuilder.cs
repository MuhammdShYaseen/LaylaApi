using System.Linq.Expressions;

namespace LaylaApi.Services.DynamicApartmentSearchService.BuilderServices
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>()
        => _ => true;

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T));

            var body = Expression.AndAlso(
                Expression.Invoke(left, param),
                Expression.Invoke(right, param));

            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}
