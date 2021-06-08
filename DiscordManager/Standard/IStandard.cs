using System.Threading.Tasks;
using DiscordCreator.Interfaces;

namespace DiscordCreator.Standard
{
  public interface IStandard<in T>
  {
    Task<bool> CheckAsync(Context context, T param);
  }
}