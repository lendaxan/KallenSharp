using LeagueSharp;
using LeagueSharp.Common;
using System;
using Color = System.Drawing.Color;

namespace S_Class_Jinx
{
    public class ClassBase
    {
        public static Orbwalking.Orbwalker LukeOrbwalker { get; set; }
        public static Menu MainMenu { get; set; }
        public static Obj_AI_Hero PlayerHero { get; set; }
        public static bool IsInitialize;
        public static readonly BuffType[] Bufftype =
        {
            BuffType.Snare,
            BuffType.Blind,
            BuffType.Charm,
            BuffType.Stun,
            BuffType.Fear,
            BuffType.Slow,
            BuffType.Taunt,
            BuffType.Suppression
        };

        public static void Initialize()
        {
            if (IsInitialize) return;

            PlayerHero = ObjectManager.Player;

            Champion.Q = new Spell(SpellSlot.Q, PlayerHero.AttackRange);
            Champion.W = new Spell(SpellSlot.W, 1490f);
            Champion.E = new Spell(SpellSlot.E, 900f);
            Champion.R = new Spell(SpellSlot.R, 2150f);

            Champion.W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);
            Champion.E.SetSkillshot(1.2f, 120f, 1750f, false, SkillshotType.SkillshotCircle);
            Champion.R.SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.SkillshotLine);
            MenuManager.Initialize();
            IsInitialize = true;
        }

        internal static class MenuManager
        {
            private const string MenuName = "S Class Jinx";

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static void Initialize()
            {
                MainMenu = new Menu(MenuName,MenuName,true);
                MainMenu.AddSubMenu(CommonWalkerMenu());
                MainMenu.AddSubMenu(DelayMenu());
                MainMenu.AddSubMenu(DrawingMenu());
                MainMenu.AddSubMenu(ItemMenu());
                MainMenu.AddSubMenu(ManaMenu());
            }

            private static Menu CommonWalkerMenu()
            {
                LukeOrbwalker = new Orbwalking.Orbwalker(MainMenu.SubMenu("commonWalker"));
                var orbWalkingMenu = new Menu("Common OrbWalker", "commonWalker");
                var targetSelectorMenu = new Menu("Target Selector", "targetSelect");
                TargetSelector.AddToMenu(targetSelectorMenu);
                orbWalkingMenu.AddSubMenu(targetSelectorMenu);
                return orbWalkingMenu;
            }

            private static Menu DrawingMenu()
            {
                var drawMenu = new Menu("Drawing Settings", "Drawings");
                drawMenu.AddItem(new MenuItem("bDraw", "Display Drawing(s)").SetValue(true));
                drawMenu.AddItem(new MenuItem("cDrawOtherQRange", "Draw Other Form Auto Range").SetValue(new Circle(true, Color.Red)));
                drawMenu.AddItem(new MenuItem("cDrawAARange", "Draw Current Auto Range").SetValue(new Circle(true, Color.Green)));
                drawMenu.AddItem(new MenuItem("cDrawWRange", "Draw W Range").SetValue(new Circle(true, Color.Purple)));
                drawMenu.AddItem(new MenuItem("cDrawERange", "Draw E Range").SetValue(new Circle(true, Color.SteelBlue)));
                drawMenu.AddItem(new MenuItem("bDrawTextOnSelf", "Display Floating-Text (Self)").SetValue(true));
                return drawMenu;
            }

            private static Menu ItemMenu()
            {
                var itemMenu = new Menu("Item Options", "itemOptions");
                itemMenu.AddItem(new MenuItem("bUseBork", "Smart BotRK/Cutlass Usage").SetValue(true));
                itemMenu.AddItem(new MenuItem("sEnemyBorkHP", "Max% HP Target has remaining").SetValue(new Slider(50)));
                itemMenu.AddItem(new MenuItem("sMinPlayerHP", "Min% HP Remaining for player(Ignores other settings)").SetValue(new Slider(20)));
                itemMenu.AddItem(new MenuItem("bUseYoumuu", "Smart Youmuu's Usage").SetValue(true));

                var QSSMenu = new Menu("QSS Settings", "QSSMenu");

                QSSMenu.AddItem(new MenuItem("bUseQSS", "Use QSS").SetValue(true));
                QSSMenu.AddItem(new MenuItem("sQSSDelay", "QSS Delay").SetValue(new Slider(300, 250, 1500)));

                foreach (var buff in Bufftype)
                {
                    QSSMenu.AddItem(new MenuItem("bUseQSS." + buff, "Use QSS On" + buff).SetValue(true));
                }

                var MercMenu = new Menu("Merc Settings", "MercMenu");
                MercMenu.AddItem(new MenuItem("bUseMerc", "Smart Merc Usage").SetValue(true));
                MercMenu.AddItem(new MenuItem("sMercDelay", "Merc Delay").SetValue(new Slider(300, 250, 1500)));

                foreach (var buff in Bufftype)
                {
                    MercMenu.AddItem(new MenuItem("bUseMerc." + buff, "Use Merc On" + buff).SetValue(true));
                }
                itemMenu.AddSubMenu(QSSMenu);
                itemMenu.AddSubMenu(MercMenu);

                itemMenu.AddItem(new MenuItem("bUseOffensiveOnlyInCombo", "Only use offensive items in combo").SetValue(true));
                itemMenu.AddItem(new MenuItem("bUseDefensiveOnlyInCombo", "Only use defensive items in combo").SetValue(true));
                return itemMenu;
            }

            private static Menu DelayMenu()
            {
                var humanMenu = new Menu("Delay Menu (ms)", "delayOptions");
                humanMenu.AddItem(new MenuItem("sQdelay", "Delay Between Q Swaping").SetValue(new Slider(200, 100, 500)));       
                humanMenu.AddItem(new MenuItem("sLevelDelay", "Delay Between Auto Level Spells").SetValue(new Slider(750, 250, 1000)));
                humanMenu.AddItem(new MenuItem("sAutoDelay", "Delay Between Auto Events").SetValue(new Slider(750, 250, 1000)));
                humanMenu.AddItem(new MenuItem("sItemDelay", "Delay Between Item's Ussage").SetValue(new Slider(650, 450, 1000)));
                humanMenu.AddItem(new MenuItem("sTrinketDelay", "Delay Between Trinket Check").SetValue(new Slider(1000, 500, 2000)));
                humanMenu.AddItem(new MenuItem("sMinRandom", "Minimum Randomizer Delay (applies to all delays)").SetValue(new Slider(0, 0, 500)));
                humanMenu.AddItem(new MenuItem("sMaxRandom", "Maximum Randomizer Delay (applies to all delays)").SetValue(new Slider(250, 0, 500)));
                return humanMenu;
            }

            private static Menu ManaMenu()
            {
                var autoEventsMenu = new Menu("Mana Manager", "manaMenu");
                autoEventsMenu.AddItem(new MenuItem("bUseManaManager", "Use Mana Manager").SetValue(true));
                autoEventsMenu.AddItem(new MenuItem("sMinManaQ", ">> Min. % Mana for Q").SetValue(new Slider(10, 0, 75)));
                autoEventsMenu.AddItem(new MenuItem("sMinManaW", ">> Min. % Mana for W").SetValue(new Slider(35, 25, 75)));
                autoEventsMenu.AddItem(new MenuItem("sMinManaE", ">> Min. % Mana for E").SetValue(new Slider(20, 0, 75)));
                autoEventsMenu.AddItem(new MenuItem("sMinManaR", ">> Min. % Mana for R").SetValue(new Slider(30, 0, 75)));
                return autoEventsMenu;
            }


        }

        internal static class Champion
        {
            public static Spell Q { get; set; }
            public static Spell W { get; set; }
            public static Spell E { get; set; }
            public static Spell R { get; set; }
        }

        internal static class Time
        {
            private static readonly DateTime AssemblyLoadTime = DateTime.Now;

            public static float TickCount
            {
                get
                {
                    return (int)DateTime.Now.Subtract(AssemblyLoadTime).TotalMilliseconds;
                }
            }
        }

    }
}
