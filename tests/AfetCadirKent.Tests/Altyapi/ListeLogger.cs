using Microsoft.Extensions.Logging;

namespace AfetCadirKent.Tests.Altyapi;

/// <summary>Yazılan log kayıtlarını saklayan test logger'ı.</summary>
public sealed class ListeLogger<T> : ILogger<T>
{
    public List<(LogLevel Seviye, string Mesaj, Exception? Hata)> Kayitlar { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
        Kayitlar.Add((logLevel, formatter(state, exception), exception));
}
