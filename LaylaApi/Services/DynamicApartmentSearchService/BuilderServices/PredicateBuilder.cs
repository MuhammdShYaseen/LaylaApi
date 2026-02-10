using System.Linq.Expressions;

namespace LaylaApi.Services.DynamicApartmentSearchService.BuilderServices
{
    internal static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() => _ => true;
        public static Expression<Func<T, bool>> False<T>() => _ => false;

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            var parameter = Expression.Parameter(typeof(T), "x");
            var leftBody = ParameterReplacer.Replace(left.Body, left.Parameters[0], parameter);
            var rightBody = ParameterReplacer.Replace(right.Body, right.Parameters[0], parameter);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(leftBody, rightBody), parameter);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            var parameter = Expression.Parameter(typeof(T), "x");
            var leftBody = ParameterReplacer.Replace(left.Body, left.Parameters[0], parameter);
            var rightBody = ParameterReplacer.Replace(right.Body, right.Parameters[0], parameter);

            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(leftBody, rightBody), parameter);
        }

        private static class ParameterReplacer
        {
            internal static Expression Replace(
                Expression body,
                ParameterExpression oldParam,
                ParameterExpression newParam)
            {
                return new Visitor(oldParam, newParam).Visit(body)!;
            }

            private sealed class Visitor : ExpressionVisitor
            {
                private readonly ParameterExpression _oldParam;
                private readonly ParameterExpression _newParam;

                public Visitor(ParameterExpression oldParam, ParameterExpression newParam)
                {
                    _oldParam = oldParam;
                    _newParam = newParam;
                }

                protected override Expression VisitParameter(ParameterExpression node)
                    => node == _oldParam ? _newParam : base.VisitParameter(node);
            }
        }
    }
}
