using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using EscapeFromTarkovCheat.Data;
using EscapeFromTarkovCheat.Utils;
using JsonType;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EscapeFromTarkovCheat.Feauters.ESP
{
    public class LootableContainerESP : MonoBehaviour
    {
        private static readonly float CacheLootItemsInterval = 60;
        private float _nextLootContainerCacheTime;
        private static readonly Color LootableContainerColor = new Color(1f, 0.6f, 0f);

        private List<ContainerGroup> _containerGroups;
        
        private float _groupingDistance = 0f;

        public void Start()
        {
            _containerGroups = new List<ContainerGroup>();
        }

        public void Update()
        {
            if (!Settings.DrawLootableContainers)
                return;

            if (Time.time >= this._nextLootContainerCacheTime && Main.GameWorld != null && Main.GameWorld.LootItems != null)
            {
                _containerGroups.Clear();
                SetLootableContainers();
                _nextLootContainerCacheTime = (Time.time + CacheLootItemsInterval);
            }

            foreach (ContainerGroup a in _containerGroups)
            {
	            foreach (GameLootContainer gameLootContainer in a.Containers)
		            gameLootContainer.RecalculateDynamics();
            }
        }

        private void SetLootableContainers()
        {
            try
            {
                LootableContainer[] array = FindObjectsOfType<LootableContainer>();
                if (array != null)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        LootableContainer current = array[i];
                        if (!(current == null) && !(Main.MainCamera == null) && GameUtils.IsLootableContainerValid(current)){
                            Assembly assembly = Assembly.Load("Assembly-CSharp");
                            //public static IEnumerable<Item> GetAllItems
                            MethodInfo method = assembly.GetType("\uEEEF",true).GetMethod("GetAllItems",BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(Item) }, null);
                            IEnumerable<Item> ItemsList = (IEnumerable<Item>)method.Invoke(null, new object[] { current.ItemOwner.RootItem });
                            float distanceTo = Vector3.Distance(Main.MainCamera.transform.position, current.transform.position);
                            bool? flag = null;
                            List<string> NameList = new List<string> { };
                            if (current.ItemOwner != null && current.ItemOwner.RootItem != null)// && distanceTo <= Settings.DrawLootableContainersDistance)
                            {
                                    using (IEnumerator<Item> enumerator3 = ItemsList.Skip(1).GetEnumerator())
                                    {
	                                    while (enumerator3.MoveNext())
	                                    {
		                                    Item item = enumerator3.Current;
		                                    if (item != null && item.Template != null && !string.IsNullOrEmpty(item.ShortName.Localized()))
		                                    {
			                                    if (Main.CheckPCItem(item))
			                                    {
				                                    flag = true;
				                                    NameList.Add(item.ShortName.Localized());
			                                    }
		                                    }
	                                    }
                                    }
                            }
                            if (flag.GetValueOrDefault())
                            {
                                GameLootContainer currentContainer = new GameLootContainer(current);
                                Item itemTemp = null;
                                string containerName = "Unknown Container";
                                if (current.ItemOwner != null && current.ItemOwner.RootItem != null)
                                {
	                                itemTemp = ((ItemsList != null) ? ItemsList.FirstOrDefault<Item>() : null);
                                }
                                if (itemTemp != null)
                                {
	                                containerName = (((itemTemp.ShortName != null) ? itemTemp.ShortName.Localized() : null) ?? "Unknown Container");
                                }
                                ContainerGroup containerGroup = _containerGroups.FirstOrDefault((ContainerGroup g) => g.ContainerName == containerName && Vector3.Distance(g.Position, current.transform.position) < _groupingDistance);
                                if (containerGroup == null)
                                {
                                    containerGroup = new ContainerGroup(containerName, current.transform.position);
                                    _containerGroups.Add(containerGroup);
                                }
                                containerGroup.NameList.AddRange(NameList);
                                containerGroup.NameList = containerGroup.NameList.Distinct().ToList();
                                containerGroup.Containers.Add(currentContainer);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in SetLootableContainers: " + ex.Message + "\n" + ex.StackTrace);
            }
        }
	    public void OnGUI()
        {
            if (!Settings.DrawLootableContainers)
                return;
            foreach (ContainerGroup containerGroup in _containerGroups)
			{
				GameLootContainer gameLootContainer = containerGroup.Containers[0];
				if (!gameLootContainer.IsOnScreen ||
				    gameLootContainer.Distance > Settings.DrawLootableContainersDistance)
				{
					continue;
				}
				else{
					string text = containerGroup.ContainerName;
					if (containerGroup.Containers.Count > 1)
					{
						text += string.Format(" (x{0})", containerGroup.Containers.Count);
					}
					text = text + " [" + gameLootContainer.FormattedDistance + "]";
					Render.DrawString(new Vector2(gameLootContainer.ScreenPosition.x - 50f, gameLootContainer.ScreenPosition.y), text, LootableContainerESP.LootableContainerColor, true);
					float num = -15f;
					foreach (string itemName in containerGroup.NameList)
					{
						string label2 = string.Format("{0}", itemName);
						Render.DrawString(new Vector2(gameLootContainer.ScreenPosition.x - 50f, gameLootContainer.ScreenPosition.y + num), label2, LootableContainerESP.LootableContainerColor, true);
						num -= 15f;
					}
				}
			}
        }
    }
}