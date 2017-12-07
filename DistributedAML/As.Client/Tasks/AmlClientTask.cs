using System;
using System.Threading.Tasks;
using As.Logger;

namespace As.Client.Tasks
{
    public class AmlClientTask<T> : Task<T>
    {
        public int Bucket;
        public Object State;
        public AmlClientTask(String actionName, int bucket, Func<T> a, Object state = null) : base(() =>
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
