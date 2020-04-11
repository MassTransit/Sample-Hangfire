using System;

namespace Sample.Hangfire.Publisher
{
    public interface IMessage
    {
        Guid Id { get; }
        string Message { get; }
    }
}
