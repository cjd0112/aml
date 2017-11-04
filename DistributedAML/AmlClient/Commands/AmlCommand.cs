using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using StructureMap;

namespace AmlClient.Commands
{
    public abstract class AmlCommand
    {
        public static Type GetAmlCommand(String s)
        {
            return GetCommands().FirstOrDefault(x => x.Name == s);
        }

        public static void RunCommand(Container c,Type t)
        {
            AmlCommand command = c.GetInstance(t) as AmlCommand;
            command.Run();
        }

        public static IEnumerable<Type> GetCommands()
        {
            return Assembly.GetAssembly(typeof(AmlCommand)).GetTypes()
                .Where(x => x.IsSubclassOf(typeof(AmlCommand)));
        }

        public abstract void Run();
    }
}
