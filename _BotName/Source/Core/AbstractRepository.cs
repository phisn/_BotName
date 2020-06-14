using System;
using System.Collections.Generic;
using System.Linq;
using _BotName.Source.Database;

namespace _BotName.Source.Core
{
    public class EntityNotExistsException : Exception
    {
    }
    
    public abstract class AbstractRepository<T> where T : new()
    {
        public abstract T FindById(ulong userId);

        public abstract T CreateForId(ulong userId);

        public virtual T FindOrCreateById(ulong userId)
        {
            var item = FindById(userId);
            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if (item == null)
            {
                item = CreateForId(userId);
            }
            return item;
        }
    }
}