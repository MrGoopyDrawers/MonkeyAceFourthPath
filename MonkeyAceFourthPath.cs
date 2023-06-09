using MelonLoader;
using BTD_Mod_Helper;
using PathsPlusPlus;
using Il2CppAssets.Scripts.Models.Towers;
using BTD_Mod_Helper.Api.Enums;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using JetBrains.Annotations;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppSystem.IO;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using Il2CppAssets.Scripts.Models.TowerSets;
using BTD_Mod_Helper.Api.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using BTD_Mod_Helper.Api.Display;
using UnityEngine;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using System.Runtime.CompilerServices;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

using MonkeyAceFourthPath;

[assembly: MelonInfo(typeof(MonkeyAceFourthPath.MonkeyAceFourthPath), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace MonkeyAceFourthPath;

public class MonkeyAceFourthPath : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<MonkeyAceFourthPath>("MonkeyAceFourthPath loaded!");
    }
    public class FourthPath2 : PathPlusPlus
    {
        public override string Tower => TowerType.MonkeyAce;
        public override int UpgradeCount => 5;

    }
    public class BiggerDarts : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 150;
        public override int Tier => 1;
        public override string Icon => VanillaSprites.ArmorPiercingDartsUpgradeIcon;

        public override string Description => "Bigger darts have a larger hitbox, and pop more bloons";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var attackModel = towerModel.GetAttackModel();
            attackModel.weapons[0].projectile.pierce += 2;
            attackModel.weapons[0].projectile.scale *= 2;

        }
    }
    public class FieryDarts : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 350;
        public override int Tier => 2;
        public override string Icon => VanillaSprites.FlintTips;

        public override string Description => "Darts pierce through lead and set bloons on fire.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var attackModel = towerModel.GetAttackModel();
            var projectileModel = attackModel.weapons[0].projectile;
            projectileModel.collisionPasses = new int[] { -1, 0 };
            var LavaBehavior = Game.instance.model.GetTowerFromId("Alchemist").GetDescendant<AddBehaviorToBloonModel>().Duplicate();
            LavaBehavior.GetBehavior<DamageOverTimeModel>().interval = 1 / 12f;
            LavaBehavior.lifespan = 10;
            LavaBehavior.lifespanFrames = 600;
            LavaBehavior.overlayType = "Fire";
            //LavaBehavior.overlayType
            projectileModel.AddBehavior(LavaBehavior);
            attackModel.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        }
    }
    
    public class TacticalAce : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 850;
        public override int Tier => 3;
        public override string Icon => VanillaSprites.FullMetalJacketUpgradeIcon;

        public override string Description => "Fire lasts longer and has big damage.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var attackModel = towerModel.GetAttackModel();
            attackModel.weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>().lifespan *= 2;
            attackModel.weapons[0].projectile.GetDescendant<DamageOverTimeModel>().damage += 10f;
            attackModel.weapons[0].projectile.GetDescendant<DamageOverTimeModel>().interval = 1 / 20f;
        }
    }
    public class EmergencyAirburst : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 2000;
        public override int Tier => 4;
        public override string Icon => VanillaSprites.AirburstDartsUpgradeIcon;

        public override string Description => "Darts burst out into more darts. Fire is permanent.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var attackModel = towerModel.GetAttackModel();
            attackModel.weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>().lifespan *= 939;
            attackModel.weapons[0].projectile.AddBehavior(new CreateProjectileOnContactModel("CreateProjectileOnContactModel_", attackModel.weapons[0].projectile.Duplicate(), new ArcEmissionModel("ArcEmissionModel_", 8, 0.0f, 360.0f, null, false, false), true, false, false));
        }
    }
    public class CrazyAce : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 55000;
        public override int Tier => 5;
        public override string Icon => VanillaSprites.CrashedMoabPropIcon;

        public override string Description => "Crazy Ace... [In more specific terms, the darts just add multiple status effects to just wipe out bloons whole, and causes some minor chaos.]";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var attackModel = towerModel.GetAttackModel();
            attackModel.weapons[0].projectile.AddBehavior(new WindModel("windModel_", 12f, 30f, 1000f, true, null, 0, null, 2));
            attackModel.weapons[0].projectile.AddBehavior(Game.instance.model.GetTowerFromId("Adora").GetAttackModel().weapons[0].projectile.GetBehavior<AdoraTrackTargetModel>().Duplicate());
            attackModel.weapons[0].projectile.AddBehavior(new CreateProjectileOnContactModel("FragSpikes", Game.instance.model.GetTowerFromId("SpikeFactory-230").GetWeapon().projectile.Duplicate(), new SingleEmissionModel("FragSpikes_Emission", null), true, false, true));
            attackModel.weapons[0].projectile.AddBehavior(new CreateProjectileOnExhaustFractionModel("FragSpikes", Game.instance.model.GetTowerFromId("SpikeFactory-230").GetWeapon().projectile.Duplicate(), Game.instance.model.GetTowerFromId("DartMonkey").GetWeapon().emission.Duplicate(), 1.0f, -1.0f, false, false, false));
            attackModel.weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.RemoveBehavior<ArriveAtTargetModel>();
        }
    }
}