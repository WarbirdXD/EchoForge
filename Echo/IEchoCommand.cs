namespace EchoForge.Echo
{
    public interface IEchoCommand
    {
        float TimeStamp { get; }
        void Execute(EchoContext context);
    }
}
