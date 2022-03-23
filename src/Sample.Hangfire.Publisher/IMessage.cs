namespace Sample.Hangfire.Publisher
{
    using System;


    public interface IMessage
    {
        Guid Id { get; }
        string Message { get; }
    }
}
