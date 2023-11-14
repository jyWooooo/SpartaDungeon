using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace source
{
    public abstract class Item
    {
        public int ID { get; protected set; }
        public string Name { get; protected set; }
        public int Price { get; protected set; }
        public string Descript { get; protected set; }
        public int Atk { get; protected set; }
        public int Def { get; protected set; }

        public Item()
        {

        }

        public Item(int id, string name, int price, int atk, int def, string desc)
        {
            ID = id;
            Name = name;
            Price = price;
            Descript = desc;
            Atk = atk;
            Def = def;
        }

        public Item(Item reference)
        {
            ID = reference.ID;
            Name = reference.Name;
            Price = reference.Price;
            Descript = reference.Descript;
            Atk = reference.Atk;
            Def = reference.Def;
        }

        public abstract void OnInsert(Character owner);
        public abstract void OnRemove(Character owner);
        public abstract void OnUsed(Character owner);
        public abstract string OnShowStatus();
        public abstract Item DeepCopy();

        public static Item JsonParse(JObject obj)
        {
            var id = (int)obj["ID"];
            var name = (string)obj["Name"];
            var price = (int)obj["Price"];
            var desc = (string)obj["Descript"];
            var atk = (int)obj["Atk"];
            var def = (int)obj["Def"];

            if (id < 10)
                return new Weapon(id, name, price, atk, desc);
            else
                return new Armor(id, name, price, def, desc);
        }
    }

    public class Weapon : Item 
    {
        public Weapon(int id, string name, int price, int atk, string desc)
        {
            ID = id;
            Name = name;
            Price = price;
            Atk = atk;
            Descript = desc;
        }

        public Weapon(Weapon reference)
        {
            ID = reference.ID;
            Name = reference.Name;
            Price = reference.Price;
            Atk = reference.Atk;
            Descript = reference.Descript;
        }

        public override void OnInsert(Character owner)
        {
        }

        public override void OnRemove(Character owner)
        {
            owner.UnEquipItem(this);
        }

        public override void OnUsed(Character owner)
        {
            Console.WriteLine("{0}을/를 장착했습니다.", Name);
            Console.ReadKey(true);
        }

        public override string OnShowStatus()
        {
            return string.Format("공격력 +{0, 2}", Atk);
        }

        public override Item DeepCopy()
        {
            return new Weapon(this);
        }
    }

    public class Armor : Item
    {
        public Armor(int id, string name, int price, int def, string desc)
        {
            ID = id;
            Name = name;
            Price = price;
            Def = def;
            Descript = desc;
        }

        public Armor(Armor reference)
        {
            ID = reference.ID;
            Name = reference.Name;
            Price = reference.Price;
            Def = reference.Def;
            Descript = reference.Descript;
        }

        public override void OnInsert(Character owner)
        {
        }

        public override void OnRemove(Character owner)
        {
            owner.UnEquipItem(this);
        }

        public override void OnUsed(Character owner)
        {
            Console.WriteLine("{0}을/를 장착했습니다.", Name);
            Console.ReadKey(true);
        }

        public override string OnShowStatus()
        {
            return string.Format("방어력 +{0, 2}", Def);
        }

        public override Item DeepCopy()
        {
            return new Armor(this);
        }
    }
}
