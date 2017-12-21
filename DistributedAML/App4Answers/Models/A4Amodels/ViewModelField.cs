using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using As.Shared;

namespace App4Answers.Models.A4Amodels
{
    public class ViewModelField
    {
        private PropertyContainer pc;
        private ViewModelRow parent;
        public ViewModelField(ViewModelRow parent,PropertyContainer pc)
        {
            this.pc = pc;
            this.parent = parent;

        }

        public String Name => pc.pi.Name;

        public String DisplayName => pc.DisplayName();

        public bool IsEnum => pc.PropertyType.IsEnum;

        public IEnumerable<String> GetEnumChoices()
        {
            return pc.PropertyType.GetEnumNames();
        }


        public Object GetValue()
        {
            return pc.GetValue(parent.Parent);
        }

        public void SetValue(Object o)
        {
            pc.SetValue(parent.Parent, o);
        }

        public IEnumerable<String> GetForeignKeyValues()
        {
            return parent.Parent.GetForeignKeyValues(Name);
        }
    }
}
