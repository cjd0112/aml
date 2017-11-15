using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Logger;

namespace AmlClient.Tasks
{
    public class MyTask<T> : Task<T>
    {
        public int Bucket;
        public Object State;
        public MyTask(String actionName, int bucket, Func<T> a, Object state = null) : base(() =>
        {
            try
            {
                L.Trace($"Running {actionName}");
                return a();
            }
            catch (Exception e)
            {
                L.Trace($"An exception encountered running {actionName} ...");
                L.Trace(e.Message);
                return default(T);
            }
        })
        {
            Bucket = bucket;
            State = state;
        }
    }

}
