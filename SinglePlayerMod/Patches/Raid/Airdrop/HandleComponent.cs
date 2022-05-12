using Comfort.Common;
using EFT;
using JET.Utility.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SinglePlayerMod.Patches.Raid.Airdrop
{
    class HandleComponent : MonoBehaviour
    {
        private SynchronizableObject plane;
        private SynchronizableObject box;
        private bool planeEnabled;
        private bool boxEnabled;
        private int amountDropped;
        private int dropChance;
        private List<AirdropPoint> airdropPoints;
        private AirdropPoint randomAirdropPoint;
        private int boxObjId;
        private Vector3 boxPosition;
        private Vector3 planeStartPosition;
        private Vector3 planeStartRotation;
        private int planeObjId;
        private float planePositivePosition;
        private float planeNegativePosition;
        private float dropHeight;
        private float timer;
        private float timeToDrop;
        private bool doNotRun;
        private GameWorld gameWorld { get => Singleton<GameWorld>.Instance; }
        private Config config;

        public void Start()
        {
            planeEnabled = false;
            boxEnabled = false;
            amountDropped = 0;
            doNotRun = false;
            boxObjId = 10;
            planePositivePosition = 3000f;
            planeNegativePosition = -3000f;
            config = AirdropLogic.AirdropConfig;
            dropChance = ChanceToSpawn();
            dropHeight = UnityEngine.Random.Range(config.planeMinFlyHeight, config.planeMaxFlyHeight);
            timeToDrop = UnityEngine.Random.Range(config.airdropMinStartTimeSeconds, config.airdropMaxStartTimeSeconds);
            planeObjId = UnityEngine.Random.Range(1, 4);
            plane = LocationScene.GetAll<SynchronizableObject>().First(x => x.GetComponent<AirplaneSynchronizableObject>());
            box = LocationScene.GetAll<SynchronizableObject>().First(x => x.GetComponent<AirdropSynchronizableObject>());
            airdropPoints = LocationScene.GetAll<AirdropPoint>().ToList();
            randomAirdropPoint = airdropPoints.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
        }

        public void FixedUpdate() // https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html
        {
            if (gameWorld == null)
            {
                return;
            }

            timer += 0.02f;

            if (timer >= timeToDrop && !planeEnabled && amountDropped != 1 && !doNotRun)
            {
                ScriptStart();
            }

            if (timer >= timeToDrop && planeEnabled && !doNotRun)
            {
                plane.transform.Translate(Vector3.forward, Space.Self);

                switch (planeObjId)
                {
                    case 1:
                        if (plane.transform.position.z >= planePositivePosition && planeEnabled)
                        {
                            DisablePlane();
                        }

                        if (plane.transform.position.z >= randomAirdropPoint.transform.position.z && !boxEnabled)
                        {
                            StartDropSequence();
                        }
                        break;
                    case 2:
                        if (plane.transform.position.x >= planePositivePosition && planeEnabled)
                        {
                            DisablePlane();
                        }

                        if (plane.transform.position.x >= randomAirdropPoint.transform.position.x && !boxEnabled)
                        {
                            StartDropSequence();
                        }
                        break;
                    case 3:
                        if (plane.transform.position.z <= planeNegativePosition && planeEnabled)
                        {
                            DisablePlane();
                        }
                        if (plane.transform.position.z <= randomAirdropPoint.transform.position.z && !boxEnabled)
                        {
                            StartDropSequence();
                        }
                        break;
                    case 4:
                        if (plane.transform.position.x <= planeNegativePosition && planeEnabled)
                        {
                            DisablePlane();
                        }
                        if (plane.transform.position.x <= randomAirdropPoint.transform.position.x && !boxEnabled)
                        {
                            StartDropSequence();
                        }
                        break;
                }
            }
        }

        private int ChanceToSpawn()
        {
            var location = gameWorld.RegisteredPlayers[0].Location; // get location name
            int result = 25; // default result is 25% to spawn

            switch (location.ToLower())
            {
                case "bigmap":
                        result = config.airdropChancePercent.bigmap;
                        break;
                case "interchange":
                        result = config.airdropChancePercent.interchange;
                        break;
                case "rezervbase":
                        result = config.airdropChancePercent.reserve;
                        break;
                case "shoreline":
                        result = config.airdropChancePercent.shoreline;
                        break;
                case "woods":
                        result = config.airdropChancePercent.woods;
                        break;
                case "lighthouse":
                        result = config.airdropChancePercent.lighthouse;
                        break;
            }

            return result;
        }

        public bool ShouldAirdropOccur() => UnityEngine.Random.Range(1, 99) <= dropChance;

        public void DoNotRun() // currently not doing anything, could be used later for multiple drops
        {
            doNotRun = true;
        }

        public void ScriptStart()
        {
            if (!ShouldAirdropOccur())
            {
                DoNotRun();
                return;
            }

            if (box != null)
            {
                boxPosition = randomAirdropPoint.transform.position;
                boxPosition.y = dropHeight;
            }

            if (plane != null)
            {
                GeneratePlane();
            }
        }

        public void StartPlaneSequence()
        {
            planeEnabled = true;
            plane.TakeFromPool();
            plane.Init(planeObjId, planeStartPosition, planeStartRotation);
            plane.transform.LookAt(boxPosition); // make it face the proper direction
            plane.ManualUpdate(0); // update once

            // sound spawning
            var sound = plane.GetComponentInChildren<AudioSource>();
            sound.volume = config.planeVolume;
            sound.dopplerLevel = 0;
            sound.Play();
        }

        public void StartDropSequence()
        {
            object[] objToPass = new object[1];
            objToPass[0] = SynchronizableObjectType.AirDrop;
            gameWorld.SynchronizableObjectLogicProcessor.GetType().GetMethod("method_9", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(gameWorld.SynchronizableObjectLogicProcessor, objToPass);

            boxEnabled = true;

            Type airdropLogicClass = Constants.Instance.TargetAssemblyTypes
                .Where(type =>
                        type.GetField("airdropSynchronizableObject_0", BindingFlags.NonPublic | BindingFlags.Instance) != null &&
                        type.GetMethod("method_17", BindingFlags.NonPublic | BindingFlags.Instance) != null).First();


            box.SetLogic((GInterface27)Activator.CreateInstance(airdropLogicClass)); // this will need to be made by using full reflection mode to exclude casting GInterface
            box.ReturnToPool();
            box.TakeFromPool();
            box.Init(boxObjId, boxPosition, Vector3.zero);
        }

        public void GeneratePlane()
        {
            // determine where plane should be facing and from where it should start!!
            switch (planeObjId)
            {
                case 1:
                    planeStartPosition = new Vector3(0, dropHeight, planeNegativePosition);
                    planeStartRotation = new Vector3(0, 0, 0);
                    break;
                case 2:
                    planeStartPosition = new Vector3(planeNegativePosition, dropHeight, 0);
                    planeStartRotation = new Vector3(0, 90, 0);
                    break;
                case 3:
                    planeStartPosition = new Vector3(0, dropHeight, planePositivePosition);
                    planeStartRotation = new Vector3(0, 180, 0);
                    break;
                case 4:
                    planeStartPosition = new Vector3(planePositivePosition, dropHeight, 0);
                    planeStartRotation = new Vector3(0, 270, 0);
                    break;
            }

            StartPlaneSequence();
        }

        public void DisablePlane()
        {
            planeEnabled = false;
            amountDropped++;
            plane.ReturnToPool();
        }
    }
}
