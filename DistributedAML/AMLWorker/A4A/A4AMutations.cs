using System;

namespace AMLWorker.A4A
{
    public class A4AMessageSetter
    {
        public string Id { get; set; }
        public string Message { get; set; }
    }

    public class A4AMutations 
    {
        public A4AMessage AddMessage(A4AMessageSetter msg)
        {
            throw new NotImplementedException();
        }
    }
}