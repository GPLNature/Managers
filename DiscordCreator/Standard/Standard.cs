﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordCreator.Interfaces;

namespace DiscordCreator.Standard
{
  internal class Standard<T> : IStandard<T>
  {
    private readonly List<IStandard<T>> _standards = new();

    public async Task<bool> CheckAsync(Context context, T param)
    {
      for (var i = 0; i < _standards.Count; i++)
      {
        var result = await _standards[i].CheckAsync(context, param).ConfigureAwait(false);
        if (!result) return false;
      }

      return true;
    }

    public Standard<T> AddCriterion(IStandard<T> criterion)
    {
      _standards.Add(criterion);
      return this;
    }
  }
}