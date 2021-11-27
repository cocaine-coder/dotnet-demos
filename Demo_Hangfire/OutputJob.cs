using Hangfire.Annotations;
using Hangfire.Common;
using System.Reflection;

namespace Demo_Hangfire
{
    public class OutputJob : Job
    {
        public OutputJob([NotNull] MethodInfo method) : base(method)
        {
        }
    }
}
