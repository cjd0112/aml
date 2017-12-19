using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using As.Shared;

namespace App4Answers.Models.A4Amodels
{
    public class ViewModelListBase : IEnumerable<ViewModelBase>
    {
        private IEnumerable<ViewModelBase> objs;
        private TypeContainer typeContainer;
        public CategoriesAndVerbs.Category Category;
        public CategoriesAndVerbs.Verb Verb;

        public ViewModelListBase(Type t,IEnumerable<ViewModelBase> objs,CategoriesAndVerbs.Category category,CategoriesAndVerbs.Verb verb)
        {
            this.typeContainer = TypeContainer.GetTypeContainer(t);
            this.objs = objs;
            Category = category;
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
