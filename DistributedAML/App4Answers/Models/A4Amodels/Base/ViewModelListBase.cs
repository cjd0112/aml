using System;
using System.Collections;
using System.Collections.Generic;
using As.Shared;

namespace App4Answers.Models.A4Amodels.Base
{
    public class ViewModelListBase : IEnumerable<ViewModelBase>,IViewModel
    {
        private IEnumerable<ViewModelBase> objs;
        private TypeContainer typeContainer;

        public ViewModelListBase(Type t,IEnumerable<ViewModelBase> objs)
        {
            this.typeContainer = TypeContainer.GetTypeContainer(t);
            this.objs = objs;
        }

        public IEnumerable<PropertyContainer> GetColumns()
        {
            return typeContainer.Properties;
        }

        public IEnumerator<ViewModelBase> GetEnumerator()
        {
            return objs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

     


        public ModelNames.ObjectTypes ObjectTypes { get; set; }
        public ModelNames.Verb Verb { get; set; }
        public ModelNames.ActionNames ActionNames { get; set; }
    }
}
