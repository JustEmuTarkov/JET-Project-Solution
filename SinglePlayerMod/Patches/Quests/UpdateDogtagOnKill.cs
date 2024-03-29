﻿using JET.Utility.Patching;
using System;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using UnityEngine;

using Equipment = GClass2087; // GetSlot
using StDamage = GStruct248; // HittedBallisticCollider

namespace SinglePlayerMod.Patches.Quests
{
    class UpdateDogtagOnKill : GenericPatch<UpdateDogtagOnKill>
    {
        private static Func<Player, Equipment> getEquipmentProperty;

        public UpdateDogtagOnKill() : base(postfix: nameof(PatchPostfix))
        {
            // compile-time checks
            _ = nameof(Equipment.GetSlot);
            _ = nameof(StDamage.Weapon);

            getEquipmentProperty = typeof(Player)
                .GetProperty("Equipment", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetGetMethod(true)
                .CreateDelegate(typeof(Func<Player, Equipment>)) as Func<Player, Equipment>;
        }

        protected override MethodBase GetTargetMethod() => typeof(Player)
            .GetMethod("OnBeenKilledByAggressor", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void PatchPostfix(Player __instance, Player aggressor, StDamage damageInfo)
        {
            if (__instance.Profile.Info.Side == EPlayerSide.Savage)
            {
                return;
            }

            var equipment = getEquipmentProperty(__instance);
            var dogtagSlot = equipment.GetSlot(EquipmentSlot.Dogtag);
            var dogtagItem = dogtagSlot.ContainedItem as Item;

            if (dogtagItem == null)
            {
                Debug.LogError("[DogtagPatch] error > DogTag slot item is null somehow.");
                return;
            }

            var itemComponent = dogtagItem.GetItemComponent<DogtagComponent>();

            if (itemComponent == null)
            {
                Debug.LogError("[DogtagPatch] error > DogTagComponent on dog tag slot is null. Something went horrifically wrong!");
                return;
            }

            var victimProfileInfo = __instance.Profile.Info;

            itemComponent.AccountId = __instance.Profile.AccountId;
            itemComponent.ProfileId = __instance.Profile.Id;
            itemComponent.Nickname = victimProfileInfo.Nickname;
            itemComponent.Side = victimProfileInfo.Side;
            itemComponent.KillerName = aggressor.Profile.Info.Nickname;
            itemComponent.Time = DateTime.Now;
            itemComponent.Status = "Killed by ";
            itemComponent.KillerAccountId = aggressor.Profile.AccountId;
            itemComponent.KillerProfileId = aggressor.Profile.Id;
            itemComponent.WeaponName = damageInfo.Weapon.Name;

            if (__instance.Profile.Info.Experience > 0)
            {
                itemComponent.Level = victimProfileInfo.Level;
            }
        }
    }
}
