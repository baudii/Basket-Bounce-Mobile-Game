using System.Threading.Tasks;

namespace BasketBounce.Systems
{
    public interface IInitializableAsync : IInitializable
    {
		new Task Init();
    }
}
