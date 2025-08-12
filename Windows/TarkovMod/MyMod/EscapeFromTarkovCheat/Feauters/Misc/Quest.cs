using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Comfort.Common;
using EFT.Quests;
using EFT;
using EscapeFromTarkovCheat.Utils;
using System.Reflection;

namespace EscapeFromTarkovCheat.Feauters
{
    class Quest
    {
        public async void FinishQuests()
        {
            System.Console.WriteLine($"====Start=====");
            var player = Main.LocalPlayer;

            if (player == null)
            {
                return;
            }

            var session = GlobalHook.tarkovApplication.GetClientBackEndSession();

            if (session == null)
            {
                System.Console.WriteLine("Session is null");
                return;
            }

            var runtimeFields = typeof(Player).GetRuntimeFields();
            var field = runtimeFields.FirstOrDefault(x => x.Name == "_questController");
            dynamic questController = field.GetValue(player);
            dynamic quests = questController.Quests;

            foreach (var quest in quests)
            {
                if (quest == null)
                {
                    continue;
                }

                if (quest.QuestStatus != EQuestStatus.Success)
                {
                    System.Console.WriteLine($"===============================");
                    System.Console.WriteLine($"Quest Name: {quest.Template.Name}");
                    System.Console.WriteLine($"Status Num: {quest.QuestStatus}");
                    System.Console.WriteLine($"===============================");
                }

                if (quest.QuestStatus == EQuestStatus.AvailableAfter)
                {
                    System.Console.WriteLine($"AvailableAfter - Quest Name: {quest.Template.Name}");
                    if (quest.Template.Name == "Quest Name Goes Here")
                    {
                        System.Console.WriteLine($"Unlocking: {quest.Template.Name}");
                        quest.SetStatus(EQuestStatus.AvailableForStart, true, true);
                    }
                    continue;
                }

                if (quest.QuestStatus == EQuestStatus.Success)
                {
                    continue;
                }

                if (quest.QuestStatus == EQuestStatus.Locked)
                {
                    System.Console.WriteLine($"Locked: {quest.Template.Name}");
                    if (quest.Template.Name == "Quest Name Goes Here")
                    {
                        System.Console.WriteLine($"Unlocking: {quest.Template.Name}");
                        quest.SetStatus(EQuestStatus.AvailableForStart, true, true);
                    }
                    continue;
                }

                if (quest.QuestStatus == EQuestStatus.AvailableForStart)
                {
                    System.Console.WriteLine($"AvailableForStart: {quest.Template.Name}");
                    continue;
                }

                if (quest.QuestStatus != EQuestStatus.Started)
                {
                    System.Console.WriteLine($"Last Check: {quest.Template.Name}");
                    continue;
                }

                System.Console.WriteLine($"Status:{quest.QuestStatus} : {quest.Template.Name}");

                if (quest.Template == null)
                {
                    continue;
                }

                if (quest.Template.Name == "XXXXXXXXXXXXXXXXXXX")
                {
                    System.Console.WriteLine($"Trying to finish quest: {quest.Template.Name}");
                    foreach (var conditions in quest.Template.Conditions)
                    {
                        System.Console.WriteLine("===== " + conditions.Key + " ======");
                        foreach (var condition in conditions.Value)
                        {
                            System.Console.WriteLine("===== " + condition + " ======");
                            quest.CompletedConditions.Add(condition.id);
                        }
                    }
                    try
                    {
                        var tries = 0;
                        while (true)
                        {
                            quest.SetStatus(EQuestStatus.AvailableForFinish, true, true);
                            quest.SetStatus(EQuestStatus.Success, true, true);
                            System.Console.WriteLine("Sending Network Request");
                            questController.FinishQuest(quest, true);
                            IResult result = await session.QuestComplete(quest.Template.Id, false, false);
 
                            if (result.Succeed || tries > 15)
                            {
                                break;
                            }
                            tries++;
                            Thread.Sleep(2500);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex.ToString());
                    }
                }


                await Task.Delay(2500);
            }
            System.Console.WriteLine($"=====Done=====");
        }
    }
}
