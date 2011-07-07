﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugin;
using Terraria_Server;
using Terraria_Server.Events;
using System.Xml;
using System.IO;

namespace SpawnGuardPE
{
    public class SpawnGuardPE : Plugin
    {
        public bool isEnabled = true;
        public int spawnxstart = 0;
        public int spawnxend = 0;
        public int spawnystart = 0;
        public int spawnyend = 0;
        public int breakx;
        public int breaky;
        string user;
        string userbak;
        string userpermbak;
        string userperm = "guest";
        string primer = "false";
        string cantalk = "true";
        string spawnin;
        string spawnendcommand = "unused";
        string spawnenduser;
        string spawnstartcommand = "unused";
        string spawnstartuser;
        public string warntext = "Editing Spawn is forbidden";
        string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "SpawnGuardPE";

        public override void Load()
        {
            Name = "SpawnGuardPE";
            Description = "Protects your spawn area.";
            Author = "Huey";
            Version = "1";
            TDSMBuild = 22;

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + Name;
            //Create folder if it doesn't exist
            CreateDirectory(pluginFolder);
            FileStream readStream = new FileStream(pluginFolder + "/Coordinates.dat", FileMode.Open);
            BinaryReader readBinary = new BinaryReader(readStream);
            spawnxstart = readBinary.ReadInt32();
            spawnxend = readBinary.ReadInt32();
            spawnystart = readBinary.ReadInt32();
            spawnyend = readBinary.ReadInt32();
            warntext = readBinary.ReadString();
            readStream.Close();
            Console.WriteLine("[SPAWNGUARDPE] Protecting (" + spawnxstart + "," + spawnystart + ") to (" + spawnxend + "," + spawnyend + ")");
        }

        public override void Enable()
        {
            Console.WriteLine("[SPAWNGUARDPE] Ready and Waiting!");
            isEnabled = true;
            this.registerHook(Hooks.TILE_CHANGE);
            this.registerHook(Hooks.PLAYER_COMMAND);
            this.registerHook(Hooks.PLAYER_LOGIN);
            this.registerHook(Hooks.PLAYER_CHAT);
        }
        public override void Disable()
        {
            Console.WriteLine("[SPAWNGUARDPE] Powering Down...");
            isEnabled = false;
        }
        public override void onPlayerJoin(PlayerLoginEvent Event)
        {
            user = Event.Player.getName().ToLower();
            FileStream primerread = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Open);
            BinaryReader binaryprimer = new BinaryReader(primerread);
            primer = binaryprimer.ReadString();
            if (primer == "false")
            {
                primer = "true";
                FileStream primerwrite = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter Writer = new BinaryWriter(primerwrite);
                Writer.Write(primer);
                Writer.Write(userperm);
                primerwrite.Close();
            }
            FileStream userread = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Create, FileAccess.ReadWrite);
            BinaryReader userbinary = new BinaryReader(userread);
            userperm = binaryprimer.ReadString();
        }
        public override void onPlayerChat(MessageEvent Event)
        {
            if (userperm == "silenced" && Event.Sender.Op == false)
            {
                Event.Sender.sendMessage("You are silenced and can not speak.", 255, 255f, 0f, 0f);
                Event.Cancelled = true;
            }
        }
        public override void onPlayerCommand(PlayerCommandEvent Event)
        {
            if (isEnabled == false) { return; }
            string[] commands = Event.Message.ToLower().Split(' ');
            if (commands.Length > 0)
            {
                if (commands[0] != null && commands[0].Trim().Length > 0)
                {
                    if (commands[0] == "/spawnguard")
                    {
                        Event.Sender.sendMessage("SpawnGuardPE Main Menu:");
                        Event.Sender.sendMessage("/spawnguard - Shows this help menu.");
                        if (Event.Sender.Op)
                        {
                            Event.Sender.sendMessage("/spawnstart - Starts the process of creating the protected area.");
                            Event.Sender.sendMessage("/spawnend - Ends the process of creating the protected area.");
                            Event.Sender.sendMessage("/spawnsave - Saves the currently painted protected area.");
                            Event.Sender.sendMessage("/spawnmessage <message> - Sets the message displayed on protected area violation.");
                        }
                        Event.Sender.sendMessage("/spawnperm - Shows the permissions menu.");
                    }
                    if (commands[0] == "/spawnperm")
                    {
                        Event.Sender.sendMessage("SpawnGuardPE Permissions Menu:");
                        Event.Sender.sendMessage("/spawnpermc - Checks what permissions group you are in/");
                        if (Event.Sender.Op)
                        {
                            Event.Sender.sendMessage("/spawnperms <player name> <group> - Sets a players permissions group.");
                        }
                        Event.Sender.sendMessage("/spawnpermh - Shows the permissions groups and what they can do.");
                    }
                    if (commands[0] == "/spawnpermc")
                    {
                        Event.Sender.sendMessage("User: " + user + ". Permissions Group: " + userperm + ".");
                    }
                    if (commands[0] == "/spawnperms" && Event.Sender.Op)
                    {
                        userbak = user;
                        user = commands[1];
                        if (commands[2] == "silenced")
                        {
                            if (commands[1] == userbak)
                            {
                                userperm = commands[2];
                            }
                            userpermbak = userperm;
                            userperm = "silenced";
                            Event.Sender.sendMessage("User " + user + " set to group " + userperm + ".");
                            FileStream userwrite = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Create, FileAccess.ReadWrite);
                            BinaryWriter Writer = new BinaryWriter(userwrite);

                            Writer.Write(primer);
                            Writer.Write(userperm);
                            user = Event.Sender.getName().ToLower();
                            userperm = userpermbak;
                            userwrite.Close();
                        }
                        if (commands[2] == "guest")
                        {
                            if (commands[1] == userbak)
                            {
                                userperm = commands[2];
                            }
                            userpermbak = userperm;
                            userperm = "guest";
                            Event.Sender.sendMessage("User " + user + " set to group " + userperm + ".");
                            FileStream userwrite = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Create, FileAccess.ReadWrite);
                            BinaryWriter Writer = new BinaryWriter(userwrite);

                            Writer.Write(primer);
                            Writer.Write(userperm);
                            user = Event.Sender.getName().ToLower();
                            userperm = userpermbak;
                            userwrite.Close();

                        }
                        if (commands[2] == "builder")
                        {
                            if (commands[1] == userbak)
                            {
                                userperm = commands[2];
                            }
                            userpermbak = userperm;
                            userperm = "builder";
                            Event.Sender.sendMessage("User " + user + " set to group " + userperm + ".");
                            FileStream userwrite = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Create, FileAccess.ReadWrite);
                            BinaryWriter Writer = new BinaryWriter(userwrite);

                            Writer.Write(primer);
                            Writer.Write(userperm);
                            user = Event.Sender.getName().ToLower();
                            userperm = userpermbak;
                            userwrite.Close();
                        }
                        if (commands[2] == "officer")
                        {
                            if (commands[1] == userbak)
                            {
                                userperm = commands[2];
                            }
                            userpermbak = userperm;
                            userperm = "officer";
                            Event.Sender.sendMessage("User " + user + " set to group " + userperm + ".");
                            FileStream userwrite = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Create, FileAccess.ReadWrite);
                            BinaryWriter Writer = new BinaryWriter(userwrite);

                            Writer.Write(primer);
                            Writer.Write(userperm);
                            user = Event.Sender.getName().ToLower();
                            userperm = userpermbak;
                            userwrite.Close();
                        }

                        }
                        if (commands[0] == "/spawndebug" && Event.Sender.Op)
                        {
                            Event.Sender.sendMessage("spawnxstart = " + spawnxstart);
                            Event.Sender.sendMessage("spawnystart = " + spawnystart);
                            Event.Sender.sendMessage("spawnxend = " + spawnxend);
                            Event.Sender.sendMessage("spawnyend = " + spawnyend);
                        }
                        if (commands[0] == "/spawnmessage" && Event.Sender.Op)
                        {
                            spawnin = Event.Message;
                            char[] arr = new char[] { '/', 's', 'p', 'a', 'w', 'n', 'm', 'e', 'a', 'g', ' ' };
                            warntext = spawnin.TrimStart(arr);
                            Event.Sender.sendMessage("Warn Text set to: '" + warntext + "'");
                            FileStream BinaryFile = new FileStream(pluginFolder + "/Coordinates.dat", FileMode.Create, FileAccess.ReadWrite);
                            BinaryWriter Writer = new BinaryWriter(BinaryFile);

                            Writer.Write(spawnxstart);
                            Writer.Write(spawnxend);
                            Writer.Write(spawnystart);
                            Writer.Write(spawnyend);
                            Writer.Write(warntext);
                            BinaryFile.Close();
                        }
                        if (commands[0] == "/spawnstart" && Event.Sender.Op && spawnstartcommand == "unused" && spawnendcommand == "unused")
                        {
                            spawnstartcommand = "using";
                            spawnstartuser = Event.Sender.getName();
                            Event.Sender.sendMessage("Please destroy the block which will be the top left corner.");
                            Event.Sender.sendMessage("NOTE: The Spawn Protection will begin AFTER the block destroyed.");
                        }
                        else if (commands[0] == "/spawnend" && Event.Sender.Op && spawnendcommand == "unused" && spawnstartcommand == "unused")
                        {
                            spawnendcommand = "using";
                            spawnenduser = Event.Sender.getName();
                            Event.Sender.sendMessage("Please destroy the block which will be the bottom right corner.");
                            Event.Sender.sendMessage("NOTE: The Spawn Protection will begin BEFORE the block destroyed.");
                        }
                        if (commands[0] == "/spawnsave" && Event.Sender.Op && spawnstartcommand == "unused" && spawnendcommand == "unused")
                        {
                            Event.Sender.sendMessage("Saving spawn protection settings.");
                            FileStream BinaryFile = new FileStream(pluginFolder + "/Coordinates.dat", FileMode.Create, FileAccess.ReadWrite);
                            BinaryWriter Writer = new BinaryWriter(BinaryFile);

                            Writer.Write(spawnxstart);
                            Writer.Write(spawnxend);
                            Writer.Write(spawnystart);
                            Writer.Write(spawnyend);
                            Writer.Write(warntext);
                            BinaryFile.Close();
                        }

                    }
                }
            }
        
        public override void  onTileChange(PlayerTileChangeEvent Event)
{
            if (isEnabled == false) { return; }
            breakx = (int)Event.Tile.tileX;
            breaky = (int)Event.Tile.tileY;
            if (userperm == "guest")
            {
                Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                Event.Cancelled = true;
            }
            if (breakx > spawnxstart && breakx < spawnxend && Event.Sender.Op == false)
            {
                if (breaky > spawnystart && breaky < spawnyend && Event.Sender.Op == false)
                {
                    if (userperm == "builder")
                    {
                        Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                        Event.Cancelled = true;
                    }
                    Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                    Event.Cancelled = true;
                }
            }
            if (spawnstartcommand == "using" && spawnstartuser == Event.Sender.getName() && Event.Sender.Op)
            {
                spawnxstart = (int)Event.Tile.tileX;
                spawnystart = (int)Event.Tile.tileY;
                Event.Sender.sendMessage("Top left corner set to (" + spawnxstart + "," + spawnystart + ")");
                Event.Sender.sendMessage("Don't forget to /spawnsave");
                spawnstartuser = "";
                spawnstartcommand = "unused";
            }
            if (spawnendcommand == "using" && spawnenduser == Event.Sender.getName() && Event.Sender.Op)
            {
                spawnxend = (int)Event.Tile.tileX;
                spawnyend = (int)Event.Tile.tileY;
                Event.Sender.sendMessage("Bottom right corner set to (" + spawnxend + "," + spawnyend + ")");
                Event.Sender.sendMessage("Don't forget to /spawnsave");
                spawnenduser = "";
                spawnendcommand = "unused";

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