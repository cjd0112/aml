using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using App4Answers.Models.A4Amodels;
using As.Shared;
using Google.Protobuf;
using Microsoft.AspNetCore.Builder;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace App4Answers.Extensions
{
    public static class TypeContainerExtensions
    {
        public static IApplicationBuilder UseTypeContainer(this IApplicationBuilder builder)
        {
            foreach (var tc in TypeContainer.Initialize(Assembly.GetAssembly(typeof(A4AModel1)).GetTypes().Where(x=>x.Name.StartsWith("A4A") && x.Name.EndsWith("ViewModel"))))
            {
                tc.
                    FirstPropertyIsPrimaryKey().
                    SetDelegateForCreateInstance().
                    AddProperties(x=>x.PropertyType == typeof(String) || !typeof(IEnumerable).IsAssignableFrom(x.PropertyType));
            }


            foreach (var tc in TypeContainer.Initialize(Assembly.GetAssembly(typeof(A4ACategory)).GetTypes()
                .Where(predicate: x => x.Name.StartsWith("A4A") && x.IsClass && typeof(IMessage).IsAssignableFrom(x))))
            {
                
                    tc
                    .FirstPropertyIsPrimaryKey()
                    .SetDelegateForCreateInstance()
                    .AddProperties(x =>
                        x.PropertyType == typeof(String) || !typeof(IEnumerable).IsAssignableFrom(x.PropertyType));
            }
            return builder;
        }
    }
}
