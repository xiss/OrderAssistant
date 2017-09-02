using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderAssistant
{
    public partial class  Brend
    {
        /// <summary>
        /// Возвращет id по полному совпадению названия
        /// </summary>
        /// <param name="name">Название бренда</param>
        /// <returns>id</returns>
        public int GetId(string name)
        {
            var id = 0;
            using (var context = new orderAssistantEntities())
            {
                var result = (from s in context.brends where s.name == name select s).FirstOrDefault();
                if (result != null)
                {
                    id = result.id;
                }
            }
            return id;
        }

        /// <summary>
        /// Возвращет бренд по id
        /// </summary>
        /// <param name="id">id бренда</param>
        /// <returns>Бренд</returns>
        public brend GetBrend(int id)
        {
            brend brend = null;
            using (var context = new orderAssistantEntities())
            {
                var result = (from s in context.brends where s.id == id select s).FirstOrDefault();
                if (result != null)
                {
                    brend = result;
                }
            }
            return brend;
        }
    }
}
