using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml.Linq;
using EFT;
using EFT.Counters;
using EFT.InventoryLogic;
using EFT.UI;
using EscapeFromTarkovCheat;
using EscapeFromTarkovCheat.Features;
using EscapeFromTarkovCheat.Utils;
using UnityEngine;
using static RootMotion.FinalIK.AimPoser;

namespace Menu.UI
{
    public class Menu : MonoBehaviour
    {
        private Rect MainWindow;
        private bool _visible = true;
        public string ItemID = "";
        public string ContainerID = "";
        public string ContainerID2 = "";
        private bool MiscMenuToggle;
        private bool SpawnMenuToggle;
        private bool LootMenuToggle;
        private bool ItemMenuToggle;
        string[] ContainerOptions = new string[] {
            "5c0a840b86f7742ffa4f2482",//THICC
            "5d235bb686f77443f4331278",//SICC
            "5c093db286f7740a1b2617e3",//Food
            "5aafbcd986f7745e590fff23",//Med
            "5c0a794586f77461c458f892",//BOSS
            "5b7c710788a4506dec015957",//Scav Box
            "5aafbde786f774389d0cbc0f",//Ammo
            "619cbf9e0a7c3a1a2731940a",//KeyCard
            "59fb016586f7746d0d4b423a",//Cash
            "5c127c4486f7745625356c13",//Magazine
            "5e2af55f86f7746d4159f07c",//Grenade
            "590c60fc86f77412b13fddcf",//Documents
        };

        private Rect MiscWindow;
        private Rect SpawnWindow;
        private Rect LootWindow;
        private Rect ItemWindow;
        private float _nextArmorTime;
        private float _nextArmorRefreshTime;

        public void GetItemsInBackpack(EquipmentSlot slot)
        {
            List<string> list = new List<string>();
            IEnumerable<Item> itemsInSlots = Main.LocalPlayer.Profile.Inventory.GetItemsInSlots(new EquipmentSlot[]
            {
                slot
            });
            if (itemsInSlots != null)
            {
                foreach (Item item in itemsInSlots)
                {
                    if (item.ShortName.Localized() == "6SH118")
                    {
                        continue;
                    }
                    list.Add("\"" + item.Template._id + "\",//" + item.ShortName.Localized());
                }
                Settings.idList = string.Join("\n", list);
            }
            else
            {
                Settings.idList = "";
            }
        }

        private void Start()
        {
            AllocConsoleHandler.Open();
            _nextArmorTime = Time.time;
            _nextArmorRefreshTime = Time.time;
            MainWindow = new Rect(60f, 160f, 180f, 100f);
            MiscWindow = new Rect(60f, 1060f, 180f, 100f);
            SpawnWindow = new Rect(60f, 1340f, 180f, 100f);
            LootWindow = new Rect(370f, 160f, 180f, 100f);
            ItemWindow = new Rect(40f, 720f, 180f, 100f);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
                _visible = !_visible;
            if(Time.time >= _nextArmorTime && Settings.AutoArmor)
            {
                AutoArmor();
                _nextArmorTime = Time.time + 3f;
            }
            if (Time.time >= _nextArmorRefreshTime && Settings.AutoRefreshArmor)
            {
                AutoRefreshArmor();
                _nextArmorRefreshTime = Time.time + Settings.AutoRefreshArmorInterval;
            }
            //if (Input.GetKeyDown(KeyCode.Delete))
            //Loader.Unload();
        }

        private void OnGUI()
        {
            if (!_visible)
                return;

            MainWindow = GUILayout.Window(0, MainWindow, RenderUi, $"Offline: {Settings.OfflineStreetStatus}");
            if (this.MiscMenuToggle)
            {
                this.MiscWindow = GUILayout.Window(1, this.MiscWindow, new GUI.WindowFunction(this.RenderUi), "Misc", Array.Empty<GUILayoutOption>());
            }
            if (this.SpawnMenuToggle)
            {
                this.SpawnWindow = GUILayout.Window(2, this.SpawnWindow, new GUI.WindowFunction(this.RenderUi), "Boss Spawn", Array.Empty<GUILayoutOption>());
            }
            if (this.LootMenuToggle)
            {
                this.LootWindow = GUILayout.Window(3, this.LootWindow, new GUI.WindowFunction(this.RenderUi), "Loot", Array.Empty<GUILayoutOption>());
            }
            if (this.ItemMenuToggle)
            {
                this.ItemWindow = GUILayout.Window(4, this.ItemWindow, new GUI.WindowFunction(this.RenderUi), "Item Spawn", Array.Empty<GUILayoutOption>());
            }
        }

        private void RenderUi(int id)
        {
            switch (id)
            {
                case 0:

                    GUILayout.Label("");
                    Settings.DrawPlayers = GUILayout.Toggle(Settings.DrawPlayers, "Draw Players");
                    Settings.HalfGodMode = GUILayout.Toggle(Settings.HalfGodMode, "HalfGodMode", Array.Empty<GUILayoutOption>());
                    GUILayout.Label($"Damage Reduction {(int)Settings.DamageReduction * 5} %");
                    Settings.DamageReduction = (int)GUILayout.HorizontalSlider(Settings.DamageReduction, 0, 20);
                    GUILayout.Label($"Heal Per Sec {Settings.HealPerSec * 0.5f}");
                    Settings.HealPerSec = (int)GUILayout.HorizontalSlider(Settings.HealPerSec, 0, 30);
                    GUILayout.Label($"Base Heal Per Sec {Settings.BaseHealPerSec * 0.05f}");
                    Settings.BaseHealPerSec = (int)GUILayout.HorizontalSlider(Settings.BaseHealPerSec, 0, 20);
                    GUILayout.Label("");
                    Settings.AutoRefreshArmor = GUILayout.Toggle(Settings.AutoRefreshArmor, "RefreshArmor");
                    GUILayout.Label($"Refresh Interval {Settings.AutoRefreshArmorInterval}");
                    Settings.AutoRefreshArmorInterval = (int)GUILayout.HorizontalSlider(Settings.AutoRefreshArmorInterval, 2, 30);
                    //Settings.AutoArmor = GUILayout.Toggle(Settings.AutoArmor, "AutoArmor");
                    Settings.BloodBathModeBoss = GUILayout.Toggle(Settings.BloodBathModeBoss, "BloodBath BossMode");
                    Settings.BloodBathModePMC = GUILayout.Toggle(Settings.BloodBathModePMC, "BloodBath PMCMode");
                    GUILayout.Label($"SpawnCount {Settings.SpawnCount}");
                    Settings.SpawnCount = (int)GUILayout.HorizontalSlider(Settings.SpawnCount, 1, 5);
                    GUILayout.Label($"SpawnInterval {(int)Settings.SpawnInterval * 10}");
                    Settings.SpawnInterval = (int)GUILayout.HorizontalSlider(Settings.SpawnInterval, 6, 24);
                    if (GUILayout.Button("Misc", GUILayout.Width(180f), GUILayout.Height(30f)))
                    {
                        this.MiscMenuToggle = !this.MiscMenuToggle;
                    }
                    if (GUILayout.Button("Boss Spawn", GUILayout.Width(180f), GUILayout.Height(30f)))
                    {
                        this.SpawnMenuToggle = !this.SpawnMenuToggle;
                    }
                    if (GUILayout.Button("Loot", GUILayout.Width(180f), GUILayout.Height(30f)))
                    {
                        this.LootMenuToggle = !this.LootMenuToggle;
                    }
                    if (GUILayout.Button("Item Spawn", GUILayout.Width(180f), GUILayout.Height(30f)))
                    {
                        this.ItemMenuToggle = !this.ItemMenuToggle;
                    }

                    //GUILayout.Label("");

                    //if (GUILayout.Button("Airdrop", GUILayout.Width(220f), GUILayout.Height(30f)))
                    //    GenerateAirdrop();
                    //if (GUILayout.Button("Acid Green", GUILayout.Width(220f), GUILayout.Height(30f)))
                    //    GenerateAcidGreen();
                    //if (GUILayout.Button("TraderStanding", Array.Empty<GUILayoutOption>()))
                    //    Settings.IncreaseTraderStanding = true;

                    GUILayout.Label("");
                    if (GUILayout.Button("Equipment", GUILayout.Width(180f), GUILayout.Height(50f)))
                    {
                        SpawnEquipment();
                    }
                    if (GUILayout.Button("Headset", GUILayout.Width(180f), GUILayout.Height(50f)))
                    {
                        SpawnParts(1);
                    }
                    if (GUILayout.Button("Consumable", GUILayout.Width(180f), GUILayout.Height(50f)))
                    {
                        SpawnConsumable();
                    }
                    break;
                case 1:
                    Settings.GodMode = GUILayout.Toggle(Settings.GodMode, "GodMode", Array.Empty<GUILayoutOption>());
                    GUILayout.Label($"LoadSpeed {Settings.LoadSpeed * 2.5f} %");
                    Settings.LoadSpeed = (int)GUILayout.HorizontalSlider(Settings.LoadSpeed, 30, 38);
                    GUILayout.Label($"UnloadSpeed {Settings.UnloadSpeed * 2.5f} %");
                    Settings.UnloadSpeed = (int)GUILayout.HorizontalSlider(Settings.UnloadSpeed, 28, 38);
                    Settings.DrawExfiltrationPoints = GUILayout.Toggle(Settings.DrawExfiltrationPoints, "Show Exits");
                    Settings.ForceThermal = GUILayout.Toggle(Settings.ForceThermal, "Thermal Vision", Array.Empty<GUILayoutOption>());
                    Settings.SpeedHack = GUILayout.Toggle(Settings.SpeedHack, "SpeedHack", Array.Empty<GUILayoutOption>());
                    GUILayout.Label($"Multiplier {(int)Settings.SpeedMulti}X");
                    Settings.SpeedMulti = GUILayout.HorizontalSlider(Settings.SpeedMulti, 5f, 50f);
                    GUILayout.Label("");
                    if (GUILayout.Button("Quest", Array.Empty<GUILayoutOption>()))
                        Settings.FinishQuest = true;
                    if (GUILayout.Button("Full Hydro & Energy", GUILayout.Width(180f), GUILayout.Height(50f)))
                        WaterAndFood();
                    if (GUILayout.Button("Add EXP", GUILayout.Width(180f), GUILayout.Height(30f)))
                        XPAdder();
                    if (GUILayout.Button("Backpack Items", GUILayout.Width(180f), GUILayout.Height(30f)))
                        GetItemsInBackpack(EquipmentSlot.Backpack);
                    GUILayout.TextField(Settings.idList, new GUILayoutOption[]
                    {
                        GUILayout.Width(180f), GUILayout.Height(30f)
                    });
                    break;

                case 2:
                    //bossSpawner.SpawnBoss(WildSpawnType.bossBully);
                    //bossSpawner.SpawnBoss(WildSpawnType.bossKilla);
                    //bossSpawner.SpawnBoss(WildSpawnType.bossKojaniy);
                    //bossSpawner.SpawnBoss(WildSpawnType.bossGluhar);
                    //bossSpawner.SpawnBoss(WildSpawnType.bossSanitar);
                    //bossSpawner.SpawnBoss(WildSpawnType.sectantPriest);
                    //bossSpawner.SpawnBoss(WildSpawnType.bossTagilla);
                    //bossSpawner.SpawnBoss(WildSpawnType.bossKnight);
                    //bossSpawner.SpawnBoss(WildSpawnType.followerBigPipe);
                    //bossSpawner.SpawnBoss(WildSpawnType.followerBirdEye);
                    //bossSpawner.SpawnBoss(WildSpawnType.bossZryachiy);
                    //bossSpawner.SpawnBoss(WildSpawnType.bossBoar);
                    //bossSpawner.SpawnBoss(WildSpawnType.bossPartisan);
                    //bossSpawner.SpawnBoss(WildSpawnType.bossKolontay);
                    //bossSpawner.SpawnBoss(WildSpawnType.ravangeZryachiyEvent);
                    //bossSpawner.SpawnBear();
                    //bossSpawner.SpawnUsec();
                    if (GUILayout.Button("BEAR", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for(int i = 0; i< 5; i++)
                        {
                            Main.bossSpawner.SpawnBear();
                        }
                    if (GUILayout.Button("USEC", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnUsec();
                        }
                    if (GUILayout.Button("Priest", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.sectantPriest);
                        }
                    if (GUILayout.Button("Priest Follower", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.sectantWarrior);
                        }
                    if (GUILayout.Button("Reshala", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.bossBully);
                        }
                    if (GUILayout.Button("Killa", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.bossKilla);
                        }
                    if (GUILayout.Button("Shturman", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.bossKojaniy);
                        }
                    if (GUILayout.Button("Gluhar", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.bossGluhar);
                        }
                    if (GUILayout.Button("Sanitar", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.bossSanitar);
                        }
                    if (GUILayout.Button("Tagilla", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.bossTagilla);
                        }
                    if (GUILayout.Button("Knight", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.bossKnight);
                        }
                    if (GUILayout.Button("BigPipe", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.followerBigPipe);
                        }
                    if (GUILayout.Button("BirdEye", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.followerBirdEye);
                        }
                    if (GUILayout.Button("Zryachiy", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.bossZryachiy);
                        }
                    if (GUILayout.Button("Kaban", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.bossBoar);
                        }
                    if (GUILayout.Button("Partisan", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.bossPartisan);
                        }
                    if (GUILayout.Button("Kolontay", GUILayout.Width(180f), GUILayout.Height(30f)))
                        for (int i = 0; i < 5; i++)
                        {
                            Main.bossSpawner.SpawnBoss(WildSpawnType.bossKolontay);
                        }
                    break;
                case 3:
                    Settings.DrawPlayers = GUILayout.Toggle(Settings.DrawPlayers, "Draw Players");
                    Settings.DrawPlayerLoots = GUILayout.Toggle(Settings.DrawPlayerLoots, "Draw Players Loot");
                    //Settings.DrawPlayerSkeleton = GUILayout.Toggle(Settings.DrawPlayerSkeleton, "Draw Skeleton");
                    //Settings.DrawPlayerName = GUILayout.Toggle(Settings.DrawPlayerName, "Draw Player Name");
                    GUILayout.Label($"Player Distance {(int)Settings.DrawPlayersDistance} m");
                    Settings.DrawPlayersDistance = GUILayout.HorizontalSlider(Settings.DrawPlayersDistance, 0f, 4000f);

                    Settings.DrawLootItems = GUILayout.Toggle(Settings.DrawLootItems, "Show Loot Items");
                    Settings.DrawNearbyLootItems = GUILayout.Toggle(Settings.DrawNearbyLootItems, "Show Nearby Items");
                    Settings.DrawDeadBodyItems = GUILayout.Toggle(Settings.DrawDeadBodyItems, "Show Deadbody Items");
                    GUILayout.Label($"Loot Item Distance {(int)Settings.DrawLootItemsDistance} m");
                    Settings.DrawLootItemsDistance = GUILayout.HorizontalSlider(Settings.DrawLootItemsDistance, 5f, 4000f);

                    Settings.DrawLootableContainers = GUILayout.Toggle(Settings.DrawLootableContainers, "Draw Containers");
                    GUILayout.Label($"Container Distance {(int)Settings.DrawLootableContainersDistance} m");
                    Settings.DrawLootableContainersDistance = GUILayout.HorizontalSlider(Settings.DrawLootableContainersDistance, 0f, 4000f);

                    GUILayout.Label("");

                    Settings.ValuableToggle = GUILayout.Toggle(Settings.ValuableToggle, "Valuable", Array.Empty<GUILayoutOption>());
                    Settings.WishListAmmoToggle = GUILayout.Toggle(Settings.WishListAmmoToggle, "WishList Ammo", Array.Empty<GUILayoutOption>());
                    Settings.WishListGearPlateToggle = GUILayout.Toggle(Settings.WishListGearPlateToggle, "WishList Gear&Plate", Array.Empty<GUILayoutOption>());
                    Settings.WishListConsumableToggle = GUILayout.Toggle(Settings.WishListConsumableToggle, "WishList Consumable", Array.Empty<GUILayoutOption>());
                    Settings.CheckPCItemAmmoToggle = GUILayout.Toggle(Settings.CheckPCItemAmmoToggle, "P&C Ammo", Array.Empty<GUILayoutOption>());
                    Settings.CheckPCItemGearPlateToggle = GUILayout.Toggle(Settings.CheckPCItemGearPlateToggle, "P&C Gear&Plate", Array.Empty<GUILayoutOption>());
                    Settings.TemporaryItemListToggle = GUILayout.Toggle(Settings.TemporaryItemListToggle, "Temporary", Array.Empty<GUILayoutOption>());

                    GUILayout.Label("");

                    Settings.LootMode = GUILayout.Toggle(Settings.LootMode, "LootMode", Array.Empty<GUILayoutOption>());
                    Settings.LootGroup1 = GUILayout.Toggle(Settings.LootGroup1, "LootGroup1", Array.Empty<GUILayoutOption>());
                    Settings.LootGroup2 = GUILayout.Toggle(Settings.LootGroup2, "LootGroup2", Array.Empty<GUILayoutOption>());
                    Settings.LootGroup3 = GUILayout.Toggle(Settings.LootGroup3, "LootGroup3", Array.Empty<GUILayoutOption>());
                    Settings.LootGroup4 = GUILayout.Toggle(Settings.LootGroup4, "LootGroup4", Array.Empty<GUILayoutOption>());
                    Settings.DogTagLV60 = GUILayout.Toggle(Settings.DogTagLV60, "DogTagLV60", Array.Empty<GUILayoutOption>());
                    GUILayout.Label($"PlayerCacheInterval {(int)Settings.PlayerCacheInterval} Sec");
                    Settings.PlayerCacheInterval = GUILayout.HorizontalSlider(Settings.PlayerCacheInterval, 3f, 25f);
                    break;
                case 4:

                    //if (GUILayout.Button("Spawn Vest", GUILayout.Width(220f), GUILayout.Height(70f)))
                    //{
                    //    Spawnitem(2);
                    //    Spawnitem(2);
                    //    Spawnitem(2);
                    //    Spawnitem(2);
                    //    Spawnitem(2);
                    //    Spawnitem(2);
                    //    Spawnitem(4);
                    //    Spawnitem(4);
                    //    Spawnitem(4);
                    //    Spawnitem(4);
                    //    Spawnitem(4);
                    //    Spawnitem(4);
                    //}

                    if (GUILayout.Button("Spawn in Backpack", GUILayout.Width(220f), GUILayout.Height(40f)))
                    {
                        Spawnitem(1);
                    }

                    //if (GUILayout.Button("Spawn Container", GUILayout.Width(220f), GUILayout.Height(40f)))
                    //{
                    //    Spawnitem(3);
                    //}

                    if (GUILayout.Button("★Spawn Case", GUILayout.Width(220f), GUILayout.Height(40f)))
                    {
                        SpawnCase();
                    }

                    if (GUILayout.Button("★Spawn Armor", GUILayout.Width(220f), GUILayout.Height(40f)))
                    {
                        SpawnArmor();
                    }

                    GUILayout.Label("");
                    ContainerID = GUILayout.TextField(ContainerID, new GUILayoutOption[]
                    {
                        GUILayout.Width(220f),
                        GUILayout.Height(30f)
                    });
                    ContainerID2 = GUILayout.TextField(ContainerID2, new GUILayoutOption[]
                    {
                        GUILayout.Width(220f),
                        GUILayout.Height(30f)
                    });
                    ItemID = GUILayout.TextField(ItemID, new GUILayoutOption[]
                    {
                        GUILayout.Width(220f),
                        GUILayout.Height(50f)
                    });

                    break;
            }


            GUI.DragWindow();

        }

        public void SpawnParts(int type)
        {
            ItemFactory itemFactory = new ItemFactory();
            dynamic Ear = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Earpiece).ContainedItem;
            dynamic Head = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Headwear).ContainedItem;
            if (type == 1 && !GameUtils.IsInventoryItemValid(Ear))
            {
                Item item = itemFactory.CreateItem("66b5f6985891c84aab75ca76");//Comtac VI
                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Earpiece).AddWithoutRestrictions(item);
            }
        }


        public void SpawnConsumable()
        {
            ItemFactory itemFactory = new ItemFactory();
            dynamic container = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Backpack).ContainedItem;

            //弹药箱
            CompoundItem AmmoCase = itemFactory.CreateCompoundItem("5aafbde786f774389d0cbc0f");
            for (int i = 0; i < 11; i++)
            {
                Item Ammo = itemFactory.CreateItem("54527ac44bdc2d36668b4567");
                AmmoCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 11; i++)
            {
                Item Ammo = itemFactory.CreateItem("61962b617c6c7b169525f168");
                AmmoCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 11; i++)
            {
                Item Ammo = itemFactory.CreateItem("5efb0c1bd79ff02a1f5e68d9");
                AmmoCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 11; i++)
            {
                Item Ammo = itemFactory.CreateItem("5c0d688c86f77413ae3407b2");
                AmmoCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 5; i++)
            {
                Item Ammo = itemFactory.CreateItem("5fc382a9d724d907e2077dab");
                AmmoCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            container.Grids[0].AddItemWithoutRestrictions(AmmoCase, container.Grids[0].FindFreeSpace(AmmoCase));

            //物品箱 塞医疗
            CompoundItem ItemCase = itemFactory.CreateCompoundItem("59fb042886f7746c5005a7b2");
            for (int i = 0; i < 6; i++)
            {
                Item Ammo = itemFactory.CreateItem("5c0e534186f7747fa1419867");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 2; i++)
            {
                Item Ammo = itemFactory.CreateItem("5af0548586f7743a532b7e99");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 6; i++)
            {
                Item Ammo = itemFactory.CreateItem("5c0e534186f7747fa1419867");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 2; i++)
            {
                Item Ammo = itemFactory.CreateItem("5c0e533786f7747fa23f4d47");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 6; i++)
            {
                Item Ammo = itemFactory.CreateItem("5c0e534186f7747fa1419867");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 2; i++)
            {
                Item Ammo = itemFactory.CreateItem("5c0e533786f7747fa23f4d47");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 6; i++)
            {
                Item Ammo = itemFactory.CreateItem("5c0e534186f7747fa1419867");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 2; i++)
            {
                Item Ammo = itemFactory.CreateItem("5c0e533786f7747fa23f4d47");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 1; i++)
            {
                Item Ammo = itemFactory.CreateItem("5d02797c86f774203f38e30a");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 5; i++)
            {
                Item Ammo = itemFactory.CreateItem("5af0454c86f7746bf20992e8");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 1; i++)
            {
                Item Ammo = itemFactory.CreateItem("5d02797c86f774203f38e30a");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 5; i++)
            {
                Item Ammo = itemFactory.CreateItem("5af0454c86f7746bf20992e8");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 1; i++)
            {
                Item Ammo = itemFactory.CreateItem("5d02797c86f774203f38e30a");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            CompoundItem InjectorCase = itemFactory.CreateCompoundItem("619cbf7d23893217ec30b689");
            for (int i = 0; i < 9; i++)
            {
                Item Ammo = itemFactory.CreateItem("5ed5166ad380ab312177c100");
                InjectorCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            ItemCase.Grids[0].AddAnywhere(InjectorCase, EErrorHandlingType.Ignore);
            for (int i = 0; i < 4; i++)
            {
                Item Ammo = itemFactory.CreateItem("60098ad7c2240c0fe85c570a");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 1; i++)
            {
                Item Ammo = itemFactory.CreateItem("5d02797c86f774203f38e30a");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            CompoundItem InjectorCase2 = itemFactory.CreateCompoundItem("619cbf7d23893217ec30b689");
            for (int i = 0; i < 3; i++)
            {
                Item Ammo = itemFactory.CreateItem("5c0e531d86f7747fa23f4d42");
                InjectorCase2.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 3; i++)
            {
                Item Ammo = itemFactory.CreateItem("637b620db7afa97bfc3d7009");
                InjectorCase2.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 3; i++)
            {
                Item Ammo = itemFactory.CreateItem("637b612fb7afa97bfc3d7005");
                InjectorCase2.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            ItemCase.Grids[0].AddAnywhere(InjectorCase2, EErrorHandlingType.Ignore);
            for (int i = 0; i < 4; i++)
            {
                Item Ammo = itemFactory.CreateItem("60098ad7c2240c0fe85c570a");
                ItemCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            container.Grids[0].AddItemWithoutRestrictions(ItemCase, container.Grids[0].FindFreeSpace(ItemCase));

            //榴弹箱
            CompoundItem GrenadeCase = itemFactory.CreateCompoundItem("5e2af55f86f7746d4159f07c");
            for (int i = 0; i < 40; i++)
            {
                Item Ammo = itemFactory.CreateItem("617fd91e5539a84ec44ce155");
                GrenadeCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            for (int i = 0; i < 24; i++)
            {
                Item Ammo = itemFactory.CreateItem("619256e5f8af2c1a4e1f5d92");
                GrenadeCase.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            container.Grids[0].AddItemWithoutRestrictions(GrenadeCase, container.Grids[0].FindFreeSpace(GrenadeCase));

        }

        public void AutoRefreshArmor()
        {
            ItemFactory itemFactory = new ItemFactory();
            dynamic armor = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.ArmorVest).ContainedItem;
            //Item item = ConstructArmor("60a283193cb70855c43a381d");//THOR IC
            //Item item = ConstructArmor("5b44cf1486f77431723e3d05");//Gen4 突击型
            Item item = ConstructArmor("545cdb794bdc2d3a198b456a");//6B43
            Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.ArmorVest).RemoveItem();
            Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.ArmorVest).Add(item, false, false);
        }
        public void AutoArmor()
        {
            ItemFactory itemFactory = new ItemFactory();
            dynamic armor = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.ArmorVest).ContainedItem;
            dynamic Face = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.FaceCover).ContainedItem;
            dynamic Eye = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Eyewear).ContainedItem;
            dynamic Head = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Headwear).ContainedItem;
            dynamic Ear = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Earpiece).ContainedItem;
            if (!GameUtils.IsInventoryItemValid(Eye))
            {
                Item item = itemFactory.CreateItem("603409c80ca681766b6a0fb2");//NPP KlASS Condor
                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Eyewear).AddWithoutRestrictions(item);
            }
            if (!GameUtils.IsInventoryItemValid(Face))
            {
                Item item = itemFactory.CreateItem("6570aead4d84f81fd002a033");//Death Shadow
                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.FaceCover).AddWithoutRestrictions(item);
            }
            if (!GameUtils.IsInventoryItemValid(armor))
            {
                //Item item = ConstructArmor("60a283193cb70855c43a381d");//THOR IC
                //Item item = ConstructArmor("5b44cf1486f77431723e3d05");//Gen4 突击型
                Item item = ConstructArmor("545cdb794bdc2d3a198b456a");//6B43
                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.ArmorVest).RemoveItem();
                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.ArmorVest).Add(item, false, false);
            }
            if (GameUtils.IsInventoryItemValid(Head) && !GameUtils.IsInventoryItemValid(Ear))
            {
                Item item = itemFactory.CreateItem("66b5f6985891c84aab75ca76");//Comtac VI
                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Earpiece).AddWithoutRestrictions(item);
            }
        }
        public void SpawnEquipment()
        {
            ItemFactory itemFactory = new ItemFactory();
            dynamic backpack = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Backpack).ContainedItem;
            dynamic vest = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.TacticalVest).ContainedItem;
            dynamic armor = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.ArmorVest).ContainedItem;
            dynamic Eye = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Eyewear).ContainedItem;
            dynamic Face = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.FaceCover).ContainedItem;
            dynamic Ear = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Earpiece).ContainedItem;
            dynamic Head = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Headwear).ContainedItem;
            //dynamic container = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.SecuredContainer).ContainedItem;

            if (!GameUtils.IsInventoryItemValid(backpack))
            {
                Item item = itemFactory.CreateItem("5df8a4d786f77412672a1e3b");//6Sh118
                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Backpack).Add(item, false, false);
                backpack = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Backpack).ContainedItem;
            }
            if (!GameUtils.IsInventoryItemValid(vest))
            {
                Item item = itemFactory.CreateItem("5df8a42886f77412640e2e75");//MPPV
                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.TacticalVest).Add(item, false, false);
                vest = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.TacticalVest).ContainedItem;
            }
            //for (int i = 0; i < 2; i++)
            //{
            //    for (int j = 0; j < 3; j++)
            //    {
            //        Item nade = itemFactory.CreateItem("617fd91e5539a84ec44ce155");//RGN
            //        vest.Grids[5 + j].AddAnywhere(nade, EErrorHandlingType.Ignore);
            //    }
            //}
            //for (int j = 0; j < 2; j++)
            //{
            //    Item nade = itemFactory.CreateItem("619256e5f8af2c1a4e1f5d92");//M7290
            //    vest.Grids[8 + j].AddAnywhere(nade, EErrorHandlingType.Ignore);
            //}
            //for (int j = 0; j < 2; j++)
            //{
            //    Item nade = itemFactory.CreateItem("619256e5f8af2c1a4e1f5d92");//M7290
            //    vest.Grids[12 + j].AddAnywhere(nade, EErrorHandlingType.Ignore);
            //}
            //for (int j = 0; j < 2; j++)
            //{
            //    Item nade = itemFactory.CreateItem("619256e5f8af2c1a4e1f5d92");//M7290
            //    vest.Grids[11].AddAnywhere(nade, EErrorHandlingType.Ignore);
            //}

            if (!GameUtils.IsInventoryItemValid(Eye))
            {
                Item item = itemFactory.CreateItem("603409c80ca681766b6a0fb2");//NPP KlASS Condor
                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Eyewear).AddWithoutRestrictions(item);
            }
            if (!GameUtils.IsInventoryItemValid(Face))
            {
                Item item = itemFactory.CreateItem("6570aead4d84f81fd002a033");//Death Shadow
                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.FaceCover).AddWithoutRestrictions(item);
            }
            if (!GameUtils.IsInventoryItemValid(Head))
            {
                //Item item1 = ConstructArmor("5c17a7ed2e2216152142459c");//AirFrame Tan T7
                //Item item2 = ConstructArmor("5c17a7ed2e2216152142459c-2");//AirFrame Tan GPNVG-18
                //Item item3 = ConstructArmor("5f60c74e3b85f6263c145586");//Rys-T
                Item item4 = ConstructArmor("5ca20ee186f774799474abc2");//Vulkan-5
                //Item item5 = ConstructArmor("5c0e874186f7745dc7616606");//Maska 1Sch
                //Item item6 = ConstructArmor("58ac60eb86f77401897560ff");//Balaclava_dev
                //backpack.Grids[0].AddAnywhere(item1, EErrorHandlingType.Ignore);
                //backpack.Grids[0].AddAnywhere(item2, EErrorHandlingType.Ignore);
                //backpack.Grids[0].AddAnywhere(item3, EErrorHandlingType.Ignore);
                backpack.Grids[0].AddAnywhere(item4, EErrorHandlingType.Ignore);
                //backpack.Grids[0].AddAnywhere(item5, EErrorHandlingType.Ignore);
                //backpack.Grids[0].AddAnywhere(item6, EErrorHandlingType.Ignore);
            }
            if (!GameUtils.IsInventoryItemValid(armor))
            {
                //Item item = ConstructArmor("60a283193cb70855c43a381d");//THOR IC
                //Item item = ConstructArmor("5b44cf1486f77431723e3d05");//Gen4 突击型
                Item item = ConstructArmor("545cdb794bdc2d3a198b456a");//6B43
                //backpack.Grids[0].AddAnywhere(item, EErrorHandlingType.Ignore);
                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.ArmorVest).Add(item, false, false);
                //Item itemP1 = itemFactory.CreateItem("656faf0ca0dce000a2020f77");//GAC 4sss2
                //Item itemP2 = itemFactory.CreateItem("656faf0ca0dce000a2020f77");//GAC 4sss2
                //backpack.Grids[0].AddAnywhere(itemP1, EErrorHandlingType.Ignore);
                //backpack.Grids[0].AddAnywhere(itemP2, EErrorHandlingType.Ignore);
            }
            //if (!GameUtils.IsInventoryItemValid(Ear))
            //{
            //    Item item = itemFactory.CreateItem("66b5f6985891c84aab75ca76");//Comtac VI
            //    Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Earpiece).AddWithoutRestrictions(item);
            //}


        }
        public void SpawnArmor()
        {
            dynamic backpack = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Backpack).ContainedItem;
            if (!GameUtils.IsInventoryItemValid(backpack))
                return;
            CompoundItem Armor = ConstructArmor(ItemID);
            if (Armor == null)
                return;
            backpack.Grids[0].AddAnywhere(Armor, EErrorHandlingType.Ignore);
        }
        public CompoundItem ConstructArmor(string ItemID)
        {
            ItemFactory itemFactory = new ItemFactory();
            List<string> Temp = null; 
            XPAdder();
            try
            {
                switch (ItemID)
                {
                    case "60a283193cb70855c43a381d"://THOR IC
                        Temp = THOR_IC;
                        break;
                    case "545cdb794bdc2d3a198b456a"://6B43 6A
                        Temp = _6B43;
                        break;
                    case "5b44cf1486f77431723e3d05"://Gen4 Assault
                        Temp = GEN4_ASSAULT;
                        break;
                    case "5f60c74e3b85f6263c145586"://Rys-T
                        Temp = RYS_T;
                        break;
                    case "5ca20ee186f774799474abc2"://Vulkan-5
                        Temp = Vulkan_5;
                        break;
                    case "5c0e874186f7745dc7616606"://Maska 1Sch
                        Temp = Maska_1Sch;
                        break;
                    case "5c17a7ed2e2216152142459c"://AirFrame Tan
                        Temp = AirFrame_Tan;
                        break;
                    case "5c17a7ed2e2216152142459c-2"://AirFrame Tan-2
                        ItemID = "5c17a7ed2e2216152142459c";
                        Temp = AirFrame_Tan2;
                        break;
                    case "5a154d5cfcdbcb001a3b00da"://Fast MT Black
                        Temp = Fast_MT_Black;
                        break;
                    case "5a16b8a9fcdbcb00165aa6ca"://TATM
                        Temp = TATM;
                        break;
                    case "5c11046cd174af02a012e42b"://W_PVS7
                        Temp = W_PVS7;
                        break;
                    case "5a398b75c4a282000a51a266"://PRA
                        Temp = PRA;
                        break;
                }

                CompoundItem ArmorBase = itemFactory.CreateCompoundItem(ItemID);
                if (Temp != null)
                {
                    for (int i = 0; i < Temp.Count; i++)
                    {
                        if (Temp[i] == "")
                        {
                            continue;
                        }

                        //Item Compo = itemFactory.CreateItem(Temp[i]);
                        CompoundItem Compo = ConstructArmor(Temp[i]);
                        ArmorBase.Slots[i].ChangeContainedItemDirectly(Compo);
                        ArmorBase.Slots[i].ApplyContainedItem();
                    }
                }
                return ArmorBase;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Log: Exception occurred: {ex.Message}");
                return null;
            }
        }

        public CompoundItem SpawnCaseItem(string CoID,string IID)
        {
            ItemFactory itemFactory = new ItemFactory();
            CompoundItem ContainerItem = itemFactory.CreateCompoundItem(CoID);
            for (int i = 0; i < 196; i++)
            {
                Item Ammo = itemFactory.CreateItem(IID);
                ContainerItem.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
            }
            return ContainerItem;
        }
        public void SpawnCase()
        {
            ItemFactory itemFactory = new ItemFactory();
            if (!ContainerOptions.Contains(ContainerID))
            {
                return;
            }
            XPAdder();
            try
            {
                dynamic container = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Backpack).ContainedItem;
                if (!GameUtils.IsInventoryItemValid(container))
                    return;
                CompoundItem ContainerItem = itemFactory.CreateCompoundItem(ContainerID);
                if (!ItemID.IsNullOrEmpty())
                {
                    for (int i = 0; i < 196; i++)
                    {
                        if (ContainerID2.IsNullOrEmpty())
                        {
                            Item Ammo = itemFactory.CreateItem(ItemID);
                            ContainerItem.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
                        }
                        else
                        {
                            CompoundItem Ammo = SpawnCaseItem(ContainerID2, ItemID);
                            ContainerItem.Grids[0].AddAnywhere(Ammo, EErrorHandlingType.Ignore);
                        }
                    }
                }
                container.Grids[0].AddItemWithoutRestrictions(ContainerItem, container.Grids[0].FindFreeSpace(ContainerItem));
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Log: Exception occurred: {ex.Message}");
            }
        }

        public void Spawnitem(int pos)
        {
            ItemFactory itemFactory = new ItemFactory();
            XPAdder();
            try
            {
                string itemId = ItemID;
                Item item = itemFactory.CreateItem(itemId);
                System.Console.WriteLine($"Log: Attempting to create item with ID: {itemId}");
                if (item == null)
                {
                    System.Console.WriteLine("Log: Failed to create item.");
                    return;
                }

                System.Console.WriteLine("Log: Item created successfully.");

                dynamic backpack = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Backpack).ContainedItem;
                dynamic vest = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.TacticalVest).ContainedItem;
                dynamic container = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.SecuredContainer).ContainedItem;
                dynamic pocket = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Pockets).ContainedItem;

                if (!item.QuestItem)
                {
                    if(pos == 1)
                    {
                        if (GameUtils.IsInventoryItemValid(backpack))
                        {
                            backpack.Grids[0].AddAnywhere(item, EErrorHandlingType.Ignore);
                            System.Console.WriteLine("Log: Item created successfully.");
                        }
                        else
                        {
                            Item newBackPackItem = itemFactory.CreateItem("5df8a4d786f77412672a1e3b"); // 6Sh118
                            if (newBackPackItem != null)
                            {
                                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Backpack).Add(newBackPackItem, false, false);
                                dynamic newBackpack = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Backpack).ContainedItem;
                                newBackpack.Grids[0].AddAnywhere(item, EErrorHandlingType.Ignore);
                            }
                        }
                    }

                    if(pos == 2)
                    {
                        if (GameUtils.IsInventoryItemValid(vest))
                        {
                            foreach (dynamic Grid in vest.Grids)
                            {
                                Grid.AddAnywhere(item, EErrorHandlingType.Ignore);
                            }
                            System.Console.WriteLine("Log: Item created successfully.");
                        }
                        else
                        {
                            Item newBackPackItem = itemFactory.CreateItem("5df8a42886f77412640e2e75"); // MPPV
                            if (newBackPackItem != null)
                            {
                                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.TacticalVest).Add(newBackPackItem, false, false);
                                dynamic newBackpack = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.TacticalVest).ContainedItem;
                                foreach(dynamic Grid in newBackpack.Grids)
                                {
                                    Grid.AddAnywhere(item, EErrorHandlingType.Ignore);
                                }
                            }
                        }
                    }

                    if(pos == 3)
                    {
                        if (GameUtils.IsInventoryItemValid(container))
                        {
                            container.Grids[0].AddAnywhere(item, EErrorHandlingType.Ignore);
                            System.Console.WriteLine("Log: Item created successfully.");
                        }
                        else
                        {
                            Item newBackPackItem = itemFactory.CreateItem("5c093ca986f7740a1867ab12"); // Kappa
                            if (newBackPackItem != null)
                            {
                                Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.SecuredContainer).AddWithoutRestrictions(newBackPackItem);
                                //Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.SecuredContainer).Add(newBackPackItem, false, false);
                                dynamic newBackpack = Main.LocalPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.SecuredContainer).ContainedItem;
                                newBackpack.Grids[0].AddAnywhere(item, EErrorHandlingType.Ignore);
                            }
                        }
                    }

                    if (pos == 4)
                    {
                        if (GameUtils.IsInventoryItemValid(pocket))
                        {
                            foreach (dynamic Grid in pocket.Grids)
                            {
                                Grid.AddAnywhere(item, EErrorHandlingType.Ignore);
                            }
                        }
                    }

                }
                else
                {
                    System.Console.WriteLine("Item Is a Quest Item, doing Nothing !");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Log: Exception occurred: {ex.Message}");
            }
        }
        public void XPAdder()
        {
            if (Main.GameWorld = null)
                return;
            Main.LocalPlayer.Profile.EftStats.SessionCounters.SetLong(500, CounterTag.Exp);
        }
        private void GenerateAirdrop()
        {
            if (Main.GameWorld != null)
            {
                //Main.LocalPlayer.HandleFlareSuccessEvent(Main.LocalPlayer.Transform.position, EFT.PrefabSettings.FlareEventType.Airdrop);
            }
        }
        private void GenerateAcidGreen()
        {
            if (Main.GameWorld != null)
            {
                //Main.LocalPlayer.HandleFlareSuccessEvent(Main.LocalPlayer.Transform.position, EFT.PrefabSettings.FlareEventType.AIFollowEvent); ;
            }
        }

        internal static List<string> THOR_IC = new List<string> {            "656fa61e94b480b8a500c0e8",//NESCO 4400            "656fa61e94b480b8a500c0e8",//NESCO 4400            "64afd81707e2cf40e903a316",//Granit侧板            "64afd81707e2cf40e903a316",//Granit侧板            "6575d561b15fef3dd4051670",//            "6575d56b16c2762fba005818",//            "6575d57a16c2762fba00581c",//            "6575d589b15fef3dd4051674",//            "6575d598b15fef3dd4051678",//            "6575d5b316c2762fba005824",//            "6575d5bd16c2762fba005828",//            "6575d5a616c2762fba005820",//
        };

        internal static List<string> _6B43 = new List<string> {            "656fa61e94b480b8a500c0e8",//NESCO 4400            "656fa61e94b480b8a500c0e8",//NESCO 4400            "64afd81707e2cf40e903a316",//Granit侧板            "64afd81707e2cf40e903a316",//Granit侧板            "6575ce3716c2762fba0057fd",//            "6575ce45dc9932aed601c616",//            "6575ce5016c2762fba005802",//            "6575ce5befc786cd9101a671",//            "6575ce6f16c2762fba005806",//            "6575ce9db15fef3dd4051628",//            "6575cea8b15fef3dd405162c",//            "6575ce8bdc9932aed601c61e",//
        };

        internal static List<string> GEN4_ASSAULT = new List<string> {            "656faf0ca0dce000a2020f77",//GAC 4sss2            "656faf0ca0dce000a2020f77",//GAC 4sss2            "64afdb577bb3bfe8fe03fd1d",//ESBI四级侧板            "64afdb577bb3bfe8fe03fd1d",//ESBI四级侧板            "6575c3b3dc9932aed601c5f4",//            "6575c3beefc786cd9101a5ed",//            "6575c3cdc6700bd6b40e8a90",//            "6575c3dfdc9932aed601c5f8",//            "6575c3ec52b7f8c76a05ee39",//            "6575c3fd52b7f8c76a05ee3d",//            "6575c40c52b7f8c76a05ee41",//
        };

        internal static List<string> RYS_T = new List<string> {
            "5f60c85b58eff926626a60f7",//面罩            "657bc285aab96fccee08bea3",//            "657bc2c5a1c61ee0c3036333",//            "657bc2e7b30eca976305118d",//
        };

        internal static List<string> Vulkan_5 = new List<string>
        {            "5ca2113f86f7740b2547e1d2",//面罩            "657bbe73a1c61ee0c303632b",//            "657bbed0aab96fccee08be96",//            "657bbefeb30eca9763051189",//
        };

        internal static List<string> Maska_1Sch = new List<string>
        {
            "5c0e842486f77443a74d2976",//1Sch面罩            "6571133d22996eaf11088200",//            "6571138e818110db4600aa71",//            "657112fa818110db4600aa6b",//
        };

        internal static List<string> AirFrame_Tan = new List<string>
        {            "5a16b7e1fcdbcb00165aa6c9",//面罩            "5a16b8a9fcdbcb00165aa6ca",//TATM            "5a398b75c4a282000a51a266",//PRA            "5c178a942e22164bef5ceca3",//Chops            "657f9897f4c82973640b235e",//            "657f98fbada5fadd1f07a585",//
        }; 
        internal static List<string> AirFrame_Tan2 = new List<string>
        {            "5a16b7e1fcdbcb00165aa6c9",//面罩            "5c0558060db834001b735271",//GPNVG-18            "",//            "5c178a942e22164bef5ceca3",//Chops            "657f9897f4c82973640b235e",//            "657f98fbada5fadd1f07a585",//
        };

        internal static List<string> Fast_MT_Black = new List<string>
        {            "",//            "5ea058e01dbce517f324b3e2",//Heavy Trooper            "",//            "",//            "",//            "657f8ec5f4c82973640b234c",//            "657f8f10f4c82973640b2350",//
        };

        internal static List<string> TATM = new List<string>
        {            "5c11046cd174af02a012e42b",//W-PVS7
        };
        internal static List<string> W_PVS7 = new List<string>
        {            "5c110624d174af029e69734c",//T-7
        };
        internal static List<string> PRA = new List<string>
        {            "6272370ee4013c5d7e31f418",//BaldrPro
        };

        public void WaterAndFood()
        {
            if (Main.LocalPlayer != null)
            {
                Main.LocalPlayer.ActiveHealthController.ChangeHydration(100f);
                Main.LocalPlayer.ActiveHealthController.ChangeEnergy(110f);
            }
        }
    }
}
