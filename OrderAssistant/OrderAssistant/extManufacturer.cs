using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderAssistant
{
    public partial class Manufacturer
    {
        /// <summary>
        /// Возвращает id по полному совпадению названия
        /// </summary>
        /// <param name="name">название</param>
        /// <returns>id</returns>
        public int GetId(string name)
        {
            var id = 0;
            using (var context = new orderAssistantEntities())
            {
                var result = (from s in context.manufacturers where s.name == name select s).FirstOrDefault();
                if (result != null)
                {
                    id = result.id;
                }
            }
            return id;
        }

        /// <summary>
        /// Возвращает производителя по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>manufacturer</returns>
        public manufacturer GetManufacturer(int id)
        {
            manufacturer manufacturer = null;
            using (var context = new orderAssistantEntities())
            {
                var result = (from s in context.manufacturers where s.id == id select s).FirstOrDefault();
                if (result != null)
                {
                    manufacturer = result;
                }
            }
            return manufacturer;
        }
    }
}
