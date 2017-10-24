namespace Yuxi.SharedKernel.Specification.Implementations
{
    using Base;

    public sealed class AllSpecification<TEntity> : CompositeSpecification<TEntity>
    {
        #region Overriden Methods

        public override bool IsSatisfiedBy(TEntity entityToTest)
        {
            return true;
        }

        #endregion
    }
}