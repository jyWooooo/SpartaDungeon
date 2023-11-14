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
            MaxPad = Math.Max(MaxPad, 4);
            foreach (var item in instance)
                MaxPad = Math.Max(MaxPad, Encoding.Default.GetBytes(item.Value.Name).Length);
        }

        public static void MakeItemDictionaryJsonFile()
        {
            var jsonStr = JsonConvert.SerializeObject(instance);
            //File.WriteAllText(@"ItemDictionary.json", jsonStr.ToString());
            File.WriteAllText(@"ItemDictionary.json", AESManager.Encrypt(jsonStr.ToString()));
        }

        public static void InitItemDictionary()
        {
            try
            {
                //var jsonStr = File.ReadAllText(@"ItemDictionary.json");
                var jsonStr = AESManager.Decrypt(File.ReadAllText(@"ItemDictionary.json"));
                var dict = JsonConvert.DeserializeObject<Dictionary<int, JObject>>(jsonStr);
                foreach (var e in dict)
                    instance.Add(e.Key, Item.JsonParse(e.Value));
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
                MakeNewItemDictionaryFile();
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
                MakeNewItemDictionaryFile();
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
                MakeNewItemDictionaryFile();
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
                MakeNewItemDictionaryFile();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
                MakeNewItemDictionaryFile();
            }
        }

        static void MakeNewItemDictionaryFile()
        {
            instance = new Dictionary<int, Item>();
            instance.Add(0, new Weapon(0, "녹슨 검", 500, 1, "더없이 평범해서 뭐라 설명하기도 뭐한 검."));
            instance.Add(1, new Weapon(1, "장검", 2000, 7, "적의 틈새를 노리기 가장 좋은 검. 길고 날카로워 단숨에 베어낼 수 있다."));
            instance.Add(2, new Weapon(2, "보검", 7000, 11, "존재 자체로 칭송받는 검. 영웅이 사용하였거나 귀한 재료로 만들어진 경우가 많다."));
            instance.Add(3, new Weapon(3, "아론다이트", 20000, 27, "란슬롯 경의 검으로, 화룡을 베어 카보넥의 일레인 공주를 구했다고 전해진다."));
            instance.Add(4, new Weapon(4, "레바테인", 200000, 73, "로키가 룬을 새겨 만든 검이다."));
            instance.Add(10, new Armor(10, "천 갑옷", 500, 1, "천으로 만든 갑옷.이름만 봐도 별로 튼튼해보이지 않는다."));
            instance.Add(11, new Armor(11, "가죽 갑옷", 1800, 3, "가죽으로 만들어 천 갑옷보다 튼튼하다."));
            instance.Add(12, new Armor(12, "사슬 갑옷", 6300, 7, "아시아와 유럽에서 오랜 시간 사용 된 대표적인 금속 갑옷."));
            instance.Add(13, new Armor(13, "성기사의 갑옷", 13500, 12, "이걸 착용하면 언데드에 특화된 방어력을 얻을 수 있다."));
            instance.Add(14, new Armor(14, "미스릴 갑옷", 150000, 39, "가볍지만 그 강도는 상상을 초월한다."));
            MakeItemDictionaryJsonFile();
        }
    }
}
