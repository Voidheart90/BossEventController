// JotunnModStub
// a Valheim mod skeleton using Jötunn
// 
// File:    JotunnModStub.cs
// Project: JotunnModStub

using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using BepInEx;
using UnityEngine;
using BepInEx.Configuration;
using Jotunn.Utils;
using Common;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using static Common.LogEx;
using Object = UnityEngine.Object;

namespace BossEventController
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class JotunnModStub : BaseUnityPlugin
    {
        public const string PluginGUID = "com.jotunn.BossEventController";
        public const string PluginName = "BossEventController";
        public const string PluginVersion = "0.0.1";
        private readonly Harmony harmony = new Harmony(PluginGUID);

        private void Awake()
        {
            //// Do all your init stuff here
            //// Acceptable value ranges can be defined to allow configuration via a slider in the BepInEx ConfigurationManager: https://github.com/BepInEx/BepInEx.ConfigurationManager
            //Config.Bind<int>("Main Section", "Example configuration integer", 1, new ConfigDescription("This is an example config, using a range limitation for ConfigurationManager", new AcceptableValueRange<int>(0, 100)));

            Config.Bind("Debug", "Enable", false, new ConfigDescription("Enable debug information"));
            Config.Bind("Debug", "LogLevel", LogLevel.All,
                new ConfigDescription("Enable Message Logging"));


            Init(Logger, Config);

            // Jotunn comes with its own Logger class to provide a consistent Log style for all mods using it
            LogInfo($"{nameof(BossEventController)} has landed");

            PrefabManager.OnPrefabsRegistered += PrefabManagerOnOnPrefabsRegistered;
        }

        private void PrefabManagerOnOnPrefabsRegistered()
        {
            try
            {
                #region IceBlock

                var iceblock = PrefabManager.Instance.CreateClonedPrefab("VoidIceBlocker", "IceBlocker");
                if (iceblock == null) throw new NullReferenceException("Can't clone IceBlock");

                var summon = iceblock.GetComponent<Destructible>();
                if (summon == null)
                    throw new NullReferenceException("Component: Invalid Destructible");

                #region IceBlock.Skeleton

                var skeletonPrefab = PrefabManager.Instance.CreateClonedPrefab("VoidDragonSkeleton", "Skeleton");
                if (skeletonPrefab == null)
                    throw new NullReferenceException("Prefab not found Skeleton");

                var skeletonAi = skeletonPrefab.GetComponent<MonsterAI>();
                if (skeletonAi == null) throw new NullReferenceException("Skeleton Monster AI");
                skeletonAi.m_huntPlayer = true;

                #endregion

                summon.m_spawnWhenDestroyed = skeletonPrefab;

                #endregion

                #region IceProjectile

                var iceProjectile = PrefabManager.Instance.CreateClonedPrefab("void_dragon_ice_projectile", "dragon_ice_projectile");
                if (iceProjectile == null) throw new NullReferenceException("Can't find dragon_ice_projectile");

                var projectileScript = iceProjectile.GetComponent<Projectile>();
                if (projectileScript == null) throw new NullReferenceException("Component: Invalid projectile script");
                projectileScript.m_spawnOnHit = iceblock;

                #endregion

                #region DragonSpitShotgun

                var voidSpit = PrefabManager.Instance.CreateClonedPrefab("void_dragon_spit_shotgun", "dragon_spit_shotgun");

                var drop = voidSpit.GetComponent<ItemDrop>();
                if (drop == null) throw new NullReferenceException("Can't clone dragon_spit_shotgun");

                drop.m_itemData.m_shared.m_attack.m_attackProjectile = iceProjectile;
                drop.m_itemData.m_shared.m_aiAttackInterval = 25.0f;

                #endregion

                #region Dragon

                var dragonPrefab = PrefabManager.Instance.GetPrefab("Dragon");
                if (dragonPrefab == null)
                    throw new NullReferenceException("Prefab not found Dragon");

                var humanoid = dragonPrefab.GetComponent<Humanoid>();
                if (humanoid == null) throw new NullReferenceException("Component: Invalid Humanoid");

                List<GameObject> list = new List<GameObject>();
                list.AddRange(humanoid.m_defaultItems);
                list.Add(voidSpit);
                humanoid.m_defaultItems = list.ToArray();

                #endregion
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }
#if DEBUG
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            { // Set a breakpoint here to break on F6 key press
            }
        }
#endif
    }
}