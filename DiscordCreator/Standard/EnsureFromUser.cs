using System.Threading.Tasks;
using Discord;
using DiscordCreator.Interfaces;

namespace DiscordCreator.Standard
{
  internal class EnsureFromUser : IStandard<IMessage>
  {
    public Task<bool> CheckAsync(Context context, IMessage param)
    {
      var ok = context.GetAuthor().Id == param.Author.Id;
      return Task.FromResult(ok);
    }
  }
}