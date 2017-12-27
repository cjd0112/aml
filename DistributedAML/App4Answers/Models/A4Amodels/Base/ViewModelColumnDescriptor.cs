using System;

namespace App4Answers.Models.A4Amodels.Base
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
