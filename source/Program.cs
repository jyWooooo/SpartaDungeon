using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace source
{
    class Program
    {
        public static Character player;

        public enum Scene
        {
            TitleScene,
            LobbyScene,
            MyInfo,
            Inventory,
            Shop,
            Dungeon,
        }
        static Scene currentScene = Scene.TitleScene;

        public struct DungeonInfo
        {
            public int ID { get; }
            public string Name { get; }
            public int Level { get; }
            public int NeedDef { get; }
            public int ClearGold { get; }
            public string[] Monsters { get; }
            public int HP { get; }
            public static int MaxPad { get; private set; }

            public DungeonInfo(int id, string name, int level, int needDef, int clearGold, string[] monsters, int hp)
            {
                ID = id;
                Name = name;
                Level = level;
                NeedDef = needDef;
                ClearGold = clearGold;
                Monsters = monsters;
                HP = hp;
            }

            public void SetMaxPad()
            {
                MaxPad = Math.Max(MaxPad, Encoding.Default.GetBytes(Name).Length);
            }
        }
        static DungeonInfo[] dungeons;

        static void Main(string[] args)
        {
            InitGame();
            int step = 0;
            while (true)
            {
                switch (currentScene)
                {
                    case Scene.TitleScene:
                        DrawTitleScene();
                        break;
                    case Scene.LobbyScene:
                        DrawLobbyScene();
                        break;
                    case Scene.MyInfo:
                        DrawMyInfo();
                        break;
                    case Scene.Inventory:
                        DrawInventory(ref step);
                        break;
                    case Scene.Shop:
                        DrawShop(ref step);
                        break;
                    case Scene.Dungeon:
                        DrawDungeon(ref step);
                        break;
                }
            }
        }

        static void InitGame()
        {
            Shop.InitItemDictionary();
            Shop.SetMaxPad();

            player = Character.LoadPlayerData();

            dungeons = new DungeonInfo[3];
            dungeons[0] = new DungeonInfo(0, "엘리니아 남쪽 숲", 1, 10, 1000, new string[] { "파란달팽이", "슬라임", "초록버섯" }, 30);
            dungeons[1] = new DungeonInfo(1, "골렘의 사원", 2, 30, 1700, new string[] { "스톤골렘", "다크스톤골렘", "믹스골렘" }, 100);
            dungeons[2] = new DungeonInfo(2, "망가진 용의 둥지", 3, 50, 2500, new string[] { "다크와이번", "네스트골렘", "스켈레곤" }, 800);
            foreach (var d in dungeons)
                d.SetMaxPad();
        }

        static ConsoleKey InputKey()
        {
            var res = Console.ReadKey(true).Key;
            if (res == ConsoleKey.NumPad0)
                res = ConsoleKey.D0;
            if (res == ConsoleKey.NumPad1)
                res = ConsoleKey.D1;
            if (res == ConsoleKey.NumPad2)
                res = ConsoleKey.D2;
            if (res == ConsoleKey.NumPad3)
                res = ConsoleKey.D3;
            if (res == ConsoleKey.NumPad4)
                res = ConsoleKey.D4;
            if (res == ConsoleKey.NumPad5)
                res = ConsoleKey.D5;
            if (res == ConsoleKey.NumPad6)
                res = ConsoleKey.D6;
            if (res == ConsoleKey.NumPad7)
                res = ConsoleKey.D7;
            if (res == ConsoleKey.NumPad8)
                res = ConsoleKey.D8;
            if (res == ConsoleKey.NumPad9)
                res = ConsoleKey.D9;
            return res;
        }

        static void DrawTitleScene()
        {
            Console.Clear();
            Console.WriteLine("======================================================================================");
            Console.WriteLine("=                                                                                    =");
            Console.WriteLine("=                                                                                    =");
            Console.WriteLine("=                                                                                    =");
            Console.WriteLine("=                                                                                    =");
            Console.WriteLine("=                                   Sparta Dungeon                                   =");
            Console.WriteLine("=                                                                                    =");
            Console.WriteLine("=                                                                                    =");
            Console.WriteLine("=                                                                                    =");
            Console.WriteLine("=                                                                                    =");
            Console.WriteLine("======================================================================================");
            Console.WriteLine("                                아무 키나 눌러서 시작                                 ");
            InputKey();
            if (player.Name == "")
            {
                string str;
                do
                {
                    Console.Clear();
                    Console.WriteLine("당신의 이름을 정해주세요 (최대 20글자)");
                    Console.Write(">> ");
                    str = Console.ReadLine();
                }
                while (!(0 < Encoding.Default.GetBytes(str).Length && Encoding.Default.GetBytes(str).Length <= 20));
                player.ChangeName(str);
                player.SavePlayerData();
            }
            currentScene = Scene.LobbyScene;
        }

        static void DrawLobbyScene()
        {
            Console.Clear();
            Console.WriteLine("마을에 도착했습니다...");
            Console.WriteLine();
            Console.WriteLine("1. 여관 (내 상태)");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            var input = InputKey();
            if (input == ConsoleKey.D1)
                currentScene = Scene.MyInfo;
            else if (input == ConsoleKey.D2)
                currentScene = Scene.Inventory;
            else if (input == ConsoleKey.D3)
                currentScene = Scene.Shop;
            else if (input == ConsoleKey.D4)
                currentScene = Scene.Dungeon;
        }

        static void DrawMyInfo()
        {
            Console.Clear();
            Console.WriteLine("여관");
            Console.WriteLine("내 상태를 보거나 휴식을 취합니다.");
            Console.WriteLine();
            Console.WriteLine($"{player.Name} ({player.Job})");
            Console.WriteLine("레  벨 : {0, 6 : #,##0}", player.Level);
            Console.WriteLine("공격력 : {0, 6 : #,##0}", player.Atk);
            Console.WriteLine("방어력 : {0, 6 : #,##0}", player.Def);
            Console.WriteLine("체  력 : {0, 6 : #,##0}", player.HP);
            Console.WriteLine("  돈   : {0, 6 : #,##0}", player.Gold);
            Console.WriteLine();
            if (player.equipWeapon != null)
                Console.WriteLine($"무  기 : {player.equipWeapon.Name}");
            else
                Console.WriteLine($"무  기 : 없  음");
            if (player.equipArmor != null)
                Console.WriteLine($"방어구 : {player.equipArmor.Name}");
            else
                Console.WriteLine($"방어구 : 없  음");
            Console.WriteLine();
            Console.WriteLine("1. 휴식하기 (500 G)");
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            var input = InputKey();
            if (input == ConsoleKey.D1)
            {
                if (player.HP >= 100)
                    Console.WriteLine("이미 최대 체력입니다.");
                else
                {
                    Console.WriteLine("500 G를 지불하고 체력을 회복했습니다.");
                    player.Heal(100, 500);
                    player.SavePlayerData();
                }
                Console.ReadKey(true);
            }
            else if (input == ConsoleKey.D0)
                currentScene = Scene.LobbyScene;
        }

        static void DrawInventory(ref int step)
        {
            Console.Clear();
            Console.WriteLine("인벤토리");
            Console.WriteLine();
            if (player.inventory.viewType)
                Console.WriteLine(" {0} | {1} | {2, -8} | {3}", "번호", "이름" + "".PadLeft(player.inventory.MaxPad - 4), "정보", "설명");
            else
                Console.WriteLine(" {0} | {1} | {2, -8} | {3, 8}", "번호", "이름" + "".PadLeft(player.inventory.MaxPad - 4), "정보", "가격");
            player.inventory.ShowItems();
            Console.WriteLine();
            switch (step)
            {
                case 0:
                    Console.WriteLine("1. 아이템 장착");
                    Console.WriteLine("2. 아이템 정렬");
                    if (player.inventory.viewType)
                        Console.WriteLine("3. 가격 보기");
                    else
                        Console.WriteLine("3. 설명 보기");
                    Console.WriteLine("0. 나가기");
                    Console.WriteLine();
                    Console.WriteLine("원하시는 행동을 입력해주세요.");
                    var input = InputKey();
                    if (input == ConsoleKey.D0)
                        currentScene = Scene.LobbyScene;
                    else if (input == ConsoleKey.D1)
                        step = 1;
                    else if (input == ConsoleKey.D2)
                        step = 2;
                    else if (input == ConsoleKey.D3)
                        player.inventory.viewType = !player.inventory.viewType;
                    break;
                case 1:
                    Console.WriteLine("아이템의 번호로 장착할 아이템을 지정합니다.");
                    Console.WriteLine("0. 나가기");
                    Console.WriteLine();
                    Console.WriteLine("장착할 아이템의 번호를 입력해주세요.");
                    var input2 = Console.ReadLine();
                    if (int.TryParse(input2, out int res))
                    {
                        if (0 < res && res <= 10)
                            player.EquipItem(res);
                        player.SavePlayerData();
                    }
                    if (res == 0)
                        step = 0;
                    break;
                case 2:
                    Console.WriteLine("1. 기본값");
                    Console.WriteLine("2. 이름 오름차순");
                    Console.WriteLine("3. 이름 내림차순");
                    Console.WriteLine("4. 공격력 높은 순");
                    Console.WriteLine("5. 방어력 높은 순");
                    Console.WriteLine("6. 가격 낮은 순");
                    Console.WriteLine("7. 가격 높은 순");
                    Console.WriteLine("0. 나가기");
                    Console.WriteLine();
                    Console.WriteLine("원하시는 행동을 입력해주세요.");
                    input = InputKey();
                    if (input == ConsoleKey.D1)
                        player.inventory.OrderBy((OrderType)0);
                    else if (input == ConsoleKey.D2)
                        player.inventory.OrderBy((OrderType)1);
                    else if (input == ConsoleKey.D3)
                        player.inventory.OrderBy((OrderType)2);
                    else if (input == ConsoleKey.D4)
                        player.inventory.OrderBy((OrderType)3);
                    else if (input == ConsoleKey.D5)
                        player.inventory.OrderBy((OrderType)4);
                    else if (input == ConsoleKey.D6)
                        player.inventory.OrderBy((OrderType)5);
                    else if (input == ConsoleKey.D7)
                        player.inventory.OrderBy((OrderType)6);
                    else if (input == ConsoleKey.D0)
                        step = 0;
                    player.SavePlayerData();
                    break;
            }

        }

        static void DrawShop(ref int step)
        {
            Console.Clear();
            Console.WriteLine("상  점");
            Console.WriteLine("돈 : {0:#,##0}", player.Gold);
            Console.WriteLine();
            Console.WriteLine(" {0} | {1} | {2, -8} | {3, 8}", "번호", "이름" + "".PadLeft(Shop.MaxPad - 4), "정보", "가격");
            var shopList = Shop.ShowBuyShop(player.inventory);
            Console.WriteLine();
            switch (step)
            {
                case 0:
                    Console.WriteLine("1. 아이템 구매");
                    Console.WriteLine("2. 아이템 판매");
                    Console.WriteLine("0. 나가기");
                    Console.WriteLine();
                    Console.WriteLine("원하시는 행동을 입력해주세요.");
                    var input = InputKey();
                    if (input == ConsoleKey.D0)
                        currentScene = Scene.LobbyScene;
                    else if (input == ConsoleKey.D1)
                        step = 1;
                    else if (input == ConsoleKey.D2)
                        step = 2;
                    break;
                case 1:
                    Console.WriteLine("아이템의 번호로 원하는 아이템을 구매합니다.");
                    Console.WriteLine("0. 나가기");
                    Console.WriteLine();
                    Console.WriteLine("구매할 아이템의 번호를 입력해주세요.");
                    var input2 = Console.ReadLine();
                    if (int.TryParse(input2, out int res))
                    {
                        if (0 < res && res <= 10)
                        {
                            if (!player.inventory.HasSameItem(shopList[res - 1]))
                            {
                                var item = shopList[res - 1].DeepCopy();
                                if (item.Price <= player.Gold)
                                {
                                    player.BuyItem(item);
                                    Console.WriteLine("{0}을/를 샀습니다.", item.Name);
                                    player.SavePlayerData();
                                }
                                else
                                    Console.WriteLine("돈이 모자랍니다.");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine("이미 있는 아이템입니다.");
                                Console.ReadKey();
                            }
                        }
                        else if (res == 0)
                            step = res;
                    }
                    break;
                case 2:
                    Console.Clear();
                    Console.WriteLine("아이템의 번호로 원하는 아이템을 판매합니다.");
                    Console.WriteLine();
                    Console.WriteLine(" {0} | {1} | {2, -8} | {3, 4}", "번호", "이름" + "".PadLeft(player.inventory.MaxPad - 4), "정보", "가격");
                    player.inventory.ShowSaleList();
                    Console.WriteLine();
                    Console.WriteLine("0. 나가기");
                    Console.WriteLine();
                    Console.WriteLine("판매할 아이템의 번호를 입력해주세요.");
                    input2 = Console.ReadLine();
                    if (int.TryParse(input2, out res))
                    {
                        if (0 < res && res <= 10)
                        {
                            string name = player.SaleItem(res - 1);
                            if (name != null)
                            {
                                Console.WriteLine("{0}을/를 팔았습니다.", name);
                                player.SavePlayerData();
                                Console.ReadKey();
                            }
                        }
                        else if (res == 0)
                            step = res;
                    }
                    break;
            }
        }

        static void DrawDungeon(ref int step)
        {
            Console.Clear();
            Console.WriteLine("던전");
            Console.WriteLine("던전을 탐험합니다. 몬스터와 싸우고 보상을 얻습니다.");
            Console.WriteLine();
            Console.WriteLine("내 정보");
            Console.WriteLine("레  벨 : {0, 6 : #,##0}", player.Level);
            Console.WriteLine("공격력 : {0, 6 : #,##0}      체  력 : {1, 6 : #,##0}", player.Atk, player.HP);
            Console.WriteLine("방어력 : {0, 6 : #,##0}        돈   : {1, 6 : #,##0}", player.Def, player.Gold);
            Console.WriteLine();
            Console.WriteLine(" {0} | {1} | {2} | {3,6}", "번호", "이름" + "".PadLeft(DungeonInfo.MaxPad - 4), "요구방어력", "보상");
            for (int i = 0; i < dungeons.Length; i++)
            {
                int pad = DungeonInfo.MaxPad - Encoding.Default.GetBytes(dungeons[i].Name).Length;
                Console.WriteLine(" - {0, 2} | {1} | {2, 10} | {3, 6:#,##0} G", i + 1, dungeons[i].Name + "".PadLeft(pad), dungeons[i].NeedDef, dungeons[i].ClearGold);
            }
            Console.WriteLine();
            Console.WriteLine();
            switch (step)
            {
                case 0:
                    Console.WriteLine("1. 입장할 던전 선택");
                    Console.WriteLine("0. 나가기");
                    Console.WriteLine();
                    Console.WriteLine("원하시는 행동을 입력해주세요.");
                    var input = InputKey();
                    if (input == ConsoleKey.D0)
                        currentScene = Scene.LobbyScene;
                    else if (input == ConsoleKey.D1)
                        step = 1;
                    break;
                case 1:
                    Console.WriteLine("던전의 번호를 입력하여 해당 던전에 입장합니다.");
                    Console.WriteLine("0. 나가기");
                    Console.WriteLine();
                    Console.WriteLine("입장할 던전의 번호를 입력해주세요.");
                    var input2 = Console.ReadLine();
                    if (int.TryParse(input2, out int res))
                    {
                        if (0 < res && res <= dungeons.Length)
                        {
                            var currentDungeon = dungeons[res - 1];
                            var rand = new Random();
                            var monsterHP = currentDungeon.HP;
                            int monsterIdx = rand.Next(0, currentDungeon.Monsters.Length);
                            var monsterName = currentDungeon.Monsters[monsterIdx];
                            monsterHP = (monsterHP * (100 + monsterIdx * 25)) / 100;
                            Console.Clear();
                            Console.WriteLine("{0}에 입장했습니다.", currentDungeon.Name);
                            Console.WriteLine();
                            Console.WriteLine("{0}을/를 만났습니다!", monsterName);
                            Console.Write("몬스터의 남은 체력 : ");
                            int cursorLeft = Console.CursorLeft;
                            int cursorTop = Console.CursorTop;
                            Console.WriteLine("{0,-6:#,##0}", monsterHP);
                            int monsterCurrentHP = monsterHP;
                            List<ConsoleKey> battleKeys = new List<ConsoleKey>();
                            while (true)
                            {
                                Console.SetCursorPosition(cursorLeft, cursorTop);
                                Console.WriteLine("{0,6:#,##0}", monsterCurrentHP > 0 ? monsterCurrentHP : 0);
                                DrawHPGuage(monsterCurrentHP, monsterHP, ConsoleColor.DarkRed);
                                Console.WriteLine();
                                Console.WriteLine("나의 남은 체력 : {0,6:#,##0}", player.HP);
                                DrawHPGuage(player.HP, 100, ConsoleColor.DarkGreen);
                                RandomBattleKeys(rand, battleKeys);
                                if (monsterCurrentHP <= 0 || player.HP <= 0)
                                    break;
                                Console.WriteLine("입력하여 공격!");
                                Console.WriteLine("▼");
                                for (int i = 0; i < battleKeys.Count; i++)
                                    Console.Write(" {0}", battleKeys[i].ToString());
                                Console.WriteLine("\n▲");
                                while (true)
                                {
                                    input = InputKey();
                                    if (input == battleKeys.First())
                                    {
                                        monsterCurrentHP -= player.Atk;
                                        battleKeys.RemoveAt(0);
                                        break;
                                    }
                                    else if (input == ConsoleKey.A || input == ConsoleKey.S || input == ConsoleKey.D)
                                    {
                                        player.HitDamage(currentDungeon.NeedDef * 2);
                                        player.SavePlayerData();
                                        break;
                                    }
                                }
                            }
                            Console.SetCursorPosition(0, 12);
                            if (monsterCurrentHP <= 0)
                            {
                                player.Heal(0, -currentDungeon.ClearGold);
                                Console.WriteLine("{0}을/를 물리쳤습니다!", monsterName);
                                Console.WriteLine();
                                Console.WriteLine("[탐험 결과]");
                                Console.WriteLine("{0} G를 얻었습니다.", currentDungeon.ClearGold);
                                if (player.ExpUp())
                                    Console.WriteLine("레벨이 {0}에서 {1}로 증가했습니다!", player.Level - 1, player.Level);
                            }
                            else
                            {
                                player.Heal(10, 0);
                                Console.WriteLine("{0}에게 당했습니다!", monsterName);
                                Console.WriteLine();
                                Console.WriteLine("[탐험 결과]");
                                Console.WriteLine("당신은 목숨만을 겨우 부지한 체 마을로 돌아왔습니다...");
                            }
                            player.SavePlayerData();
                            Console.WriteLine();
                            Console.WriteLine("0. 나가기");
                            Console.WriteLine();
                            Console.WriteLine("원하시는 행동을 입력해주세요.");
                            do
                            {
                                input = InputKey();
                            }
                            while (input != ConsoleKey.D0);
                            step = 0;
                        }
                        else if (res == 0)
                            step = res;
                    }
                    break;

            }
        }

        static void DrawHPGuage(int currentHP, int maxHP, ConsoleColor color)
        {
            float rate = (float)currentHP / maxHP;
            int extraCnt = 50;
            Console.Write("[");
            Console.BackgroundColor = color;
            for (float f = rate; f >= 0; f -= 0.02f)
            {
                Console.Write(" ");
                extraCnt--;
            }
            Console.ResetColor();
            for (int i = 0; i <= extraCnt; i++)
                Console.Write(" ");
            Console.WriteLine("]");
        }

        static void RandomBattleKeys(Random rand, List<ConsoleKey> ret)
        {
            while (ret.Count < 10)
            {
                int r = rand.Next(0, 3);
                if (r == 0)
                    ret.Add(ConsoleKey.A);
                else if (r == 1)
                    ret.Add(ConsoleKey.S);
                else
                    ret.Add(ConsoleKey.D);
            }
        }
    }
}