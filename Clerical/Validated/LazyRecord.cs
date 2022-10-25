namespace FowlFever.Clerical.Validated;

public abstract record LazyRecord<T> where T : class {
    private   T? _lazyValue;
    protected T  LazyValue => LazyInitializer.EnsureInitialized(ref _lazyValue, _getLazyRecordValue);

    protected bool Reset {
        init {
            if (value) {
                _lazyValue = null;
            }
        }
    }

    protected abstract T _getLazyRecordValue();
}