﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game;
using UnityEngine;
using UnityEngine.Assertions;
using Util;

namespace Demos
{
    public class Demo
    {
        private const string DEMO_VERSION_STRING = "VELOCITYDEMO 1.3";

        public string PlayerName { get; private set; }
        public int MapID { get; private set; }
        public long TotalTickTime { get; private set; }
        public List<DemoTick> Ticks { get; private set; }
        public bool RunValid { get; set; }
        private string DemoName { get; set; }

        /// <summary>
        /// Creates a new Demo from a file.
        /// </summary>
        public Demo(string demoName)
        {
            DemoName = demoName;

            using (FileStream stream = new FileStream(Paths.GetDemoPath(demoName), FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(stream);
                LoadFromBinaryReader(reader);
            }
        }

        public Demo(byte[] data)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(data));
            LoadFromBinaryReader(reader);
        }

        //Make a demo from a list of ticks
        public Demo(List<DemoTick> pTicks, string pPlayerName, MapData map, bool valid)
        {
            Ticks = pTicks;
            PlayerName = pPlayerName;
            MapID = map.id;
            DemoName = Guid.NewGuid().ToString();
            RunValid = valid;

            if(Ticks != null && Ticks.Count > 0)
                TotalTickTime = Ticks[Ticks.Count - 1].Time;
        }

        private void LoadFromBinaryReader(BinaryReader reader)
        {
            Ticks = new List<DemoTick>();

            //Read header
            string demoVersion = reader.ReadString();
            Assert.IsTrue(demoVersion == DEMO_VERSION_STRING);

            PlayerName = reader.ReadString();
            MapID = reader.ReadInt32();
            TotalTickTime = reader.ReadInt64();
            RunValid = reader.ReadBoolean();

            //Read ticks until end of file
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                long time = reader.ReadInt64();
                float xPos = reader.ReadSingle();
                float yPos = reader.ReadSingle();
                float zPos = reader.ReadSingle();
                float xRot = reader.ReadSingle();
                float yRot = reader.ReadSingle();
                bool crouched = reader.ReadBoolean();
                Vector3 pos = new Vector3(xPos, yPos, zPos);
                Quaternion rot = Quaternion.Euler(xRot, yRot, 0f);
                DemoTick tick = new DemoTick(time, pos, rot, crouched);

                Ticks.Add(tick);
            }
        }

        public void SaveToFile()
        {
            using (FileStream stream = new FileStream(Paths.GetDemoPath(DemoName), FileMode.Create))
            {
                SaveToStream(stream);
            }
        }

        public void SaveToStream(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                //header
                writer.Write(DEMO_VERSION_STRING);
                writer.Write(PlayerName);
                writer.Write(MapID);
                writer.Write(TotalTickTime);
                writer.Write(RunValid);

                //ticks
                foreach(DemoTick tick in Ticks)
                {
                    writer.Write(tick.Time);
                    writer.Write(tick.Position.x);
                    writer.Write(tick.Position.y);
                    writer.Write(tick.Position.z);
                    writer.Write(tick.Rotation.eulerAngles.x);
                    writer.Write(tick.Rotation.eulerAngles.y);
                    writer.Write(tick.Crouched);
                }
            }
        }

        public void DeleteDemoFile()
        {
            File.Delete(Paths.GetDemoPath(DemoName));
        }

        public static Demo[] GetAllDemos()
        {
            string[] names = GetDemoFiles();
            Demo[] ret = new Demo[names.Length];

            for(int i = 0; i < names.Length; i++)
            {
                ret[i] = new Demo(names[i]);
            }

            return ret;
        }

        private static string[] GetDemoFiles()
        {
            return Directory.GetFiles(Paths.DemoDir, "*.vdem").Select(Path.GetFileNameWithoutExtension).ToArray();
        }
    }
}
