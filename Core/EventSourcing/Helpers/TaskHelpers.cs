using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourcing.Helpers
{
    internal static class TaskHelpers
    {
        /// <summary>
        /// Throws an aggregate exception instead of the first one
        /// </summary>
        public static async Task WhenAllWithAggregateException(IEnumerable<Task> tasks)
        {
            var whenAllTask = Task.WhenAll(tasks);

            try
            {
                await whenAllTask.ConfigureAwait(false);
            }
            catch
            {
                if (whenAllTask.Exception != null)
                {
                    // throw aggregate exception instead of the first one
                    throw whenAllTask.Exception;
                }

                throw;
            }
        }
    }
}