using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using ThunderRoad;
using ThunderRoad.Skill.Spell;
using UnityEngine;
namespace TowerChanger
{
    public class TowerChanger : ThunderScript
    {
        public static HomeTower homeTower = null;


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
                homeTower.Invoke("UnlockTowerDoor", 0);
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
                homeTower.Invoke("TowerAnnihilation", 0);
            }
        }

        [ModOptionButton]
        [ModOption("Start Raid", valueSourceName = nameof(raid))]
        public static void Raid(string pressed)
        {
            if (Player.currentCreature != null && homeTower != null)
            {
                homeTower.Invoke("StartRaid", 0);
            }
        }
        public override void ScriptLoaded(ModManager.ModData modData)
        {
            base.ScriptLoaded(modData);
            EventManager.onLevelLoad += EventManager_onLevelLoad;
            if (!modData.TryGetModOption("Tower State", out ModOption modoptionTowerState)) return;
            modoptionTowerState.ValueChanged += ModoptionTowerState_ValueChanged;
        }

        private void ModoptionTowerState_ValueChanged(object obj)
        {
            if (homeTower != null)
            {
                if (state == "Broken")
                {
                    ShowTowerMethod(true);
                }
                else
                {
                    ShowTowerMethod(false);
                }
            }
        }

        private void EventManager_onLevelLoad(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart) return;
            if (levelData.id == "Home")
            {
                if (Transform.FindObjectOfType<HomeTower>() is HomeTower tower)
                {
                    homeTower = tower;
                    if (state == "Broken")
                    {
                        ShowTowerMethod(true);
                    }
                    else
                    {
                        ShowTowerMethod(false);
                    }
                }
            }
            else
            {
                homeTower = null;
            }
        }

        private void ShowTowerMethod(bool isCollapsed)
        {
            if (isCollapsed)
            {
                homeTower.towerUndamaged.gameObject.SetActive(value: false);
                foreach (ParticleSystem item in homeTower.towerStorm)
                {
                    if (!((UnityEngine.Object)(object)item == null))
                    {
                        item.Stop();
                    }
                }

                homeTower.towerCollapsed.gameObject.SetActive(value: true);
                return;
            }

            homeTower.towerUndamaged.gameObject.SetActive(value: true);
            foreach (ParticleSystem item2 in homeTower.towerStorm)
            {
                if (!((UnityEngine.Object)(object)item2 == null))
                {
                    item2.Play();
                }
            }

            homeTower.towerCollapsed.gameObject.SetActive(value: false);
        }
    }
}
