namespace BrandonUtils.Standalone.Hierarchic {
    public interface IDependant<out TGuardian, TDependant>
        where TGuardian : IGuardian<TGuardian, TDependant>
        where TDependant : IDependant<TGuardian, TDependant> {
        TGuardian Guardian { get; }
    }
}