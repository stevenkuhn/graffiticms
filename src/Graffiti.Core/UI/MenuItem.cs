using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Graffiti.Core
{
    public class MenuItem
    {
        private string _name;
        private string _command;
        private string _description;
        private string _commandMenu;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Command
        {
            get { return _command; }
            set { _command = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string CommandMenu
        {
            get { return _commandMenu; }
            set { _commandMenu = value; }
        }

        public MenuItem(string name, string command, string description, string commandMenu)
        {
            _name = name;
            _command = command;
            _description = description;
            _commandMenu = commandMenu;
        }

        public static List<MenuItem> GetMenuItems()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            Type[] types = assembly.GetTypes();

            List<MenuItem> menuItems = new List<MenuItem>();

            foreach (Type t in types)
            {
                if (t.IsClass)
                {
                    if (t.GetInterface("IMenuItem") != null)
                    {
                        object temp = Activator.CreateInstance(t);

                        menuItems.AddRange((List<MenuItem>)t.InvokeMember("GetMenuItems", 
                                                            BindingFlags.Default | BindingFlags.InvokeMethod,
                                                            null, temp, null));
                    }
                }
            }
            
            return menuItems;
        }
    }
}
