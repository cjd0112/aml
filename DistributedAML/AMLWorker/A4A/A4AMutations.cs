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
        private A4ARepositoryGraphDb db;

        public A4AMutations(A4ARepositoryGraphDb db)
        {
            this.db = db;
        }

        public A4AMessage AddMessage(A4AMessageSetter msg)
        {
            return db.AddMessage(msg);
        }
    }
}