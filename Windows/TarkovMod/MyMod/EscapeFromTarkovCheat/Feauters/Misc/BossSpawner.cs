using System;
using EFT;
using Comfort.Common;
using UnityEngine;
 
namespace EscapeFromTarkovCheat.Features
{
    public class BossSpawner
    {
        public void SpawnBoss(WildSpawnType bossType)
        {
            var botGame = Singleton<IBotGame>.Instance;

            if (botGame == null)
            {
                return;
            }

/*            botGame.BotsController.SpawnBotDebugServer(
                EPlayerSide.Savage,        // Side of the bot (Savage/Scav)
                false,                     // Not a player scav
                bossType,                  // Type of bot to spawn
                BotDifficulty.normal,      // Difficulty level of the bot
                true                       // I smoke crack for fun :)
            );*/

        }
        public void SpawnBear()
        {
            var botGame = Singleton<IBotGame>.Instance;

            if (botGame == null)
            {
                return;
            }

/*            botGame.BotsController.SpawnBotDebugServer(
                EPlayerSide.Bear,
                false,
                WildSpawnType.pmcBEAR,
                BotDifficulty.normal,
                true
            );*/

        }

        public void SpawnUsec()
        {
            var botGame = Singleton<IBotGame>.Instance;

            if (botGame == null)
            {
                return;
            }

/*            botGame.BotsController.SpawnBotDebugServer(
                EPlayerSide.Usec,
                false,
                WildSpawnType.pmcUSEC,
                BotDifficulty.normal,
                true
            );*/

        }

        public void SpawnRandomBoss()
        {
            var botGame = Singleton<IBotGame>.Instance;

            WildSpawnType bossType = WildSpawnType.bossKilla;

            if (botGame == null)
            {
                return;
            }

            int randomNumber = UnityEngine.Random.Range(1, 9);

            switch (randomNumber)
            {
                case 1:
                    bossType = WildSpawnType.bossKilla;
                    break;
                case 2:
                    bossType = WildSpawnType.bossKojaniy;
                    break;
                case 3:
                    bossType = WildSpawnType.bossGluhar;
                    break;
                case 4:
                    bossType = WildSpawnType.bossTagilla;
                    break;
                case 5:
                    bossType = WildSpawnType.bossKnight;
                    break;
                case 6:
                    bossType = WildSpawnType.followerBigPipe;
                    break;
                case 7:
                    bossType = WildSpawnType.followerBirdEye;
                    break;
                case 8:
                    bossType = WildSpawnType.bossBoar;
                    break;
                default:
                    bossType = WildSpawnType.bossKilla;
                    break;
            }

/*            botGame.BotsController.SpawnBotDebugServer(
                EPlayerSide.Savage,        // Side of the bot (Savage/Scav)
                false,                     // Not a player scav
                bossType,                  // Type of bot to spawn
                BotDifficulty.normal,      // Difficulty level of the bot
                true                       // I smoke crack for fun :)
            );*/

        }
        public void SpawnRandomPMC()
        {
            int randomNumber = UnityEngine.Random.Range(0, 5);
            int randomNumber2 = UnityEngine.Random.Range(2, 5);

            switch (randomNumber)
            {
                case 0:
                case 1:
                    for (int i = 0; i < randomNumber2; i++)
                    {
                        SpawnUsec();
                    }
                    return;
                case 2:
                case 3:
                    for (int i = 0; i < randomNumber2; i++)
                    {
                        SpawnBear();
                    }
                    return;
                case 4:
                    for (int i = 0; i < randomNumber2; i++)
                    {
                        SpawnBear();
                        SpawnUsec();
                    }
                    return;
            }
        }

        public void SpawnRandom()
        {
            var botGame = Singleton<IBotGame>.Instance;

            WildSpawnType bossType = WildSpawnType.bossKilla;

            if (botGame == null)
            {
                return;
            }

            int randomNumber = UnityEngine.Random.Range(1, 16);
            int randomNumber2 = UnityEngine.Random.Range(2, 5);

            switch (randomNumber)
            {
                case 1:
                    bossType = WildSpawnType.bossKilla;
                    break;
                case 2:
                    bossType = WildSpawnType.bossKojaniy;
                    break;
                case 3:
                    bossType = WildSpawnType.bossGluhar;
                    break;
                case 4:
                    bossType = WildSpawnType.bossTagilla;
                    break;
                case 5:
                    bossType = WildSpawnType.bossKnight;
                    break;
                case 6:
                    bossType = WildSpawnType.followerBigPipe;
                    break;
                case 7:
                    bossType = WildSpawnType.followerBirdEye;
                    break;
                case 8:
                    bossType = WildSpawnType.bossBoar;
                    break;
                case 9:
                    bossType = WildSpawnType.bossKilla;
                    break;
                case 10:
                case 11:
                    for (int i = 0; i < randomNumber2; i++)
                    {
                        SpawnUsec();
                    }
                    return;
                case 12:
                case 13:
                    for (int i = 0; i < randomNumber2; i++)
                    {
                        SpawnBear();
                    }
                    return;
                case 14:
                    for (int i = 0; i < randomNumber2; i++)
                    {
                        SpawnBear();
                        SpawnUsec();
                    }
                    return;
            }

/*            botGame.BotsController.SpawnBotDebugServer(
                EPlayerSide.Savage,        // Side of the bot (Savage/Scav)
                false,                     // Not a player scav
                bossType,                  // Type of bot to spawn
                BotDifficulty.normal,      // Difficulty level of the bot
                true                       // I smoke crack for fun :)
            );*/

        }
    }
}
