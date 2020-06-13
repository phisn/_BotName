using System;
using System.Collections.Generic;

namespace _BotName.Source.Core
{
    public class EntityNotExistsException : Exception
    {
    }
    
    public abstract class AbstractVolatileRepository<T> where T : new()
    {
        protected Dictionary<ulong, T> Data;
        

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

        public virtual T FindById(ulong userId)
        {
            Data.TryGetValue(userId, out var item);
            return item;
        }
        
        public virtual T CreateForId(ulong userId)
        {
            if (Data.TryGetValue(userId, out var casinoUser)) {
                throw new EntityNotExistsException();
            }
            casinoUser = new T();
            Data.Add(userId, casinoUser);
            return casinoUser;
        }
    }
}