using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderAssistant
{
    public partial class Item
    {
        /// <summary>
        /// Возвращает id по коду 1С
        /// </summary>
        /// <param name="id1C">Код 1С</param>
        /// <returns>Код 1С либо 0 если записи нет</returns>
        int GetId(string id1C)
        {
            var id = 0;
            using (var context = new orderAssistantEntities())
            {
                var result = (from s in context.items where s.id1C == id1C select s).FirstOrDefault();
                if (result != null)
                {
                    id = result.id;
                }
            }
            return id;
        }

        /// <summary>
        /// Возвращает Item по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>item</returns>
        item GetItem(int id)
        {
            item item = null;
            using (var context = new orderAssistantEntities())
            {
                var result = (from s in context.items where s.id == id select s).FirstOrDefault();
                if (result != null)
                {
                    item = result;
                }
            }
            return item;
        }
    }
}
