using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLib
{
    public interface IErrorPublisher
    {
        void PublishError(Exception ex);
        void PublishError(string msg);
    }
}
