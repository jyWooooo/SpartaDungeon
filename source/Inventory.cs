using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace source
{
    public class Inventory
    {
        List<Item> items = new List<Item>();
        public bool viewType = true;
        public int MaxPad { get; private set; }

        public Character parent { get; private set; }

        public Inventory(Character _parent)
        {
            parent = _parent;
            MaxPad = 4;
        }

        public void InsertItem(Item item)
        {
            items.Add(item);
            item.OnInsert(parent);
            MaxPad = Math.Max(MaxPad, Encoding.Default.GetBytes(item.Name).Length);
        }

        public void RemoveItem(Item item)
        {
            items.Remove(item);
            item.OnRemove(parent);
        }

        public Item GetItem(int idx)
        {
            if (idx < items.Count)
                return items[idx];
            else return null;
        }

        public void ShowItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                int pad = MaxPad - Encoding.Default.GetBytes(items[i].Name).Length;
                if (viewType)
                {
                    if (items[i] == parent.equipWeapon || items[i] == parent.equipArmor)
                        Console.WriteLine("[E]{0, 2} | {1} | {2, -7} | {3}", i + 1, items[i].Name + "".PadLeft(pad), items[i].OnShowStatus(), items[i].Descript);
                    else                                                                     
                        Console.WriteLine(" - {0, 2} | {1} | {2, -7} | {3}", i + 1, items[i].Name + "".PadLeft(pad), items[i].OnShowStatus(), items[i].Descript);
                }
                else
                {
                    if (items[i] == parent.equipWeapon || items[i] == parent.equipArmor)
                        Console.WriteLine("[E]{0, 2} | {1} | {2, -7} | {3, 8:#,##0} G", i + 1, items[i].Name + "".PadLeft(pad), items[i].OnShowStatus(), items[i].Price);
                    else                                   
                        Console.WriteLine(" - {0, 2} | {1} | {2, -7} | {3, 8:#,##0} G", i + 1, items[i].Name + "".PadLeft(pad), items[i].OnShowStatus(), items[i].Price);
                }
            }
        }

        public void ShowSaleList()
        {
            for (int i = 0; i < items.Count; i++)
            {
                int pad = MaxPad - Encoding.Default.GetBytes(items[i].Name).Length;
                if (items[i] == parent.equipWeapon || items[i] == parent.equipArmor)
                    Console.WriteLine("[E]{0, 2} | {1} | {2, -7} | {3, 6:#,##0}", i + 1, items[i].Name + "".PadLeft(pad), items[i].OnShowStatus(), items[i].Price);
                else
                    Console.WriteLine(" - {0, 2} | {1} | {2, -7} | {3, 6:#,##0}", i + 1, items[i].Name + "".PadLeft(pad), items[i].OnShowStatus(), items[i].Price);
            }
        }

        public bool HasSameItem(Item item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == item.ID)
                    return true;
            }
            return false;
        }

        public void OrderBy(OrderType orderType)
        {
            switch (orderType)
            {
                case OrderType.Default:
                    items = items.OrderBy(element => element.ID).ToList();
                    break;
                case OrderType.NameAsc:
                    items = items.OrderBy(element => element.Name).ToList();
                    break;
                case OrderType.NameDesc:
                    items = items.OrderByDescending(element => element.Name).ToList();
                    break;
                case OrderType.AtkDesc:
                    items = items.OrderByDescending(element => (element as Weapon) != null ? (element as Weapon).Atk : -element.ID).ToList();
                    break;
                case OrderType.DefDesc:
                    items = items.OrderByDescending(element => (element as Armor) != null ? (element as Armor).Def : -element.ID).ToList();
                    break;
                case OrderType.PriceAsc:
                    items = items.OrderBy(element => element.Price).ToList();
                    break;
                case OrderType.PriceDesc:
                    items = items.OrderByDescending(element => element.Price).ToList();
                    break;
            }
        }
        
        public JArray ToJArray()
        {
            JArray res = new JArray(items.Select(x => x.ID).ToList());
            return res;
        }
    }

    public enum OrderType
    {
        Default,
        NameAsc,
        NameDesc,
        AtkDesc,
        DefDesc,
        PriceAsc,
        PriceDesc,
    }
}