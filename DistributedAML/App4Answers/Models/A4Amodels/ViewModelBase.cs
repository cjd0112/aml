using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using As.Shared;
using Fasterflect;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;


namespace App4Answers.Models.A4Amodels
{
    public class ViewModelBase
    {
        public IEnumerable<String> GetColumnNames()
        {
            return typeContainer.Properties.Select(x => x.pi.Name);
        }

        public int GetFieldCount()
        {
            return typeContainer.Properties.Count();
        }

        private TypeContainer typeContainer;

        public CategoriesAndVerbs.Category Category;
        public CategoriesAndVerbs.Verb Verb;

        protected ViewModelBase(CategoriesAndVerbs.Category category,CategoriesAndVerbs.Verb verb)
        {
            Category = category;
            Verb = verb;
            typeContainer = TypeContainer.GetTypeContainer(GetType(),x => x.PropertyType == typeof(String) || !typeof(IEnumerable).IsAssignableFrom(x.PropertyType));
        }

        protected ViewModelBase(Object modelSource, CategoriesAndVerbs.Category category, CategoriesAndVerbs.Verb verb) :this(category,verb)
        {
            var tc = TypeContainer.GetTypeContainer(modelSource.GetType());
            foreach (var c in typeContainer.Properties)
            {
                var modelField = tc.GetProperty(c.Name);
                if (modelField == null)
                    throw new Exception(
                        $"Model field - {c.Name} - from ViewModel - {this.typeContainer.UnderlyingType.Name} - is not found on type of object - {modelSource.GetType()}");
                c.SetValue(this,modelField.GetValue(modelSource));
            }
        }

        protected ViewModelBase(IFormCollection form, CategoriesAndVerbs.Category category, CategoriesAndVerbs.Verb verb) : this(category,verb)
        {
            foreach (var c in typeContainer.Properties)
            {
                if (form.ContainsKey(c.Name))
                    c.SetValue(this, form[c.Name].ToString());
            }
        }

        public T ModelClassFromViewModel<T>()
        {
            var newType = TypeContainer.GetTypeContainer(typeof(T));
            T newOne = newType.CreateInstance<T>();

            foreach (var c in typeContainer.Properties)
            {
                newType.GetProperty(c.Name).SetValue(newOne,c.GetValue(this));
            }
            return newOne;
        }



        public IEnumerable<ViewModelRow> GetRows(int numColumns)
        {
            List<ViewModelRow> rows = new List<ViewModelRow>();
            int cnt = 0;
            foreach (var row in typeContainer.Properties.GroupBy(x => x.index / numColumns))
            {
                rows.Add(new ViewModelRow(this,cnt++,row.ToList()));
            }
            return rows;
        }

        public Object GetValue(string name)
        {
            return typeContainer.GetProperty(name).GetValue(this);
        }

        public void SetValue(string name, Object val)
        {
            typeContainer.GetProperty(name).SetValue(this, val);

        }


      
    }
}
