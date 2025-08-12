using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using EscapeFromTarkovCheat.Utils;
using UnityEngine;

namespace EscapeFromTarkovCheat.Feauters.Misc
{
    public class HP_Stamina_Speed : MonoBehaviour
    {

        private float _nextRegenTime;
        private float _nextBuffTime;
        public EBodyPart[] BodyPartList = new EBodyPart[]{
            EBodyPart.Head,
            EBodyPart.Chest,
            EBodyPart.Stomach,
            EBodyPart.LeftArm,
            EBodyPart.RightArm,
            EBodyPart.LeftLeg,
            EBodyPart.RightLeg,
        };
        dynamic Field;

        public void Start()
        {
            Assembly assembly = Assembly.Load("Assembly-CSharp");
            //public static readonly \uXXXX Existence;
            Field = assembly.GetType("\uED21", true).GetField("Existence", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).GetValue(null);
            _nextRegenTime = Time.time;
            _nextBuffTime = Time.time;
        }
        public void Update()
        {
            if (Main.LocalPlayer != null && Settings.GodMode)
            {
                this.God();
            }
            else if(Main.LocalPlayer != null && Settings.HalfGodMode)
            {
                this.HalfGod();
            }
            if (Main.GameWorld != null && Settings.SpeedHack)
            {
                this.SpeedMod();
            }
            //技能效果
            if(Main.LocalPlayer != null && (Settings.GodMode || Settings.HalfGodMode))
            {
                if (Time.time > _nextBuffTime)
                {

                    Main.LocalPlayer.ActiveHealthController.DoPermanentHealthBoost(0.05f * Settings.BaseHealPerSec);
                    Main.LocalPlayer.Skills.AttentionLootSpeed.Value = 20f;//搜刮速度
                    Main.LocalPlayer.Skills.SurgerySpeed.Value = 0.75f;//手术速度
                    Main.LocalPlayer.Skills.DrawSpeed.Value = 0.8f;//开镜速度
                    Main.LocalPlayer.Skills.StrengthBuffLiftWeightInc.Value = 12f;//负重 0.3 = 100kg 每额外1.3 = 100kg
                    Main.LocalPlayer.Skills.HeavyVestMoveSpeedPenaltyReduction.Value = 1f;//重甲减速缓解
                    Main.LocalPlayer.Skills.LightVestMoveSpeedPenaltyReduction.Value = 1f;//轻甲减速缓解
                    Main.LocalPlayer.Skills.BotReloadSpeed.Value = 0.6f;//换弹速度
                    Main.LocalPlayer.Skills.StrengthBuffJumpHeightInc.Value = 0.8f;//跳跃高度
                    Main.LocalPlayer.Skills.EnduranceBuffJumpCostRed.Value = 0.95f;//跳跃消耗耐力
                    Main.LocalPlayer.Skills.StrengthBuffThrowDistanceInc.Value = 1f;//力量投掷物距离
                    Main.LocalPlayer.Skills.ImmunityPainKiller.Value = 20f;//止痛药持续时间
                    Main.LocalPlayer.Skills.EnduranceBuffEnduranceInc.Value = 5f;//耐力上限
                    Main.LocalPlayer.Skills.StrengthBuffAimFatigue.Value = 0.8f;//瞄准耐力消耗
                    Main.LocalPlayer.Skills.WeaponDurabilityLosOnShotReduce.Value = 0.95f;//武器耐久消耗
                    Main.LocalPlayer.Skills.MagDrillsLoadSpeed.Value = Settings.LoadSpeed * 2.5f;//压弹速度
                    Main.LocalPlayer.Skills.MagDrillsUnloadSpeed.Value = Settings.UnloadSpeed * 2.5f;//卸弹速度

                    //Main.LocalPlayer.Skills.TroubleFixing.Value = 3f;//排障速度 无效
                    //Main.LocalPlayer.Skills.StrengthBuffSprintSpeedInc.Value = 10f;//跑步速度 无效

                    //Main.LocalPlayer.ActiveHealthController.DoPainKiller();
                    Main.LocalPlayer.Physical.WalkOverweightLimits.Set(9000f, 10000f);
                    Main.LocalPlayer.Physical.BaseOverweightLimits.Set(9000f, 10000f);
                    Main.LocalPlayer.Physical.SprintOverweightLimits.Set(9000f, 10000f);
                    Main.LocalPlayer.Physical.WalkSpeedOverweightLimits.Set(9000f, 10000f);
                    foreach (dynamic dict in Main.LocalPlayer.Skills.WeaponBuffs)
                    {
                        //System.Console.WriteLine("======================");
                        //System.Console.WriteLine("Reload Speed:" + dict.Value[EBuffId.WeaponReloadBuff].Value);
                        //System.Console.WriteLine("Swap Speed:" + dict.Value[EBuffId.WeaponSwapBuff].Value);
                        //System.Console.WriteLine("Recoil:" + dict.Value[EBuffId.WeaponRecoilBuff].Value);
                        //System.Console.WriteLine("Ergonomics:" + dict.Value[EBuffId.WeaponErgonomicsBuff].Value);
                        //System.Console.WriteLine("Double Action Recoil:" + dict.Value[EBuffId.WeaponDoubleActionRecoilReduceBuff].Value);
                        //System.Console.WriteLine("Mounting Ergonomics:" + dict.Value[EBuffId.MountingErgonomicsGainPerLevel].Value);
                        //System.Console.WriteLine("Bipod Ergonomics:" + dict.Value[EBuffId.BipodErgonomicsGainPerLevel].Value);

                        dict.Value[EBuffId.WeaponSwapBuff].Value = 0.8f;
                        dict.Value[EBuffId.WeaponRecoilBuff].Value = 0.8f;
                        dict.Value[EBuffId.WeaponErgonomicsBuff].Value = 3f;
                        dict.Value[EBuffId.MountingErgonomicsGainPerLevel].Value = 100f;
                        dict.Value[EBuffId.BipodErgonomicsGainPerLevel].Value = 100f;
                    }
                    Main.LocalPlayer.Physical.UpdateWeightLimits();
                    Main.LocalPlayer.Physical.BaseInertiaLimits = new Vector3(10000f, 10000f, 1f);
                    Main.LocalPlayer.Physical.OnWeightUpdated();

                    _nextBuffTime = Time.time + 30f;
                }


                if (Time.time > _nextRegenTime && Settings.HealPerSec != 0)
                {
                    List<EBodyPart> damaged = new List<EBodyPart>();

                    foreach (EBodyPart BodyPart in BodyPartList)
                    {
                        if (!Main.LocalPlayer.ActiveHealthController.GetBodyPartHealth(BodyPart, false).AtMaximum && !Main.LocalPlayer.ActiveHealthController.IsBodyPartDestroyed(BodyPart))
                        {
                            damaged.Add(BodyPart);
                        }
                    }
                    if (damaged.Count > 0)
                    {
                        System.Random random = new System.Random();
                        Main.LocalPlayer.ActiveHealthController.ChangeHealth(damaged[random.Next(damaged.Count)], 1, Field);
                    }
                    if(Settings.HealPerSec != 0)
                    {
                        _nextRegenTime = Time.time + (2f / (float)Settings.HealPerSec);
                    }
                }
            }
        }

        private void God()
        {
            Player localPlayer = Main.LocalPlayer;
            if (Main.LocalPlayer.ActiveHealthController != null)
            {
                Main.LocalPlayer.ActiveHealthController.SetDamageCoeff(-1f);
                Main.LocalPlayer.ActiveHealthController.RemoveNegativeEffects((EBodyPart)7);
                Main.LocalPlayer.ActiveHealthController.RestoreFullHealth();
                Main.LocalPlayer.ActiveHealthController.ChangeHydration(100f);
                Main.LocalPlayer.ActiveHealthController.ChangeEnergy(110f);
                Main.LocalPlayer.ActiveHealthController.FallSafeHeight = 1000f;
                Main.LocalPlayer.Skills.AttentionEliteLuckySearch.Value = 1f;
            }
            EFTHardSettings.Instance.LOOT_RAYCAST_DISTANCE = 6f;
            EFTHardSettings.Instance.DOOR_RAYCAST_DISTANCE = 6f;

            //InfiniteStamina
            if (Main.LocalPlayer.Physical.Stamina.Current < 99f)
            {
                Main.LocalPlayer.Physical.Stamina.Current = Main.LocalPlayer.Physical.Stamina.TotalCapacity.Value;
            }
            if (Main.LocalPlayer.Physical.HandsStamina.Current < 99f)
            {
                Main.LocalPlayer.Physical.HandsStamina.Current = Main.LocalPlayer.Physical.HandsStamina.TotalCapacity.Value;
            }
            if (Main.LocalPlayer.Physical.Oxygen.Current < 99f)
            {
                Main.LocalPlayer.Physical.Oxygen.Current = Main.LocalPlayer.Physical.Oxygen.TotalCapacity.Value;
            }
        }

        private void HalfGod()
        {
            Player localPlayer = Main.LocalPlayer;
            if (Main.LocalPlayer.ActiveHealthController != null) {

                Main.LocalPlayer.ActiveHealthController.SetDamageCoeff((100f-(Settings.DamageReduction*5))/100f);
                Main.LocalPlayer.Skills.AttentionEliteLuckySearch.Value = 0.75f;
                Main.LocalPlayer.ActiveHealthController.FallSafeHeight = 4f;
            }
            EFTHardSettings.Instance.LOOT_RAYCAST_DISTANCE = 3f;
            EFTHardSettings.Instance.DOOR_RAYCAST_DISTANCE = 3f;
        }
        private void SpeedMod()
        {
            if (Main.LocalPlayer == null && Main.MainCamera == null)
            {
                return;
            }
            if (Input.GetKey((KeyCode)119))
            {
                Main.LocalPlayer.Transform.position += Settings.SpeedMulti * Time.deltaTime * Main.MainCamera.transform.forward;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                Main.LocalPlayer.MovementContext.IsGrounded = true;
                Main.LocalPlayer.MovementContext.FreefallTime = -0.4f;
            }
        }
    }
}