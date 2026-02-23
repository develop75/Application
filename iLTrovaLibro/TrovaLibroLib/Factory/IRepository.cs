using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrovaLibroLib.Factory
{
    public interface IRepository
    {
        List<T> GetAll<T>() where T : class;

        T? GetById<T>(long id) where T : class;

        T Create<T>(T item) where T : class;

        T Update<T>(T item) where T : class;

        void Delete<T>(long id) where T : class;

        List<T> Find<T>(Func<T, bool> predicate) where T : class;
    }
}
