using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebServer.Managers
{
    public interface IDataManager
    {
        void ManageBytes(byte[] bytes);
        void ManageStream(Stream stream);

        void Close();
        void SetLinkedDataManager(IDataManager dataManager);
    }
}
