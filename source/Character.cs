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
    public class Character
    {
        int _atk;
        int _def;
        public int Exp { get; private set; }
        public string Name { get; private set; }
        public string Job { get; private set; }
        public int Level { get; private set; }
        public int Atk
        {
            get
            {
                if (equipWeapon != null)
                    return _atk + equipWeapon.Atk;
                else return _atk;
            }
            private set
            {
                _atk = value;
            }
        }
        public int Def
        {
            get
            {
                if (equipArmor != null) 
                    return _def + equipArmor.Def;
                else return _def;

            }
            private set
            {
                _def = value;
            }
        }
        public int HP { get; private set; }
        public int Gold { get; private set; }
        public Inventory inventory { get; }
        public Weapon equipWeapon { get; private set; }
        public Armor equipArmor { get; private set; }

        public Character()
        {

        }

        public Character(string name, string job, int level, int atk, int def, int hp, int gold, int exp)
        {
            equipWeapon = null;
            equipArmor = null;
            Name = name;
            Job = job;
            Level = level;
            _atk = atk;
            _def = def;
            Exp = exp;
            HP = hp;
            Gold = gold;
            inventory = new Inventory(this);
        }

        public Character(string name, string job, int level, int atk, int def, int hp, int gold, int exp, Weapon weapon, Armor armor)
        {
            equipWeapon = weapon;
            equipArmor = armor;
            Name = name;
            Job = job;
            Level = level;
            _atk = atk;
            _def = def;
            Exp = exp;
            HP = hp;
            Gold = gold;
            inventory = new Inventory(this);
            inventory.InsertItem(weapon);
            inventory.InsertItem(armor);
        }

        public void ChangeName(string name)
        {
            Name = name;
        }

        public void EquipItem(Item item)
        {
            if (item as Weapon != null)
            {
                equipWeapon = item as Weapon;
                item.OnUsed(this);
            }
            if (item as Armor != null)
            {
                equipArmor = item as Armor;
                item.OnUsed(this);
            }
        }

        public void UnEquipItem(Item item)
        {
            if (equipWeapon == item)
                equipWeapon = null;
            if (equipArmor == item)
                equipArmor = null;
        }

        public bool EquipItem(int idx)
        {
            var item = inventory.GetItem(idx);
            if (item != null)
            {
                EquipItem(item);
                return true;
            }
            return false;
        }

        public string SaleItem(int idx)
        {
            var item = inventory.GetItem(idx);
            if (item != null)
            {
                inventory.RemoveItem(item);
                Gold += (int)(item.Price * 0.85f);
                return item.Name;
            }
            return null;
        }

        public void BuyItem(Item item)
        {
            inventory.InsertItem(item);
            Gold -= item.Price;
        }

        public void HitDamage(int damage)
        {
            HP -= damage-Def;
            if (HP < 0)
                HP = 0;
            else if (HP > 100)
                HP = 100;
        }

        public void Heal(int heal, int useGold)
        {
            HP += heal;
            if (HP > 100)
                HP = 100;
            Gold -= useGold;
        }

        public bool ExpUp()
        {
            Exp++;
            if (Exp == Level)
            {
                Level++;
                _atk += 2;
                _def += 2;
                Exp = 0;
                return true;
            }
            return false;
        }

        public JObject ToJObject()
        {
            JObject res = new JObject
            {
                { "Name", Name },
                { "Job", Job },
                { "Level", Level },
                { "Atk", _atk },
                { "Def", _def },
                { "HP", HP },
                { "Gold", Gold },
                { "Exp", Exp },
                { "equipWeapon", equipWeapon?.ID },
                { "equipArmor", equipArmor?.ID },
                { "Inventory", inventory.ToJArray() }
            };
            return res;
        }

        public void SavePlayerData()
        {
            var jsonStr = JsonConvert.SerializeObject(ToJObject());
            //File.WriteAllText(@"PlayerData.json", jsonStr.ToString());
            File.WriteAllText(@"PlayerData.json", AESManager.Encrypt(jsonStr.ToString()));
        }

        public static Character LoadPlayerData()
        {
            Character res = null;
            try
            {
                //var josnStr = File.ReadAllText(@"PlayerData.json");
                var josnStr = AESManager.Decrypt(File.ReadAllText(@"PlayerData.json"));
                var jobject = JsonConvert.DeserializeObject<JObject>(josnStr);
                res = new Character((string)jobject["Name"], (string)jobject["Job"], (int)jobject["Level"], (int)jobject["Atk"],
                                              (int)jobject["Def"], (int)jobject["HP"], (int)jobject["Gold"], (int)jobject["Exp"]);
                foreach (var e in jobject["Inventory"])
                {
                    if (Shop.instance.ContainsKey((int)e))
                    {
                        Item n = Shop.instance[(int)e].DeepCopy();
                        res.inventory.InsertItem(n);
                        if (jobject["equipArmor"].HasValues)
                            if ((int)jobject["equipWeapon"] == (int)e)
                                res.equipWeapon = n as Weapon;
                        if (jobject["equipArmor"].HasValues)
                            if ((int)jobject["equipArmor"] == (int)e)
                                res.equipArmor = n as Armor;
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
            }
            if (res == null)
                res = MakeBasicCharacterFile();
            return res;
        }

        public static Character MakeBasicCharacterFile()
        {
            Character res = new Character("", "전사", 1, 10, 5, 100, 1500, 0);
            res.SavePlayerData();
            return res;
        }
    }
}
