using System;
using System.Collections;
using System.Collections.Generic;
using As.Shared;

namespace App4Answers.Models.A4Amodels.Base
{
    public class ViewModelListBase : IEnumerable<ViewModelBase>
    {
        private IEnumerable<ViewModelBase> objs;
        private TypeContainer typeContainer;
        public ModelNames.AdministrationNames AdministrationNames;
        public ModelNames.Verb Verb;

        public ViewModelListBase(Type t,IEnumerable<ViewModelBase> objs,ModelNames.AdministrationNames administrationNames,ModelNames.Verb verb)
        {
            this.typeContainer = TypeContainer.GetTypeContainer(t);
            this.objs = objs;
            AdministrationNames = administrationNames;
            Verb = verb;
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
    }
}
