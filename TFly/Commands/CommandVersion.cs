using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TFly.Commands
{
    public class CommandVersion : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => ("v" + Assembly.GetExecutingAssembly().GetName().Name);
        public string Help => "Gets the version of the plugin";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "tfly.version" };


        public void Execute(IRocketPlayer caller, string[] command)
        {
            TFly.Logger.Log("#########################################");
            TFly.Logger.Log(string.Format("# Build Version: {0}", TFly.Version));
            TFly.Logger.Log(string.Format("# Build Date: {0}", TFly.BuildDate));
            TFly.Logger.Log("#########################################");
        }
    }
}
