﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugin;
using Terraria_Server;
using Terraria_Server.Events;
 using Terraria_Server.Commands;
 using Terraria_Server.Logging;
using System.Xml;
using System.IO;

namespace SpawnGuardPE
{
    public class SpawnGuardPE : Plugin
    {
        public bool isEnabled = true;
        public int spawnxstart1 = 0;
        public int spawnxend1 = 0;
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
            TDSMBuild = 29;

            AddCommand("spawnguard")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Shows the SpawnGuard Main Help file.")
                .WithHelpText("/spawnguard")
                .Calls(SpawnMenu);
            AddCommand("spawnsave")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Saves the current protected area to the currently selected save file.")
                .WithHelpText("/spawnsave")
                .Calls(SpawnSave);
            AddCommand("spawnemptyall")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Empties all of the save slots.")
                .WithHelpText("/spawnemptyall")
                .Calls(SpawnEmptyAll);
            AddCommand("spawnstart1")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Starts the protection zone marking and saving process for save slot 1.")
                .WithHelpText("/spawnstart1 name")
                .Calls(SpawnStart1);
            AddCommand("spawnstart2")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Starts the protection zone marking and saving process for save slot 2.")
                .WithHelpText("/spawnstart2 name")
                .Calls(SpawnStart2);
            AddCommand("spawnstart3")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Starts the protection zone marking and saving process for save slot 3.")
                .WithHelpText("/spawnstart3 name")
                .Calls(SpawnStart3);
            AddCommand("spawnend")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Ends the protection zone marking and saving process for the selected slot.")
                .WithHelpText("/spawnend")
                .Calls(SpawnEnd);
            AddCommand("spawnperm")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Shows the SpawnGuard Permissions Main Help file.")
                .WithHelpText("/spawnperm")
                .Calls(SpawnPerm);
            AddCommand("spawnpermd")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Sets the default permissions group for new players.")
                .WithHelpText("/spawnpermd group")
                .Calls(SpawnPermD);
            AddCommand("spawnpermh")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Shows the permissions groups and what they can do.")
                .WithHelpText("/spawnpermh")
                .Calls(SpawnPermH);
            AddCommand("spawnpermc")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Shows your current permissions group.")
                .WithHelpText("/spawnpermc")
                .Calls(SpawnPermC);
            AddCommand("spawnperms")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Sets the said player to said permissions group.")
                .WithHelpText("/spawnperms name group")
                .Calls(SpawnPermS);
            AddCommand("spawnmessage")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Sets the spawn message. Buggy.")
                .WithHelpText("/spawnmessage message spaces allowed")
                .Calls(SpawnMessage);
            AddCommand("spawnlist")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Shows the current protected areas and their coordinates.")
                .WithHelpText("/spawnlist")
                .Calls(SpawnList);

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



            Console.WriteLine("[SPAWNGUARDPE] Protecting '" + spawnname1 + "':" + "(" + spawnxstart1 + "," + spawnystart1 + ") to (" + spawnxend1 + "," + spawnyend1 + ")");
            Console.WriteLine("[SPAWNGUARDPE] Protecting '" + spawnname2 + "':" + "(" + spawnxstart2 + "," + spawnystart2 + ") to (" + spawnxend2 + "," + spawnyend2 + ")");
            Console.WriteLine("[SPAWNGUARDPE] Protecting '" + spawnname3 + "':" + "(" + spawnxstart3 + "," + spawnystart3 + ") to (" + spawnxend3 + "," + spawnyend3 + ")");

        }

        public override void Enable()
        {
            Console.WriteLine("[SPAWNGUARDPE] Ready and Waiting!");
            isEnabled = true;
            this.registerHook(Hooks.PLAYER_TILECHANGE);
            this.registerHook(Hooks.PLAYER_LOGIN);
            this.registerHook(Hooks.PLAYER_CHAT);
        }
        public override void Disable()
        {
            Console.WriteLine("[SPAWNGUARDPE] Powering Down...");
            isEnabled = false;
        }
        public void SpawnMenu(Server server, ISender sender, ArgumentList args)
        {
            sender.sendMessage("SpawnGuardPE Main Menu:");
            if (sender.Op)
            {
                sender.sendMessage("/spawnstart<number> <one word name> - Starts the process of creating the protected area.");
                sender.sendMessage("/spawnend - Ends the process of creating the protected area.");
                sender.sendMessage("/spawnsave - Saves the currently painted protected area.");
                sender.sendMessage("/spawnmessage <message> - Sets the message displayed on protected area violation.");
                sender.sendMessage("/spawnlist - Lists all of the currently protected areas.");
            }
            sender.sendMessage("/spawnperm - Shows the permissions menu.");
        }

        public void SpawnSave(Server server, ISender sender, ArgumentList args)
        {
            sender.sendMessage("Saving spawn protection settings.");
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
        }
        public void SpawnEmptyAll(Server server, ISender sender, ArgumentList args)
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
            sender.sendMessage("All protected areas cleared.");
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
        public void SpawnStart1(Server server, ISender sender, ArgumentList args)
        {
            spawnnumber = 1;
            spawnname1 = args[0].ToString();
            spawnstartcommand = "using";
            spawnstartuser = sender.Name;
            sender.sendMessage("Please destroy the block which will be the top left corner.");
            sender.sendMessage("NOTE: The Spawn Protection will begin AFTER the block destroyed.");
        }
        public void SpawnStart2(Server server, ISender sender, ArgumentList args)
        {
            spawnnumber = 2;
            spawnname2 = args[0].ToString();
            spawnstartcommand = "using";
            spawnstartuser = sender.Name;
            sender.sendMessage("Please destroy the block which will be the top left corner.");
            sender.sendMessage("NOTE: The Spawn Protection will begin AFTER the block destroyed.");
        }
        public void SpawnStart3(Server server, ISender sender, ArgumentList args)
        {
            spawnnumber = 3;
            spawnname3 = args[0].ToString();
            spawnstartcommand = "using";
            spawnstartuser = sender.Name;
            sender.sendMessage("Please destroy the block which will be the top left corner.");
            sender.sendMessage("NOTE: The Spawn Protection will begin AFTER the block destroyed.");
        }
        public void SpawnEnd(Server server, ISender sender, ArgumentList args)
        {
            spawnendcommand = "using";
            spawnenduser = sender.Name;
            sender.sendMessage("Please destroy the block which will be the bottom right corner.");
            sender.sendMessage("NOTE: The Spawn Protection will begin BEFORE the block destroyed.");
        }
        public void SpawnPerm(Server server, ISender sender, ArgumentList args)
        {
            sender.sendMessage("SpawnGuardPE Permissions Menu:");
            sender.sendMessage("/spawnpermc - Checks what permissions group you are in/");
            if (sender.Op)
            {
                sender.sendMessage("/spawnperms <player name> <group> - Sets a players permissions group.");
                sender.sendMessage("/spawnpermd <group name> - Sets the default permissions group.");
            }
            sender.sendMessage("/spawnpermh - Shows the permissions groups and what they can do.");
        }
        public void SpawnPermD(Server server, ISender sender, ArgumentList args)
        {
            string userpermdefault = args[0].ToString().ToLower();
            sender.sendMessage("Default Group set to: '" + userpermdefault + "'");
            sender.sendMessage("Requires a server restart to take effect.");
            FileStream AreaBank = new FileStream(pluginFolder + "/AreaBank.dat", FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter AreaBankWriter = new BinaryWriter(AreaBank);
            AreaBankWriter.Write(spawnnumber);
            AreaBankWriter.Write(userpermdefault);
            AreaBank.Close();
        }
        public void SpawnPermH(Server server, ISender sender, ArgumentList args)
        {
            sender.sendMessage("Silenced - Can not talk. Can not build in protected area.");
            sender.sendMessage("Guest - Can talk. Can not build anywhere.");
            sender.sendMessage("Builder - Can talk. Can not build in protected area.");
            sender.sendMessage("Officer - Can talk. Can build in protected area but can not set it.");
        }
        public void SpawnPermC(Server server, ISender sender, ArgumentList args)
        {
            sender.sendMessage("User: " + user + ". Permissions Group: " + userperm + ".");
        }
        public void SpawnPermS(Server server, ISender sender, ArgumentList args)
        {
            userbak = user;
            user = args[0].ToString().ToLower();
            if (args[1].ToString().ToLower() == "silenced")
            {
                if (args[0].ToString().ToLower() == userbak)
                {
                    userperm = args[1].ToString().ToLower();
                }
                userpermbak = userperm;
                userperm = "silenced";
                sender.sendMessage("User " + user + " set to group " + userperm + ".");
                FileStream userwrite = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter Writer = new BinaryWriter(userwrite);

                Writer.Write(primer);
                Writer.Write(userperm);
                user = sender.Name.ToLower();
                userperm = userpermbak;
                userwrite.Close();
            }
            if (args[1].ToString().ToLower() == "guest")
            {
                if (args[0].ToString().ToLower() == userbak)
                {
                    userperm = args[1].ToString().ToLower();
                }
                userpermbak = userperm;
                userperm = "guest";
                sender.sendMessage("User " + user + " set to group " + userperm + ".");
                FileStream userwrite = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter Writer = new BinaryWriter(userwrite);

                Writer.Write(primer);
                Writer.Write(userperm);
                user = sender.Name.ToLower();
                userperm = userpermbak;
                userwrite.Close();
            }
            if (args[1].ToString().ToLower() == "builder")
            {
                if (args[0].ToString().ToLower() == userbak)
                {
                    userperm = args[1].ToString().ToLower();
                }
                userpermbak = userperm;
                userperm = "builder";
                sender.sendMessage("User " + user + " set to group " + userperm + ".");
                FileStream userwrite = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter Writer = new BinaryWriter(userwrite);

                Writer.Write(primer);
                Writer.Write(userperm);
                user = sender.Name.ToLower();
                userperm = userpermbak;
                userwrite.Close();
            }
            if (args[1].ToString().ToLower() == "officer")
            {
                if (args[0].ToString().ToLower() == userbak)
                {
                    userperm = args[1].ToString().ToLower();
                }
                userpermbak = userperm;
                userperm = "officer";
                sender.sendMessage("User " + user + " set to group " + userperm + ".");
                FileStream userwrite = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter Writer = new BinaryWriter(userwrite);

                Writer.Write(primer);
                Writer.Write(userperm);
                user = sender.Name.ToLower();
                userperm = userpermbak;
                userwrite.Close();
            }
        }
        public void SpawnMessage(Server server, ISender sender, ArgumentList args)
        {
            spawnin = args.ToString();
            char[] arr = new char[] { '/', 's', 'p', 'a', 'w', 'n', 'm', 'e', 'a', 'g', ' ' };
            warntext = spawnin.TrimStart(arr);
            sender.sendMessage("Warn Text set to: '" + warntext + "'");

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
        public void SpawnList(Server server, ISender sender, ArgumentList args)
        {
            sender.sendMessage("Protected area 1: '" + spawnname1 + "'");
            sender.sendMessage("Protected area 2: '" + spawnname2 + "'");
            sender.sendMessage("Protected area 3: '" + spawnname3 + "'");
        }
        public override void onPlayerJoin(PlayerLoginEvent Event)
        {
            try
            {
                user = Event.Player.Name.ToLower();
                FileStream primerread = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Open);
                BinaryReader binaryprimer = new BinaryReader(primerread);
                primer = binaryprimer.ReadInt32();
                userperm = binaryprimer.ReadString();
                binaryprimer.Close();
            }
            catch
            {
                FileStream readStream = new FileStream(pluginFolder + "/AreaBank.dat", FileMode.Open);
                BinaryReader readBinary = new BinaryReader(readStream);
                spawnnumber = readBinary.ReadInt32();
                userperm = readBinary.ReadString();
                readStream.Close();
                readBinary.Close();
                primer = 1;
                FileStream primerwrite = new FileStream(pluginFolder + "/PlayerPerms/" + user + ".dat", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter primerwriter = new BinaryWriter(primerwrite);
                primerwriter.Write(primer);
                primerwriter.Write(userperm);
                primerwrite.Close();
            }
        }
        public override void onPlayerChat(MessageEvent Event)
        {
            if (userperm == "silenced" && Event.Sender.Op == false)
            {
                Event.Sender.sendMessage("You are silenced and can not speak.", 255, 255f, 0f, 0f);
                Event.Cancelled = true;
            }
        }
        public override void onPlayerTileChange(PlayerTileChangeEvent Event)
        {
            if (isEnabled == false) { return; }
            breakx = (int)Event.Position.X;
            breaky = (int)Event.Position.Y;
            string tileid = Event.Tile.Type.ToString();
            if (userperm == "guest" && Event.Sender.Op == false)
            {
                if (used == "no")
                {
                    used = "yes";
                    if (tileid != "73" && tileid != "3" && tileid != "52" && tileid != "83" && tileid != "82")
                    {
                        Event.Sender.sendMessage("You are a guest. Talk to an OP to enable building.", 255, 255f, 0f, 0f);
                    }
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
                            if (tileid != "73" && tileid != "3" && tileid != "52" && tileid != "83" && tileid != "82")
                            {
                                Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                            }
                            Event.Cancelled = true;
                        }
                    }
                    if (used == "no")
                    {
                        used = "yes";
                        if (tileid != "73" && tileid != "3" && tileid != "52" && tileid != "83" && tileid != "82")
                        {
                            Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                        }
                    }
                    Event.Cancelled = true;
                }
            }

            if (userperm == "guest" && Event.Sender.Op == false)
            {
                if (used == "no")
                {
                    used = "yes";
                    if (tileid != "73" && tileid != "3" && tileid != "52" && tileid != "83" && tileid != "82")
                    {
                        Event.Sender.sendMessage("You are a guest. Talk to an OP to enable building.", 255, 255f, 0f, 0f);
                    }
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
                            if (tileid != "73" && tileid != "3" && tileid != "52" && tileid != "83" && tileid != "82")
                            {
                                Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                            }
                        }
                        Event.Cancelled = true;
                    }
                    if (used == "no")
                    {
                        used = "yes";
                        if (tileid != "73" && tileid != "3" && tileid != "52" && tileid != "83" && tileid != "82")
                        {
                            Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                        }
                    }
                    Event.Cancelled = true;
                }
            }
            if (userperm == "guest" && Event.Sender.Op == false)
            {
                if (used == "no")
                {
                    used = "yes";
                    if (tileid != "73" && tileid != "3" && tileid != "52" && tileid != "83" && tileid != "82")
                    {
                        Event.Sender.sendMessage("You are a guest. Talk to an OP to enable building.", 255, 255f, 0f, 0f);
                    }
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
                            if (tileid != "73" && tileid != "3" && tileid != "52" && tileid != "83" && tileid != "82")
                            {
                                Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                            }
                        }
                        Event.Cancelled = true;
                    }
                    if (used == "no")
                    {
                        used = "yes";
                        if (tileid != "73" && tileid != "3" && tileid != "52" && tileid != "83" && tileid != "82")
                        {
                            Event.Sender.sendMessage(warntext, 255, 255f, 0f, 0f);
                        }
                    }
                    Event.Cancelled = true;
                }
            }
            used = "no";
            if (spawnstartcommand == "using" && spawnstartuser == Event.Sender.Name && Event.Sender.Op)
            {
                if (spawnnumber == 1)
                {
                    spawnxstart1 = (int)Event.Position.X;
                    spawnystart1 = (int)Event.Position.Y;
                    Event.Sender.sendMessage("Top left corner set to (" + spawnxstart1 + "," + spawnystart1 + ")");
                    Event.Sender.sendMessage("Next step: /spawnend");
                    spawnstartuser = "";
                    spawnstartcommand = "unused";
                    Event.Cancelled = true;
                }
                if (spawnnumber == 2)
                {
                    spawnxstart2 = (int)Event.Position.X;
                    spawnystart2 = (int)Event.Position.Y;
                    Event.Sender.sendMessage("Top left corner set to (" + spawnxstart2 + "," + spawnystart2 + ")");
                    Event.Sender.sendMessage("Next step: /spawnend");
                    spawnstartuser = "";
                    spawnstartcommand = "unused";
                    Event.Cancelled = true;
                }
                if (spawnnumber == 3)
                {
                    spawnxstart3 = (int)Event.Position.X;
                    spawnystart3 = (int)Event.Position.Y;
                    Event.Sender.sendMessage("Top left corner set to (" + spawnxstart3 + "," + spawnystart3 + ")");
                    Event.Sender.sendMessage("Next step: /spawnend");
                    spawnstartuser = "";
                    spawnstartcommand = "unused";
                    Event.Cancelled = true;
                }
            }
            if (spawnendcommand == "using" && spawnenduser == Event.Sender.Name && Event.Sender.Op)
            {
                if (spawnnumber == 1)
                {
                    spawnxend1 = (int)Event.Position.X;
                    spawnyend1 = (int)Event.Position.Y;
                    Event.Sender.sendMessage("Bottom right corner set to (" + spawnxend1 + "," + spawnyend1 + ")");
                    Event.Sender.sendMessage("Next step: /spawnsave");
                    spawnenduser = "";
                    spawnendcommand = "unused";
                    Event.Cancelled = true;
                }
                if (spawnnumber == 2)
                {
                    spawnxend2 = (int)Event.Position.X;
                    spawnyend2 = (int)Event.Position.Y;
                    Event.Sender.sendMessage("Bottom right corner set to (" + spawnxend2 + "," + spawnyend2 + ")");
                    Event.Sender.sendMessage("Next step: /spawnsave");
                    spawnenduser = "";
                    spawnendcommand = "unused";
                    Event.Cancelled = true;
                }
                if (spawnnumber == 3)
                {
                    spawnxend3 = (int)Event.Position.X;
                    spawnyend3 = (int)Event.Position.Y;
                    Event.Sender.sendMessage("Bottom right corner set to (" + spawnxend3 + "," + spawnyend3 + ")");
                    Event.Sender.sendMessage("Next step: /spawnsave");
                    spawnenduser = "";
                    spawnendcommand = "unused";
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
    

