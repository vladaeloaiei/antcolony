using ActressMas;
using Message = ActressMas.Message;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace AntColony
{
    public class WorldAgent : TurnBasedAgent
    {
        private readonly string _antHillPosition;

        private AntWorldForm _formGui;

        public World World { get; set; }

        public Dictionary<string, string> AntsPositions { get; set; }
        public Dictionary<string, string> Loads { get; set; }

        public WorldAgent(World world, string name)
        {
            Name = name;
            World = world;
            AntsPositions = new Dictionary<string, string>();
            Loads = new Dictionary<string, string>();
            _antHillPosition = Utils.Str(Utils.NODE_COUNT / 2, Utils.NODE_COUNT / 2);

            _formGui = new AntWorldForm();
            new Thread(GUIThread).Start();
        }

        private void GUIThread()
        {
            _formGui.SetOwner(this);
            _formGui.ShowDialog();
            Application.Run();
        }

        public override void Setup()
        {
            Console.WriteLine(@"Starting " + Name);

            // var resPos = new List<string>();
            // var compPos = Utils.Str(Utils.NODE_COUNT / 2, Utils.NODE_COUNT / 2);
            // resPos.Add(compPos); // the position of the base
            //
            // for (var i = 1; i <= Utils.NoResources; i++)
            // {
            //     while (resPos.Contains(compPos)) // resources do not overlap
            //     {
            //         var x = Utils.RandNoGen.Next(Utils.NODE_COUNT);
            //         var y = Utils.RandNoGen.Next(Utils.NODE_COUNT);
            //         compPos = Utils.Str(x, y);
            //     }
            //
            //     ResourcePositions.Add("res" + i, compPos);
            //     resPos.Add(compPos);
            // }
        }

        public override void Act(Queue<Message> messages)
        {
            while (messages.Count > 0)
            {
                var message = messages.Dequeue();
                Console.WriteLine(@"\t[{1} -> {0}]: {2}", Name, message.Sender, message.Content);

                Utils.ParseMessage(message.Content, out var action, out string parameters);

                switch (action)
                {
                    case "position":
                        HandlePosition(message.Sender, parameters);
                        break;

                    case "change":
                        HandleChange(message.Sender, parameters);
                        break;

                    case "pick-up":
                        HandlePickUp(message.Sender, parameters);
                        break;

                    case "carry":
                        HandleCarry(message.Sender, parameters);
                        break;

                    case "unload":
                        HandleUnload(message.Sender);
                        break;
                }

                _formGui.UpdateWorldGui();
            }
        }

        private void HandlePosition(string sender, string position)
        {
            AntsPositions.Add(sender, position);
            Send(sender, "move");
        }

        private void HandleChange(string sender, string position)
        {
            AntsPositions[sender] = position;

            foreach (var k in AntsPositions.Keys)
            {
                if (k == sender)
                    continue;
                if (AntsPositions[k] == position)
                {
                    Send(sender, "block");
                    return;
                }
            }

            Send(sender, "move");
        }

        private void HandlePickUp(string sender, string position)
        {
            Loads[sender] = position;
            Send(sender, "move");
        }

        private void HandleCarry(string sender, string position)
        {
            var res = Loads[sender];

            AntsPositions[sender] = position;
            Send(sender, "move");
        }

        private void HandleUnload(string sender)
        {
            Loads.Remove(sender);
            Send(sender, "move");
        }
    }
}