using System.IO;

namespace WebServer.Managers
{
    public interface IConnectionManager
    {
        void ListenForBytesFromClient();
    }
}