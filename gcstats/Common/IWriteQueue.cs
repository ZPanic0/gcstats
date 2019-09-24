using System.Threading.Tasks;

namespace gcstats.Common
{
    public interface IWriteQueue<T>
    {
        Task Start();
        void Enqueue(T request);
        void Close();
        bool Any();
    }
}
