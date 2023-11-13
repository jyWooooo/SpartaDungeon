using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace source
{
    public class Shop
    {
        public static Dictionary<int, Item> instance = new Dictionary<int, Item>();
        public static int MaxPad { get; private set; }
        public static List<Item> ShowBuyShop(Inventory inventory)
        {
            int i = 1;
            List<Item> res = new List<Item>();
            foreach (var item in instance)
            {
                    int pad = MaxPad - Encoding.Default.GetBytes(item.Value.Name).Length;
                if (inventory.HasSameItem(item.Value))
                    Console.WriteLine(" - {0, 2} | {1} | {2, -7} | {3, 7}", i, item.Value.Name + "".PadLeft(pad), item.Value.OnShowStatus(), "보유중");
                else
                    Console.WriteLine(" - {0, 2} | {1} | {2, -7} | {3, 8 : #,###} G", i, item.Value.Name + "".PadLeft(pad), item.Value.OnShowStatus(), item.Value.Price);
                res.Add(item.Value);
                i++;
            }
            return res;
        }
        public static void SetMaxPad()
        {
            foreach (var item in instance)
                MaxPad = Math.Max(MaxPad, Encoding.Default.GetBytes(item.Value.Name).Length);
        }

        public static void MakeItemDictionaryJsonFile()
        {
            var jsonStr = JsonConvert.SerializeObject(instance);
            File.WriteAllText(@"ItemDictionary.json", AESManager.Encrypt(jsonStr.ToString()));
        }

        public static void InitItemDictionary()
        {
            var jsonStr = AESManager.Decrypt(File.ReadAllText(@"ItemDictionary.json"));
            var dict = JsonConvert.DeserializeObject<Dictionary<int, JObject>>(jsonStr);
            foreach (var e in dict)
                instance.Add(e.Key, Item.JsonParse(e.Value));
        }
    }
}
