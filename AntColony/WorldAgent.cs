using ActressMas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Message = ActressMas.Message;

namespace AntColony
{
    public class WorldAgent : TurnBasedAgent
    {
        private readonly AntWorldForm _formGui;

        public World World { get; set; }
        public Dictionary<string, string> Loads { get; set; }

        public WorldAgent(World world, string name)
        {
            Name = name;
            World = world;
            Loads = new Dictionary<string, string>();

            _formGui = new AntWorldForm();
            new Thread(GuiThread).Start();
        }

        private void GuiThread()
        {
            _formGui.SetOwner(this);
            _formGui.ShowDialog();
            Application.Run();
        }

        public override void Setup()
        {
            Console.WriteLine(@"Starting " + Name);
        } 

        public override void Act(Queue<Message> messages)
        {
            while (messages.Count > 0)
            {
                var message = messages.Dequeue();
                Console.WriteLine($"[{message.Sender} -> {this.Name}]: {message.Content}");

                Utils.ParseMessage(message.Content, out var action, out string parameters);

                switch (action)
                {
                    case "position":
                        HandlePosition(message.Sender, parameters);
                        break;
                    case "pick-up":
                        HandlePickUp(message.Sender, parameters);
                        break;
                    case "carry":
                        HandleCarry(message.Sender, parameters);
                        break;
                    case "unload":
                        HandleUnload(message.Sender, parameters);
                        break;
                }

                _formGui.UpdateWorldGui();
            }
        }

        private void HandlePosition(string sender, string position)
        {
            var ant = new Ant(new Node { Position = JsonConvert.DeserializeObject<Point>(position), HasFood = false });
            World.AddOrUpdateAnt(sender, ant);

            var currentWorldNode = World.Graph.Keys.SingleOrDefault(node => node.Equals(ant.CurrentNode));

            if (currentWorldNode != null && currentWorldNode.HasFood)
            {
                Send(sender, "pick-up");
            }
            else
            {
                //TODO Decrease weight if is >0 evrey 3 edge move
                var nextEdges = GetNextEdges(currentWorldNode);
                Send(sender, Utils.Serialize("move", nextEdges));
            }
        }

        private IList<Edge> GetNextEdges(Node currentNode)
        {
            return currentNode != null
                ? World.Graph[currentNode]
                : new List<Edge>();
        }

        private void HandlePickUp(string sender, string position)
        {
            var node = new Node
            {
                Position = JsonConvert.DeserializeObject<Point>(position)
            };

            var currentWorldNode = World.Graph.Keys.SingleOrDefault(n => n.Equals(node));
            var nodeEdges = World.Graph[currentWorldNode];

            currentWorldNode.DecreaseFoodQuantity();

            World.Graph.Remove(currentWorldNode);
            World.Graph.Add(currentWorldNode, nodeEdges);

            Send(sender, "return");
        }

        private void HandleCarry(string sender, string edge)
        {
            var deserializedEdge = JsonConvert.DeserializeObject<Edge>(edge);

            var currentWorldNode = World.Graph.Keys.SingleOrDefault(node => node.Equals(deserializedEdge.NodeA));
            foreach (var worldEdge in World.Graph[currentWorldNode])
            {
                if (worldEdge.NodeB.Equals(deserializedEdge.NodeB))
                {
                    worldEdge.IncreaseWeight();
                }
            }

            Send(sender, "return");
        }

        private void HandleUnload(string sender, string position)
        {
            //TODO Check food quantity and stop if all food is at base
            HandlePosition(sender, position);
        }
    }
}