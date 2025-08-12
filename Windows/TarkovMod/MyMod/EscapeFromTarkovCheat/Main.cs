using System;
using System.Collections.Generic;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using EFT.InputSystem;
using EscapeFromTarkovCheat.Data;
using EscapeFromTarkovCheat.Feauters;
using EscapeFromTarkovCheat.Feauters.ESP;
using EscapeFromTarkovCheat.Utils;
using UnityEngine;
using System.Linq;
using EscapeFromTarkovCheat.Feauters.Misc;
using EFT.InventoryLogic;
using EscapeFromTarkovCheat.Features;
using EscapeFromTarkovCheat;
namespace EscapeFromTarkovCheat
{
    public class Main : MonoBehaviour
    {
        public static List<GamePlayer> GamePlayers = new List<GamePlayer>();
        public static List<Player> Players { get; private set; } = new List<Player>();
        public static List<Throwable> Grenades { get; private set; } = new List<Throwable>();

        public static Player LocalPlayer;
        public static GameWorld GameWorld;
        public static Camera MainCamera;
        public static BossSpawner bossSpawner;
        private Quest questFinisher;
        private int _maxDuration = 120;

        private float _nextPlayerCacheTime;
        private float _nextMapCacheTime;
        private float _nextSpawnTime;


        public void Awake()
        {

            GameObject hookObject = new GameObject();
            bossSpawner = new BossSpawner();
            this.questFinisher = new Quest();
            GlobalHook.Initialize(FindObjectOfType<TarkovApplication>());
            hookObject.AddComponent<Menu.UI.Menu>();
            hookObject.AddComponent<PlayerESP>();
            hookObject.AddComponent<ItemESP>();
            hookObject.AddComponent<LootableContainerESP>();
            hookObject.AddComponent<ExfiltrationPointsESP>();
            hookObject.AddComponent<HP_Stamina_Speed>();
            DontDestroyOnLoad(hookObject);
            _nextMapCacheTime = Time.time;
            _nextSpawnTime = Time.time;

        }

        public void Update()
        {

            if (Settings.DrawPlayers || Settings.DrawPlayerLoots || Settings.PlayerCache)
            {
                if (Time.time >= _nextPlayerCacheTime)
                {
                    GameWorld = Singleton<GameWorld>.Instance;
                    MainCamera = Camera.main;
                    if ((GameWorld != null) && (GameWorld.RegisteredPlayers != null))
                    {
                        GamePlayers.Clear();
                        
                        foreach (Player player in GameWorld.RegisteredPlayers)
                        {
                            if (player.IsYourPlayer)
                            {
                                LocalPlayer = player;
                                continue;
                            }

                            if (!GameUtils.IsPlayerAlive(player) || (Vector3.Distance(MainCamera.transform.position, player.Transform.position) > Settings.DrawPlayersDistance))
                                continue;

                            GamePlayers.Add(new GamePlayer(player));
                            
                        }
                        if (Settings.DrawPlayerLoots)
                        {
                            PlayerESP.PlayerItemsCombos.Clear();
                            SetPlayerContainer();
                        }

                        _nextPlayerCacheTime = (Time.time + Settings.PlayerCacheInterval);
                    }
                }

                foreach (GamePlayer gamePlayer in GamePlayers)
                    gamePlayer.RecalculateDynamics();
            }

            if (Main.MainCamera != null && Main.MainCamera.GetComponent<ThermalVision>() != null)
            {
                Main.MainCamera.GetComponent<ThermalVision>().IsNoisy = false;
                Main.MainCamera.GetComponent<ThermalVision>().IsFpsStuck = false;
                Main.MainCamera.GetComponent<ThermalVision>().IsMotionBlurred = false;
                Main.MainCamera.GetComponent<ThermalVision>().IsPixelated = false;
                Main.MainCamera.GetComponent<ThermalVision>().IsGlitch = false;
                Main.MainCamera.GetComponent<ThermalVision>().ChromaticAberrationThermalShift = 0f;
                Main.MainCamera.GetComponent<ThermalVision>().On = Settings.ForceThermal;
            }

            if (Input.GetKeyDown(Settings.UnlockDoors))
            {
                foreach (var door in FindObjectsOfType<Door>())
                {
                    if (door.DoorState == EDoorState.Open || Vector3.Distance(door.transform.position, LocalPlayer.Position) > 20f)
                        continue;

                    door.DoorState = EDoorState.Shut;
                }
            }

            if (Input.GetKeyDown(Settings.KillAll))
            {
                var gameWorld = Singleton<GameWorld>.Instance;
                if (gameWorld != null)
                {
                    IEnumerable<Player> players = gameWorld.AllAlivePlayersList.Where(x => !x.IsYourPlayer);
                    foreach (Player player in players)
                    {

                        if (!player.IsYourPlayer)
                        {

                            player.ActiveHealthController.Kill(EDamageType.Landmine);
                        }
                    }
                }
            }

            if (Input.GetKeyDown(Settings.TeleportAllEnemies))
            {
                TeleportAllEnemies();
            }
            
            if (Input.GetKeyDown(Settings.TeleportMarkedEnemies))
            {
                TeleportMarkedEnemies();
            }

            if (Input.GetKeyDown(Settings.TeleportLootItem))
            {
                TeleportLootAll();
            }

            if (Input.GetKeyDown(Settings.TeleportWishList))
            {
                TeleportLootWishList();
            }

            if (Input.GetKeyDown(Settings.ThermalToggle))
            {
                Settings.ForceThermal = !Settings.ForceThermal;
            }

            if (Input.GetKeyDown(Settings.SpeedHackKey))
            {
                Settings.SpeedHack = !Settings.SpeedHack;
            }

            if (Time.time >= _nextMapCacheTime)
            {
                if (GlobalHook.tarkovApplication != null)
                {
                    var session = GlobalHook.tarkovApplication.GetClientBackEndSession();
                    if (session != null)
                    {
                        if (session.LocationSettings != null)
                        {
                            foreach (var location in session.LocationSettings.locations.Values)
                            {
                                location.ForceOnlineRaidInPVE = false;
                                location.session_duration_minutes = _maxDuration;
                                location.EscapeTimeLimit = _maxDuration;
                                location.EscapeTimeLimitCoop = _maxDuration;
                                location.exit_time = _maxDuration;
                                location.IsSecret = false;
                                location.Locked = false;
                            }
                            Settings.OfflineStreetStatus = "On";
                            _nextMapCacheTime = (Time.time + 8f);
                        }
                    }
                }
            }
           

            if (Settings.FinishQuest)
            {
                questFinisher.FinishQuests();
                Settings.FinishQuest = false;
            }

            if (Time.time > _nextSpawnTime && (Settings.BloodBathModeBoss || Settings.BloodBathModePMC))
            {

                if (Settings.BloodBathModeBoss && Settings.BloodBathModePMC)
                {
                    for (int i = 0; i < Settings.SpawnCount; i++)
                    {
                        bossSpawner.SpawnRandom();
                    }
                }
                else if (Settings.BloodBathModeBoss)
                {
                    for (int i = 0; i < Settings.SpawnCount; i++)
                    {
                        bossSpawner.SpawnRandomBoss();
                    }
                }
                else if (Settings.BloodBathModePMC)
                {
                    for (int i = 0; i < Settings.SpawnCount; i++)
                    {
                        bossSpawner.SpawnRandomPMC();
                    }
                }
                _nextSpawnTime = Time.time + Settings.SpawnInterval * 10f;
            }

/*            if (Settings.IncreaseTraderStanding)
            {
                Player localPlayer = Main.LocalPlayer;
                if (localPlayer != null)
                {
                    System.Console.WriteLine("=====Setting Traders Standing=====");
                    foreach (KeyValuePair<string, Profile.TraderInfo> keyValuePair in localPlayer.Profile.TradersInfo)
                    {
                        string text = keyValuePair.Key;
                        Profile.TraderInfo traderInfo = keyValuePair.Value;
                        if (Settings.TraderList.Contains(text) && traderInfo.Standing < 2)
                        {
                            System.Console.WriteLine("Setting Standing for Trader:" + text);
                            traderInfo.Init(text, localPlayer.Profile.Info);
                            double standing = 2.50;
                            traderInfo.SetStanding(standing);
                        }
                        if (text == "656f0f98d80a697f855d34b1")
                        {
                            System.Console.WriteLine("BTR:" + traderInfo.Standing);
                        }
                        else if (text == "579dc571d53a0658a154fbec")
                        {
                            System.Console.WriteLine("Fence:" + traderInfo.Standing);
                        }
                        else
                        {
                            System.Console.WriteLine("Trader Name:" + text);

                        }
                        System.Console.WriteLine("Current Standing:" + traderInfo.Standing);
                    }
                    Settings.IncreaseTraderStanding = false;
                }
            }*/

            //if (LocalPlayer != null)
            //{
            //    LocalPlayer.Skills.Immunity.SetLevel(51);
            //}
        }
        private void TeleportAllEnemies()
        {
            var targetPosition = GetTargetPosition();
            var gameWorld = Singleton<GameWorld>.Instance;

            if (gameWorld != null)
            {
                foreach (var player in gameWorld.AllAlivePlayersList.Where(x => !x.IsYourPlayer))
                {
                    if (player.Profile.Info.Settings.Role == WildSpawnType.bossZryachiy || player.Profile.Info.Settings.Role == WildSpawnType.followerZryachiy)
                    {
                        continue;
                    }
                    player.Teleport(targetPosition);
                }
            }
        }
        
        private void TeleportMarkedEnemies()
        {
            var targetPosition = GetTargetPosition();
            var gameWorld = Singleton<GameWorld>.Instance;

            if (gameWorld != null)
            {
                foreach (PlayerAndItem pai in PlayerESP.PlayerItemsCombos)
                {
                    if (pai.PlayerRef.Player.Profile.Info.Settings.Role == WildSpawnType.bossZryachiy || pai.PlayerRef.Player.Profile.Info.Settings.Role == WildSpawnType.followerZryachiy)
                    {
                        continue;
                    }
                    pai.PlayerRef.Player.Teleport(targetPosition);
                }
            }
        }

        private void TeleportLootWishList()
        {
            var targetPosition = GetTargetPosition();
            var lootItems = FindObjectsOfType<LootItem>()
                .Where(IsGroundLoot)
                .Where(IsInteractableLoot)
                .Where(lootItem => CheckWishList(lootItem.Item))
                .Select(lootItem => new GameLootItem(lootItem))
                .ToList();

            int itemCount = lootItems.Count;
            float radius = 1.2f;
            float additionalSpacing = 0.5f;

            for (int i = 0; i < itemCount; i++)
            {
                var angle = i * Mathf.PI * 2 / 60;
                var x = Mathf.Cos(angle) * (radius + (i * additionalSpacing / 60));
                var z = Mathf.Sin(angle) * (radius + (i * additionalSpacing / 60));

                Vector3 position = new Vector3(targetPosition.x + x, targetPosition.y + 0.5f, targetPosition.z + z);
                lootItems[i].LootItem.transform.position = position;
            }
        }

        private void TeleportLootAll()
        {
            var targetPosition = GetTargetPosition();
            var lootItems = FindObjectsOfType<LootItem>()
                .Where(IsGroundLoot)
                .Where(IsInteractableLoot)
                .Where(lootItem => CheckLootItem(lootItem.Item,true))
                .Select(lootItem => new GameLootItem(lootItem))
                .ToList();

            int itemCount = lootItems.Count;
            float radius = 1.5f;
            float additionalSpacing = 0.6f;

            for (int i = 0; i < itemCount; i++)
            {
                var angle = i * Mathf.PI * 2 / 80;
                var x = Mathf.Cos(angle) * (radius + (i * additionalSpacing / 80));
                var z = Mathf.Sin(angle) * (radius + (i * additionalSpacing / 80));

                Vector3 position = new Vector3(targetPosition.x + x, targetPosition.y + 0.5f, targetPosition.z + z);
                lootItems[i].LootItem.transform.position = position;
            }
        }


        private Vector3 GetTargetPosition()
        {
            var centerPosition = LocalPlayer.Transform.position;

            if (LocalPlayer.HandsController is Player.FirearmController firearmController)
            {
                if (Physics.Raycast(new Ray(firearmController.Fireport.position, firearmController.WeaponDirection), out RaycastHit hit, float.MaxValue))
                {
                    centerPosition = hit.point;
                }
            }

            return centerPosition;
        }

        private bool IsGroundLoot(LootItem lootItem)
        {
            return lootItem.GetComponent<Corpse>() == null;
        }

        private bool IsInteractableLoot(LootItem lootItem)
        {
            var collider = lootItem.GetComponent<Collider>();
            return collider != null && collider.enabled;
        }

        public static bool CheckWishList(Item item)
        {

            if (Settings.ValuableToggle)
            {
                if (Settings.Filter_Keycard_Container.Contains(item.Template._id))
                    return false;
                if (Settings.Category_Keycard_Container.Contains(item.Template.ParentId))
                    return true;
                if (Settings.Valuable.Contains(item.Template._id))
                    return true;
            }

            if (Settings.WishListAmmoToggle && Settings.Ammo.Contains(item.Template._id))
                return true;
            if (Settings.WishListGearPlateToggle && Settings.Gear_Armor_Plate.Contains(item.Template._id))
                return true;
            if (Settings.WishListConsumableToggle && Settings.Consumable.Contains(item.Template._id))
                return true;
            if (Settings.TemporaryItemListToggle && Settings.TemporaryItemList.Contains(item.Template._id))
                return true;
            return false;
        }

        public static bool CheckPCItem(Item item)
        {
            if (Settings.ValuableToggle)
            {
                if (Settings.LootPC_SingleFilter.Contains(item.Template._id))
                    return false;
                if (Settings.Filter_Keycard_Container.Contains(item.Template._id))
                    return false;
                if (Settings.Category_Keycard_Container.Contains(item.Template.ParentId))
                    return true;
                if (Settings.Valuable.Contains(item.Template._id))
                    return true;
            }
            if (Settings.CheckPCItemAmmoToggle && Settings.Ammo.Contains(item.Template._id))
                return true;
            if (Settings.CheckPCItemGearPlateToggle && Settings.Gear_Armor_Plate.Contains(item.Template._id))
                return true;
            if (Settings.TemporaryItemListToggle && Settings.TemporaryItemList.Contains(item.Template._id))
                return true;
            return false;
        }

        public static bool CheckLootItem(Item item, bool check)
        {
            if (check)
                if(CheckWishList(item))
                    return false;
            if (Settings.LootItemList.Contains(item.Template._id))
                return true;
            if (Settings.LootItem_SingleFilter.Contains(item.Template._id))
                return false;
            if (Settings.LootItem_WeaponFilter.Contains(item.Template._id))
                return false;
            if (Settings.LootItem_CategoryFilter.Contains(item.Template.ParentId))
                return false;
            if (item.Template.Parent != null && Settings.LootItem_CategoryFilter.Contains(item.Template.Parent.ParentId))
                return false;
            if (item.Template.Parent.Parent != null && Settings.LootItem_CategoryFilter.Contains(item.Template.Parent.Parent.ParentId))
                return false;
            return true;
        }

        
        private void SetPlayerContainer()
        {
            if (Settings.LootMode && Main.GamePlayers.Count < 4)
            {
                if (Settings.LootGroup1)
                {

                }
                if (Settings.LootGroup2)
                {

                }
                if (Settings.LootGroup3)
                {

                }
                if (Settings.LootGroup4)
                {

                }
            }
            foreach (GamePlayer player in Main.GamePlayers)
            {
                IEnumerable<Item> BackPackItems = player.Player.Profile.Inventory.GetItemsInSlots(new EquipmentSlot[]
                {
                    EquipmentSlot.Backpack
                });
                IEnumerable<Item> ArmorVestItems = player.Player.Profile.Inventory.GetItemsInSlots(new EquipmentSlot[]
                {
                    EquipmentSlot.ArmorVest
                });
                IEnumerable<Item> VestItems = player.Player.Profile.Inventory.GetItemsInSlots(new EquipmentSlot[]
                {
                    EquipmentSlot.TacticalVest
                });
                IEnumerable<Item> PocketsItems = player.Player.Profile.Inventory.GetItemsInSlots(new EquipmentSlot[]
                {
                    EquipmentSlot.Pockets
                });
                IEnumerable<Item> FaceCoverItems = player.Player.Profile.Inventory.GetItemsInSlots(new EquipmentSlot[]
                {
                    EquipmentSlot.FaceCover
                });
                IEnumerable<Item> HeadwearItems = player.Player.Profile.Inventory.GetItemsInSlots(new EquipmentSlot[]
                {
                    EquipmentSlot.Headwear
                });
                IEnumerable<Item> FirstPrimaryWeaponItems = player.Player.Profile.Inventory.GetItemsInSlots(new EquipmentSlot[]
                {
                    EquipmentSlot.FirstPrimaryWeapon
                });
                IEnumerable<Item> SecondPrimaryWeaponItems = player.Player.Profile.Inventory.GetItemsInSlots(new EquipmentSlot[]
                {
                    EquipmentSlot.SecondPrimaryWeapon
                });

                IEnumerable<Item> AllItems = new List<Item>();
                if (BackPackItems != null)
                {
                    AllItems = AllItems.Concat(BackPackItems);
                }
                if (ArmorVestItems != null)
                {
                    AllItems = AllItems.Concat(ArmorVestItems);
                }
                if (VestItems != null)
                {
                    AllItems = AllItems.Concat(VestItems);
                }
                if (PocketsItems != null)
                {
                    AllItems = AllItems.Concat(PocketsItems);
                }
                if (FaceCoverItems != null)
                {
                    AllItems = AllItems.Concat(FaceCoverItems);
                }
                if (HeadwearItems != null)
                {
                    AllItems = AllItems.Concat(HeadwearItems);
                }
                if (FirstPrimaryWeaponItems != null)
                {
                    AllItems = AllItems.Concat(FirstPrimaryWeaponItems);
                }
                if (SecondPrimaryWeaponItems != null)
                {
                    AllItems = AllItems.Concat(SecondPrimaryWeaponItems);
                }
                List<string> NameList = new List<string>();
                using (IEnumerator<Item> enumerator3 = AllItems.GetEnumerator())
                {
                    while (enumerator3.MoveNext())
                    {
                        Item item = enumerator3.Current;
                        if (item != null && item.Template != null && !string.IsNullOrEmpty(item.ShortName.Localized()))
                        {
                            if (Main.CheckPCItem(item))
                            {
                                NameList.Add(item.ShortName.Localized());
                            }
                        }
                    }
                }
                if (NameList.Count > 0)
                {
                    NameList = NameList.Distinct().ToList();
                    PlayerAndItem temp = new PlayerAndItem(player, NameList);
                    PlayerESP.PlayerItemsCombos.Add(temp);
                    if (Settings.LootMode)
                    {
                        player.Player.Teleport(GetTargetPosition());
                        player.Player.ActiveHealthController.Kill(EDamageType.Landmine);
                    }

                }
                if (Settings.LootMode)
                {
                    if(Settings.DogTagLV60 && (player.Player.Profile.Info.Settings.Role == WildSpawnType.pmcUSEC || player.Player.Profile.Info.Settings.Role == WildSpawnType.pmcBEAR) && player.Player.Profile.Info.Level >= 60){
                        player.Player.Teleport(GetTargetPosition());
                    }
                    else
                    {
                        player.Player.ActiveHealthController.Kill(EDamageType.Landmine);
                    }
                }
            }
        }

    }
}
