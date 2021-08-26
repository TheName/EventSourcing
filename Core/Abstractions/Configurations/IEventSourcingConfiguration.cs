namespace EventSourcing.Abstractions.Configurations
{
    public interface IEventSourcingConfiguration
    {
        string BoundedContext { get; }
    }
}