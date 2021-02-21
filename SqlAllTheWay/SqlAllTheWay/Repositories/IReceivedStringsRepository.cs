using System.Threading.Tasks;

namespace SqlAllTheWay.Repositories
{
    public interface IReceivedStringsRepository
    {
        Task Insert(string text);
    }
}