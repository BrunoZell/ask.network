namespace AskFi.Runtime.Internal;

internal static class NoSynchronizationContextScope
{
    // Copied from https://stackoverflow.com/a/28307965/5185376

    /// <summary>
    /// During the lifetime of the returned disposable the <see cref="SynchronizationContext"/> of this thread is
    /// set to <see langword="null"/>. This allows synchronous functions to wait for asynchronous task without the danger of a deadlock.
    /// </summary>
    public static RestoreSyncronizationContextDisposable Enter()
    {
        var context = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(null);
        return new RestoreSyncronizationContextDisposable(context);
    }

    public readonly struct RestoreSyncronizationContextDisposable : IDisposable, IEquatable<RestoreSyncronizationContextDisposable>
    {
        private readonly SynchronizationContext? _synchronizationContext;

        public RestoreSyncronizationContextDisposable(SynchronizationContext? synchronizationContext) =>
            _synchronizationContext = synchronizationContext;

        public void Dispose() =>
            SynchronizationContext.SetSynchronizationContext(_synchronizationContext);

        public override bool Equals(object? obj) => false;
        public bool Equals(RestoreSyncronizationContextDisposable other) => false;
        public override int GetHashCode() => 0;
        public static bool operator ==(RestoreSyncronizationContextDisposable left, RestoreSyncronizationContextDisposable right) => left.Equals(right);
        public static bool operator !=(RestoreSyncronizationContextDisposable left, RestoreSyncronizationContextDisposable right) => !left.Equals(right);
    }
}
