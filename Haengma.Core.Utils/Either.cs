namespace Haengma.Core.Utils
{
    public abstract record Either<L, R>
    {
        public sealed record Left(L Value) : Either<L, Nothing>();
        public sealed record Right(R Value) : Either<Nothing, R>();

        public bool IsLeft => this switch
        {
            Left _ => true,
            _ => false
        };

        public bool IsRight => this switch
        {
            Right _ => true,
            _ => false
        };

        public static implicit operator L?(Either<L, R> either) => either switch
        {
            Left left => left.Value,
            _ => default
        };

        public static implicit operator R?(Either<L, R> either) => either switch
        {
            Right right => right.Value,
            _ => default
        };
    }
}
