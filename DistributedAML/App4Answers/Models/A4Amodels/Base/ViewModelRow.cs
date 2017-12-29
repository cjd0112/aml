using System.Collections.Generic;
using As.Shared;

namespace App4Answers.Models.A4Amodels.Base
{
    public class ViewModelRow
    {
        public int Row { get; set; }
        public ViewModelRow(ViewModelBase parent,int cnt,List<PropertyContainer> properties)
        {
            Row = cnt;

            Fields = new List<ViewModelField>();
            foreach (var c in properties)
            {
                Fields.Add(new ViewModelField(this, c));
            }

            Parent = parent;
        }

        public ViewModelBase Parent;

        public List<ViewModelField> Fields { get; set; }
    }
}
