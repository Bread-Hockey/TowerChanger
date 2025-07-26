using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using ThunderRoad;
using UnityEngine;
namespace TowerChanger
{
    public class TowerChanger : ThunderScript
    {
        public static HomeTower homeTower = null;
        Type type = typeof(HomeTower);
        public static MethodInfo ShowTower;


        public static ModOptionString[] unlockDoor =
        {
            new ModOptionString("Unlock", "Unlock")
        };
        public static ModOptionString[] explode =
        {
            new ModOptionString("Explode", "Explode")
        };
        public static ModOptionString[] raid =
        {
            new ModOptionString("Start", "Start")
        };
        public static ModOptionString[] collapsed =
        {
            new ModOptionString("Broken", "Broken"),
            new ModOptionString("Standing", "Standing")
        };

        [ModOptionButton]
        [ModOption("Unlock Tower Door", valueSourceName = nameof(unlockDoor))]
        public static void UnlockDoor(string pressed)
        {
            if (Player.currentCreature != null && homeTower != null)
            {
                homeTower.dalgarianDoor.door.SetActive(true);
                homeTower.dalgarianDoor.destroyedDoor.SetActive(false);
                homeTower.UnlockTowerDoor();
                homeTower.towerTeleporter.gameObject.SetActive(true);
            }
        }

        [ModOption("Tower State", valueSourceName = nameof(collapsed))]
        public static string state;

        [ModOptionButton]
        [ModOption("Explode Tower", valueSourceName = nameof(explode))]
        public static void Explode(string pressed)
        {
            if (Player.currentCreature != null && homeTower != null)
            {
                homeTower.TowerAnnihilation();
            }
        }

        [ModOptionButton]
        [ModOption("Start Raid", valueSourceName = nameof(raid))]
        public static void Raid(string pressed)
        {
            if (Player.currentCreature != null && homeTower != null)
            {
                homeTower.StartRaid();
            }
        }
        public override void ScriptLoaded(ModManager.ModData modData)
        {
            base.ScriptLoaded(modData);
            EventManager.onLevelLoad += EventManager_onLevelLoad;
            if (!modData.TryGetModOption("Tower State", out ModOption modoptionTowerState)) return;
            modoptionTowerState.ValueChanged += ModoptionTowerState_ValueChanged;
            ShowTower = type.GetMethod("ShowTower", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void ModoptionTowerState_ValueChanged(object obj)
        {
            if (homeTower != null)
            {
                if (state == "Broken")
                {
                    ShowTower.Invoke(homeTower, new object[] { true });
                }
                else
                {
                    ShowTower.Invoke(homeTower, new object[] { false });
                }
            }
        }

        private void EventManager_onLevelLoad(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart) return;
            if (levelData.id == "Home" && Player.characterData.mode.saveData is SandboxSaveData)
            {
                if (Transform.FindObjectOfType<HomeTower>() is HomeTower tower)
                {
                    homeTower = tower;
                    if (state == "Broken")
                    {
                        ShowTower.Invoke(homeTower, new object[] { true });
                    }
                    else
                    {
                        ShowTower.Invoke(homeTower, new object[] { false });
                    }
                }
            }
            else
            {
                homeTower = null;
            }
        }
    }
}
