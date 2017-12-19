using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App4Answers.Models.A4Amodels
{
    public class ViewModelColumnDescriptor
    {

        public ViewModelColumnDescriptor(string underlyingName, string displayName)
        {
            UnderlyingName = underlyingName;
            DisplayName = displayName;
        }
        public String UnderlyingName { get; set; }

        public String DisplayName { get; set; }
    }
}
