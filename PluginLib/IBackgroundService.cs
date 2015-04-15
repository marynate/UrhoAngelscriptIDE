using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLib
{
    /// <summary>
    /// Background services will be instantiated at startup
    /// 
    /// They must set themselves up in the constructor
    /// </summary>
    public interface IBackgroundService
    {
    }
}
