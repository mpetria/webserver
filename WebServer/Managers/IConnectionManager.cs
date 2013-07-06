using System.IO;

namespace WebServer.Managers
{
    public interface IConnectionManager
    {
        void ProcessStream(Stream clientStream);
        void Close();
        string ConnectionId { get; }
    }
}