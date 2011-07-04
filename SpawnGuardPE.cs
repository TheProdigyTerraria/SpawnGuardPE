﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Plugin;
using Terraria_Server;
using Terraria_Server.Events;
using System.IO;
using System.Xml;

namespace SpawnGuardPE
{
    public class SpawnGuardPE : Plugin
    {
        public bool isEnabled = true;
        public int spawnxstart = 0;
        public int spawnxend = 0;
        public int spawnystart = 0;
        public int spawnyend = 0;
        string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "SpawnGuardPE";
        string playerpermission = "guest";
        string playerperm = "guest";
        string permtarget;
        string permtargetgroup;
        string spawnendcommand = "unused";
        string spawnenduser;
        string spawnstartcommand = "unused";
        string spawnstartuser;
        public string warntext = "Editing Spawn is forbidden";
        public override void Load()
        {
            Name = "SpawnGuardPE";
            Description = "Protects your spawn area.";
            Author = "Huey (The Prodigy)";
            Version = "1";
            TDSMBuild = 19;//Create folder if it doesn't exist
            CreateDirectory(pluginFolder);
            XmlDocument xmlDocu = new XmlDocument();
            xmlDocu.Load(pluginFolder + "/Permissions.xml");
            XmlNodeList playerpermxml = xmlDocu.GetElementsByTagName("playerperm");
            playerpermission = playerpermxml[0].InnerText.ToString();
            //break
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pluginFolder + "/Settings.Xml");
            XmlNodeList startxxml = xmlDoc.GetElementsByTagName("spawnxstart");
            XmlNodeList endxxml = xmlDoc.GetElementsByTagName("spawnxend");
            XmlNodeList startyxml = xmlDoc.GetElementsByTagName("spawnystart");
            XmlNodeList endyxml = xmlDoc.GetElementsByTagName("spawnyend");
            XmlNodeList warnxml = xmlDoc.GetElementsByTagName("warntext");
            Console.WriteLine("[SPAWNGUARDPE] Protecting (" + startxxml[0].InnerText + "," + startyxml[0].InnerText + "), (" + endxxml[0].InnerText + "," + endyxml[0].InnerText + ")");
            spawnxstart = Convert.ToInt32(startxxml[0].InnerText);
            spawnxend = Convert.ToInt32(endxxml[0].InnerText);
            spawnystart = Convert.ToInt32(startyxml[0].InnerText);
            spawnyend = Convert.ToInt32(startyxml[0].InnerText);
            warntext = warnxml[0].InnerText.ToString();
        }
        public override void Enable()
        {
            Program.tConsole.WriteLine(base.Name + " Ready and Waiting!.");
            isEnabled = true;
            this.registerHook(Hooks.TILE_CHANGE);
            this.registerHook(Hooks.PLAYER_COMMAND);
            this.registerHook(Hooks.PLAYER_LOGIN);
        }
        public override void Disable()
        {
            Program.tConsole.WriteLine(base.Name + " Powering Down...");
            isEnabled = false;
        }
        public override void onPlayerJoin(PlayerLoginEvent Event)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(pluginFolder + "/Permissions.xml", null);
            xmlWriter.WriteElementString(Event.Player.getName(), playerperm);
            xmlWriter.WriteEndElement();
            xmlWriter.Close();
        }
        public override void onPlayerCommand(PlayerCommandEvent Event)
        {
            if (isEnabled == false) { return; }
            string[] commands = Event.Message.ToLower().Split(' ');
            {
                if (commands.Length > 0)
                {
                    if (commands[0] != null && commands[0].Trim().Length > 0) //If it is not nothing, and the string is actually something
                    {
                        if (commands[0] == "/perm" && Event.Sender.Op)
                        {
                            permtarget = commands[1];
                            permtargetgroup = commands[2];
                            if (permtarget == Event.Player.getName())
                            {
                                if (permtargetgroup == "guest")
                                {
                                    playerperm = "guest";
                                }
                                if (permtargetgroup == "builder")
                                {
                                    playerperm = "builder";
                                }
                                if (permtargetgroup == "officer")
                                {
                                    playerperm = "officer";
                                }
                            }
                            XmlTextWriter xmlWriter = new XmlTextWriter(pluginFolder + "/Permissions,xml", null);
                            xmlWriter.WriteStartDocument();
                            xmlWriter.WriteStartElement("permissions");
                            xmlWriter.WriteElementString(Event.Player.getName(), playerperm);
                            xmlWriter.WriteEndElement();
                            xmlWriter.Close();
                        }
                        if (commands[0] == "/spawnstart" && Event.Sender.Op && spawnstartcommand == "unused" && spawnendcommand == "unused")
                        {
                            spawnstartcommand = "using";
                            spawnstartuser = Event.Sender.getName();
                            Event.Sender.sendMessage("Please destroy the block which will be the top left corner");
                            Event.Sender.sendMessage("NOTE: The Spawn Protection will start a block AFTER this block.");
                        }
                        else if (commands[0] == "/spawnend" && Event.Sender.Op && spawnendcommand == "unused" && spawnstartcommand == "unused")
                        {
                            spawnendcommand = "using";
                            spawnenduser = Event.Sender.getName();
                            Event.Sender.sendMessage("Please destroy the block which will be the bottom right corner");
                            Event.Sender.sendMessage("NOTE: The Spawn Protection will start a block BEFORE this block.");
                        }
                        if (commands[0] == "/spawnsave" && Event.Sender.Op && spawnstartcommand == "unused" && spawnendcommand == "unused")
                        {
                            Event.Sender.sendMessage("Saving spawn protection settings");
                            XmlTextWriter xmlWriter = new XmlTextWriter(pluginFolder + "/Settings.xml", null);
                            xmlWriter.WriteStartDocument();
                            xmlWriter.WriteStartElement("settings");
                            xmlWriter.WriteElementString("spawnxstart", spawnxstart.ToString());
                            xmlWriter.WriteElementString("spawnxend", spawnxend.ToString());
                            xmlWriter.WriteElementString("spawnystart", spawnystart.ToString());
                            xmlWriter.WriteElementString("spawnyend", spawnyend.ToString());
                            xmlWriter.WriteElementString("warntext", warntext);
                            xmlWriter.WriteEndElement();
                            xmlWriter.Close();
                        }

                    }
                }
            }
        }
        public override void onTileChange(PlayerTileChangeEvent Event)
        {
            if (isEnabled == false) { return; }
            int tilex = (int)Event.Tile.tileX;
            int tiley = (int)Event.Tile.tileY;
            if (spawnstartcommand == "using" && spawnstartuser == Event.Sender.getName() && Event.Sender.Op)
            {
                spawnxstart = (int)Event.Tile.tileX;
                spawnystart = (int)Event.Tile.tileY;
                Event.Sender.sendMessage("Top left corner set to (" + spawnxstart + "," + spawnystart + ")");
                Event.Sender.sendMessage("Don't forget to /spawnsave");
                spawnstartuser = "";
                spawnstartcommand = "unused";
            }
            else if (spawnendcommand == "using" && spawnenduser == Event.Sender.getName() && Event.Sender.Op)
            {
                spawnxend = (int)Event.Tile.tileX;
                spawnyend = (int)Event.Tile.tileY;
                Event.Sender.sendMessage("Bottom right corner set to (" + spawnxend + "," + spawnyend + ")");
                Event.Sender.sendMessage("Don't forget to /spawnsave");
                spawnenduser = "";
                spawnendcommand = "unused";

            }
            if (playerperm == "guest")
            {
                if (tiley > spawnystart && tiley < spawnyend)
                {
                Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                Event.Cancelled = true;
                }
            }
            if (tilex > spawnxstart && tilex < spawnxend && playerperm == "builder")
            {
                if (tiley > spawnystart && tiley < spawnyend)
                {
                    Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                    Event.Cancelled = true;
                }
            }
        }
        private static void CreateDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
    }
}

