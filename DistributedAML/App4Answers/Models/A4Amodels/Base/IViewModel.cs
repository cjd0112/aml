using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App4Answers.Models.A4Amodels.Base
{
    public interface IViewModel
    {

        ModelNames.ObjectTypes  ObjectTypes { get; set; }

        ModelNames.Verb Verb { get; set; }

        ModelNames.ActionNames ActionNames { get; set; }
    }
}
