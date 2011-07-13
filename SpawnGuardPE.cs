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
        public int spawnxstart1 = 0;
        public int spawnxend1= 0;
        public int spawnystart1 = 0;
        public int spawnyend1 = 0;
        public int spawnxstart2 = 0;
        public int spawnxend2 = 0;
        public int spawnystart2 = 0;
        public int spawnyend2 = 0;
        public int spawnxstart3 = 0;
        public int spawnxend3 = 0;
        public int spawnystart3 = 0;
        public int spawnyend3 = 0;
        public int spawnnumber = 1;
        public int breakx;
        public int breaky;
        public int primer = 2;
        string used = "no";
        string user;
        string userbak;
        string userpermbak;
        string userperm = "guest";
        string spawnin;
        string spawnname1;
        string spawnname2;
        string spawnname3;
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
            TDSMBuild = 24;

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + Name;
            //Create folder if it doesn't exist
            CreateDirectory(pluginFolder);
            FileStream readStream = new FileStream(pluginFolder + "/AreaBank.dat", FileMode.Open);
            BinaryReader readBinary = new BinaryReader(readStream);
            spawnnumber = readBinary.ReadInt32();
            userperm = readBinary.ReadString();
            readStream.Close();
            readBinary.Close();
            string userpermdefault = userperm;

            FileStream readA = new FileStream(pluginFolder + "/SpawnAreas/Area1.dat", FileMode.Open);
            BinaryReader readAa = new BinaryReader(readA);
            spawnxstart1 = readAa.ReadInt32();
            spawnxend1 = readAa.ReadInt32();
            spawnystart1 = readAa.ReadInt32();
            spawnyend1 = readAa.ReadInt32();
            warntext = readAa.ReadString();
            spawnname1 = readAa.ReadString();
            readA.Close();
            readAa.Close();
            if (spawnnumber > 1)
            {
                FileStream readB = new FileStream(pluginFolder + "/SpawnAreas/Area2.dat", FileMode.Open);
                BinaryReader readBa = new BinaryReader(readB);
                spawnxstart2 = readBa.ReadInt32();
                spawnxend2 = readBa.ReadInt32();
                spawnystart2 = readBa.ReadInt32();
                spawnyend2 = readBa.ReadInt32();
                warntext = readBa.ReadString();
                spawnname2 = readBa.ReadString();
                readB.Close();
                readBa.Close();
            }
            if (spawnnumber > 2)
            {
                FileStream readC = new FileStream(pluginFolder + "/SpawnAreas/Area3.dat", FileMode.Open);
                BinaryReader readCa = new BinaryReader(readC);
                spawnxstart3 = readCa.ReadInt32();
                spawnxend3 = readCa.ReadInt32();
                spawnystart3 = readCa.ReadInt32();
                spawnyend3 = readCa.ReadInt32();
                warntext = readCa.ReadString();
                spawnname3 = readCa.ReadString();
                readC.Close();
                readCa.Close();
            } 

            Console.WriteLine("[SPAWNGUARDPE] Protecting '" + spawnname1 + "':" + "(" + spawnxstart1 + "," + spawnystart1 + ") to (" + spawnxend1 + "," + spawnyend1 + ")");
                Console.WriteLine("[SPAWNGUARDPE] Protecting '" + spawnname2 + "':" + "(" + spawnxstart2 + "," + spawnystart2 + ") to (" + spawnxend2 + "," + spawnyend2 + ")");
                Console.WriteLine("[SPAWNGUARDPE] Protecting '" + spawnname3 + "':" + "(" + spawnxstart3 + "," + spawnystart3 + ") to (" + spawnxend3 + "," + spawnyend3 + ")");
            
        }

        public override void Enable()
        {
            Console.WriteLine("[SPAWNGUARDPE] Ready and Waiting!");
            isEnabled = true;
            this.registerHook(Hooks.PLAYER_TILECHANGE);
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
            primer = binaryprimer.ReadInt32();
            userperm = binaryprimer.ReadString();
            binaryprimer.Close();
            if (primer == 2)
            {
                primer = 1;
                FileStream primerwrite = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter Writer = new BinaryWriter(primerwrite);
                Writer.Write(primer);
                Writer.Write(userperm);
                primerwrite.Close();
            }
            FileStream userread = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Open);
            BinaryReader binaryuser = new BinaryReader(userread);
            primer = binaryuser.ReadInt32();
            userperm = binaryuser.ReadString();
            binaryuser.Close();
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
                        if (Event.Sender.Op)
                        {
                            Event.Sender.sendMessage("/spawnstart<number> <one word name> - Starts the process of creating the protected area.");
                            Event.Sender.sendMessage("/spawnend - Ends the process of creating the protected area.");
                            Event.Sender.sendMessage("/spawnsave - Saves the currently painted protected area.");
                            Event.Sender.sendMessage("/spawnmessage <message> - Sets the message displayed on protected area violation.");
                            Event.Sender.sendMessage("/spawnlist - Lists all of the currently protected areas.");
                        }
                        Event.Sender.sendMessage("/spawnperm - Shows the permissions menu.");
                    }
                    if (commands[0] == "/spawnsave" && Event.Sender.Op && spawnstartcommand == "unused" && spawnendcommand == "unused")
                    {
                        Event.Sender.sendMessage("Saving spawn protection settings.");
                        if (spawnnumber == 1)
                        {
                            FileStream BinaryFile = new FileStream(pluginFolder + "/SpawnAreas/Area1.dat", FileMode.Create, FileAccess.ReadWrite);
                            BinaryWriter Writer = new BinaryWriter(BinaryFile);
                            Writer.Write(spawnxstart1);
                            Writer.Write(spawnxend1);
                            Writer.Write(spawnystart1);
                            Writer.Write(spawnyend1);
                            Writer.Write(warntext);
                            Writer.Write(spawnname1);
                            BinaryFile.Close();
                        }
                        if (spawnnumber == 2)
                        {
                            FileStream BinaryFile = new FileStream(pluginFolder + "/SpawnAreas/Area2.dat", FileMode.Create, FileAccess.ReadWrite);
                            BinaryWriter Writer = new BinaryWriter(BinaryFile);
                            Writer.Write(spawnxstart2);
                            Writer.Write(spawnxend2);
                            Writer.Write(spawnystart2);
                            Writer.Write(spawnyend2);
                            Writer.Write(warntext);
                            Writer.Write(spawnname2);
                            BinaryFile.Close();
                        }
                        if (spawnnumber == 3)
                        {
                            FileStream BinaryFile = new FileStream(pluginFolder + "/SpawnAreas/Area3.dat", FileMode.Create, FileAccess.ReadWrite);
                            BinaryWriter Writer = new BinaryWriter(BinaryFile);
                            Writer.Write(spawnxstart3);
                            Writer.Write(spawnxend3);
                            Writer.Write(spawnystart3);
                            Writer.Write(spawnyend3);
                            Writer.Write(warntext);
                            Writer.Write(spawnname3);
                            BinaryFile.Close();
                        }

                        FileStream AreaBank = new FileStream(pluginFolder + "/AreaBank.dat", FileMode.Create, FileAccess.ReadWrite);
                        BinaryWriter AreaBankWriter = new BinaryWriter(AreaBank);
                        AreaBankWriter.Write(spawnnumber);
                        //AreaBankWriter.Write(userpermdefault);
                        AreaBank.Close();
                    }
                    if (commands[0] == "/spawnemptyall" && Event.Sender.Op && commands[1] == "verify")
                    {
                        spawnxstart1 = 0;
                        spawnxend1 = 0;
                        spawnystart1 = 0;
                        spawnyend1 = 0;
                        spawnname1 = "Empty";
                        spawnxstart2 = 0;
                        spawnxend2 = 0;
                        spawnystart2 = 0;
                        spawnyend2 = 0;
                        spawnname2 = "Empty";
                        spawnxstart3 = 0;
                        spawnxend3 = 0;
                        spawnystart3 = 0;
                        spawnyend3 = 0;
                        spawnname3 = "Empty";
                        Event.Sender.sendMessage("All protected areas cleared.");
                        FileStream BinaryFile = new FileStream(pluginFolder + "/SpawnAreas/Area1.dat", FileMode.Create, FileAccess.ReadWrite);
                        BinaryWriter Writer = new BinaryWriter(BinaryFile);
                        Writer.Write(0);
                        Writer.Write(0);
                        Writer.Write(0);
                        Writer.Write(0);
                        Writer.Write(warntext);
                        Writer.Write("Empty");
                        BinaryFile.Close();
                        FileStream BinaryFile1 = new FileStream(pluginFolder + "/SpawnAreas/Area2.dat", FileMode.Create, FileAccess.ReadWrite);
                        BinaryWriter Writer1 = new BinaryWriter(BinaryFile1);
                        Writer1.Write(0);
                        Writer1.Write(0);
                        Writer1.Write(0);
                        Writer1.Write(0);
                        Writer1.Write(warntext);
                        Writer1.Write("Empty");
                        BinaryFile1.Close();
                        FileStream BinaryFile2 = new FileStream(pluginFolder + "/SpawnAreas/Area3.dat", FileMode.Create, FileAccess.ReadWrite);
                        BinaryWriter Writer2 = new BinaryWriter(BinaryFile2);
                        Writer2.Write(0);
                        Writer2.Write(0);
                        Writer2.Write(0);
                        Writer2.Write(0);
                        Writer2.Write(warntext);
                        Writer2.Write("Empty");
                        BinaryFile2.Close();
                    }
                    if (commands[0] == "/spawnstart1" && Event.Sender.Op && spawnstartcommand == "unused" && spawnendcommand == "unused")
                    {
                        spawnnumber = 1;
                        spawnname1 = commands[1];
                        spawnstartcommand = "using";
                        spawnstartuser = Event.Sender.getName();
                        Event.Sender.sendMessage("Please destroy the block which will be the top left corner.");
                        Event.Sender.sendMessage("NOTE: The Spawn Protection will begin AFTER the block destroyed.");
                    }
                    if (commands[0] == "/spawnstart2" && Event.Sender.Op && spawnstartcommand == "unused" && spawnendcommand == "unused")
                    {
                        spawnnumber = 2;
                        spawnname2 = commands[1];
                        spawnstartcommand = "using";
                        spawnstartuser = Event.Sender.getName();
                        Event.Sender.sendMessage("Please destroy the block which will be the top left corner.");
                        Event.Sender.sendMessage("NOTE: The Spawn Protection will begin AFTER the block destroyed.");
                    }
                    if (commands[0] == "/spawnstart3" && Event.Sender.Op && spawnstartcommand == "unused" && spawnendcommand == "unused")
                    {
                        spawnnumber = 3;
                        spawnname3 = commands[1];
                        spawnstartcommand = "using";
                        spawnstartuser = Event.Sender.getName();
                        Event.Sender.sendMessage("Please destroy the block which will be the top left corner.");
                        Event.Sender.sendMessage("NOTE: The Spawn Protection will begin AFTER the block destroyed.");
                    }
                    if (commands[0] == "/spawnend" && Event.Sender.Op && spawnendcommand == "unused" && spawnstartcommand == "unused")
                    {
                        spawnendcommand = "using";
                        spawnenduser = Event.Sender.getName();
                        Event.Sender.sendMessage("Please destroy the block which will be the bottom right corner.");
                        Event.Sender.sendMessage("NOTE: The Spawn Protection will begin BEFORE the block destroyed.");
                    }
                    if (commands[0] == "/spawnperm")
                    {
                        Event.Sender.sendMessage("SpawnGuardPE Permissions Menu:");
                        Event.Sender.sendMessage("/spawnpermc - Checks what permissions group you are in/");
                        if (Event.Sender.Op)
                        {
                            Event.Sender.sendMessage("/spawnperms <player name> <group> - Sets a players permissions group.");
                            Event.Sender.sendMessage("/spawnpermd <group name> - Sets the default permissions group.");
                        }
                        Event.Sender.sendMessage("/spawnpermh - Shows the permissions groups and what they can do.");
                    }
                    if (commands[0] == "/spawnpermd" && Event.Sender.Op)
                    {
                        string userpermdefault = commands[1];
                        Event.Sender.sendMessage("Default Group set to: '" + userpermdefault + "'");
                        Event.Sender.sendMessage("Requires a server restart to take effect.");
                        FileStream AreaBank = new FileStream(pluginFolder + "/AreaBank.dat", FileMode.Create, FileAccess.ReadWrite);
                        BinaryWriter AreaBankWriter = new BinaryWriter(AreaBank);
                        AreaBankWriter.Write(spawnnumber);
                        AreaBankWriter.Write(userpermdefault);
                        AreaBank.Close();
                    }
                    if (commands[0] == "/spawnpermh")
                    {
                        Event.Sender.sendMessage("Silenced - Can not talk. Can not build in protected area.");
                        Event.Sender.sendMessage("Guest - Can talk. Can not build anywhere.");
                        Event.Sender.sendMessage("Builder - Can talk. Can not build in protected area.");
                        Event.Sender.sendMessage("Officer - Can talk. Can build in protected area but can not set it.");
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
                    if (commands[0] == "/spawnmessage" && Event.Sender.Op)
                        {
                            spawnin = Event.Message;
                            char[] arr = new char[] { '/', 's', 'p', 'a', 'w', 'n', 'm', 'e', 'a', 'g', ' ' };
                            warntext = spawnin.TrimStart(arr);
                            Event.Sender.sendMessage("Warn Text set to: '" + warntext + "'");
                            
                                FileStream BinaryFile = new FileStream(pluginFolder + "/SpawnAreas/Area1.dat", FileMode.Create, FileAccess.ReadWrite);
                                BinaryWriter Writer = new BinaryWriter(BinaryFile);

                                Writer.Write(spawnxstart1);
                                Writer.Write(spawnxend1);
                                Writer.Write(spawnystart1);
                                Writer.Write(spawnyend1);
                                Writer.Write(warntext);
                                Writer.Write(spawnname1);
                            
                                FileStream BinaryFile1 = new FileStream(pluginFolder + "/SpawnAreas/Area2.dat", FileMode.Create, FileAccess.ReadWrite);
                                BinaryWriter Writer1 = new BinaryWriter(BinaryFile1);

                                Writer1.Write(spawnxstart2);
                                Writer1.Write(spawnxend2);
                                Writer1.Write(spawnystart2);
                                Writer1.Write(spawnyend2);
                                Writer1.Write(warntext);
                                Writer1.Write(spawnname2);
                            
                                FileStream BinaryFile2 = new FileStream(pluginFolder + "/SpawnAreas/Area3.dat", FileMode.Create, FileAccess.ReadWrite);
                                BinaryWriter Writer2 = new BinaryWriter(BinaryFile2);

                                Writer2.Write(spawnxstart3);
                                Writer2.Write(spawnxend3);
                                Writer2.Write(spawnystart3);
                                Writer2.Write(spawnyend3);
                                Writer2.Write(warntext);
                                Writer2.Write(spawnname3);
                            
                        }
                        if (commands[0] == "/spawnnumber1" && Event.Sender.Op)
                        {
                            spawnnumber = 1;
                        }
                        if (commands[0] == "/spawnnumber2" && Event.Sender.Op)
                        {
                            spawnnumber = 2;
                        }
                        if (commands[0] == "/spawnnumber3" && Event.Sender.Op)
                        {
                            spawnnumber = 3;
                        }
                        if (commands[0] == "/spawnlist" && Event.Sender.Op)
                        {
                                Event.Sender.sendMessage("Protected area 1: '" + spawnname1 + "'");
                                Event.Sender.sendMessage("Protected area 2: '" + spawnname2 + "'");
                                Event.Sender.sendMessage("Protected area 3: '" + spawnname3 + "'");
                            
                        }
                    }
                }
            }
        
        public override void onPlayerTileChange(PlayerTileChangeEvent Event)
{
            if (isEnabled == false) { return; }
            breakx = (int)Event.Tile.tileX;
            breaky = (int)Event.Tile.tileY;
            if (userperm == "guest" && Event.Sender.Op == false)
            {
                if (used == "no")
                {
                    used = "yes";
                    Event.Sender.sendMessage("You are a guest. Talk to an OP to enable building.", 255, 255f, 0f, 0f);
                }
                Event.Cancelled = true;
            }
            if (breakx > spawnxstart1 && breakx < spawnxend1 && Event.Sender.Op == false && userperm != "officer" && userperm != "guest")
            {
                if (breaky > spawnystart1 && breaky < spawnyend1 && Event.Sender.Op == false && userperm != "officer" && userperm != "guest")
                {
                    if (userperm == "builder")
                    {
                        if (used == "no")
                        {
                            used = "yes";
                            Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                        }
                        Event.Cancelled = true;
                    }
                    if (used == "no")
                    {
                        used = "yes";
                        Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                    }
                    Event.Cancelled = true;
                }
            }

            if (userperm == "guest" && Event.Sender.Op == false)
            {
                if (used == "no")
                {
                    used = "yes";
                    Event.Sender.sendMessage("You are a guest. Talk to an OP to enable building.", 255, 255f, 0f, 0f);
                }
                Event.Cancelled = true;
            }
            if (breakx > spawnxstart2 && breakx < spawnxend2 && Event.Sender.Op == false && userperm != "officer" && userperm != "guest")
            {
                if (breaky > spawnystart2 && breaky < spawnyend2 && Event.Sender.Op == false && userperm != "officer" && userperm != "guest")
                {
                    if (userperm == "builder")
                    {
                        if (used == "no")
                        {
                            used = "yes";
                            Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                        }
                        Event.Cancelled = true;
                    }
                    if (used == "no")
                    {
                        used = "yes";
                        Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                    }
                    Event.Cancelled = true;
                }
            }
            if (userperm == "guest" && Event.Sender.Op == false)
            {
                if (used == "no")
                {
                    used = "yes";
                    Event.Sender.sendMessage("You are a guest. Talk to an OP to enable building.", 255, 255f, 0f, 0f);
                }
                Event.Cancelled = true;
            }
            if (breakx > spawnxstart3 && breakx < spawnxend3 && Event.Sender.Op == false && userperm != "officer" && userperm != "guest")
            {
                if (breaky > spawnystart3 && breaky < spawnyend3 && Event.Sender.Op == false && userperm != "officer" && userperm != "guest")
                {
                    if (userperm == "builder")
                    {
                        if (used == "no")
                        {
                            used = "yes";
                            Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                        }
                        Event.Cancelled = true;
                    }
                    if (used == "no")
                    {
                        used = "yes";
                        Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                    }
                    Event.Cancelled = true;
                }
            }
            used = "no";
            if (spawnstartcommand == "using" && spawnstartuser == Event.Sender.getName() && Event.Sender.Op)
            {
                if (spawnnumber == 1)
                {
                    spawnxstart1 = (int)Event.Tile.tileX;
                    spawnystart1 = (int)Event.Tile.tileY;
                    Event.Sender.sendMessage("Top left corner set to (" + spawnxstart1 + "," + spawnystart1 + ")");
                    Event.Sender.sendMessage("Don't forget to /spawnsave");
                    spawnstartuser = "";
                    spawnstartcommand = "unused";
                }
                if (spawnnumber == 2)
                {
                    spawnxstart2 = (int)Event.Tile.tileX;
                    spawnystart2 = (int)Event.Tile.tileY;
                    Event.Sender.sendMessage("Top left corner set to (" + spawnxstart2 + "," + spawnystart2 + ")");
                    Event.Sender.sendMessage("Don't forget to /spawnsave");
                    spawnstartuser = "";
                    spawnstartcommand = "unused";
                }
                if (spawnnumber == 3)
                {
                    spawnxstart3 = (int)Event.Tile.tileX;
                    spawnystart3 = (int)Event.Tile.tileY;
                    Event.Sender.sendMessage("Top left corner set to (" + spawnxstart3 + "," + spawnystart3 + ")");
                    Event.Sender.sendMessage("Don't forget to /spawnsave");
                    spawnstartuser = "";
                    spawnstartcommand = "unused";
                }
            }
            if (spawnendcommand == "using" && spawnenduser == Event.Sender.getName() && Event.Sender.Op)
            {
                if (spawnnumber == 1)
                {
                    spawnxend1 = (int)Event.Tile.tileX;
                    spawnyend1 = (int)Event.Tile.tileY;
                    Event.Sender.sendMessage("Bottom right corner set to (" + spawnxend1 + "," + spawnyend1 + ")");
                    Event.Sender.sendMessage("Don't forget to /spawnsave");
                    spawnenduser = "";
                    spawnendcommand = "unused";
                }
                if (spawnnumber == 2)
                {
                    spawnxend2 = (int)Event.Tile.tileX;
                    spawnyend2 = (int)Event.Tile.tileY;
                    Event.Sender.sendMessage("Bottom right corner set to (" + spawnxend2 + "," + spawnyend2 + ")");
                    Event.Sender.sendMessage("Don't forget to /spawnsave");
                    spawnenduser = "";
                    spawnendcommand = "unused";
                }
                if (spawnnumber == 3)
                {
                    spawnxend3 = (int)Event.Tile.tileX;
                    spawnyend3 = (int)Event.Tile.tileY;
                    Event.Sender.sendMessage("Bottom right corner set to (" + spawnxend3 + "," + spawnyend3 + ")");
                    Event.Sender.sendMessage("Don't forget to /spawnsave");
                    spawnenduser = "";
                    spawnendcommand = "unused";
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