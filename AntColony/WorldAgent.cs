using ActressMas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Message = ActressMas.Message;

namespace AntColony
{
    public class WorldAgent : TurnBasedAgent
    {
        private readonly AntWorldForm _formGui;

        private int _totalFood;

        private Stopwatch _stopwatch;

        public World World { get; set; }
        public Dictionary<string, string> Loads { get; set; }

        public WorldAgent(World world, string name, Stopwatch stopwatch)
        {
            Name = name;
            World = world;
            Loads = new Dictionary<string, string>();

            _stopwatch = stopwatch;
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

            var edges = GetGraphEdges();

            new Thread(() =>
            {
                DecreaseWeights(World.Graph.Values.SelectMany(e => e).ToList());
            }).Start();
        }

        private void DecreaseWeights(List<Edge> edges)
        {
            while (true)
            {
                Thread.Sleep(Utils.DECREMENT_DELAY);

                edges.ForEach(edge =>
                {
                    edge.DecreaseWight();
                });
            }
        }

        private List<Edge> GetGraphEdges()
        {
            var edges = new List<Edge>();

            foreach (var edge in World.Graph.Values.SelectMany(e => e))
            {
                if (!edges.Any(e => e.Equals(edge)))
                {
                    edges.Add(edge);
                }
            }

            return edges;
        }

        public override void Act(Queue<Message> messages)
        {
            while (messages.Count > 0)
            {
                var message = messages.Dequeue();

                Utils.ParseMessage(message.Content, out var action, out string parameters);

                _formGui.UpdateWorldGui();

                if (_totalFood == World.TotalFood)
                {
                    Console.WriteLine($"[{this.Name}]: STOP");
                    this.Stop();
                    _stopwatch.Stop();
                    Console.WriteLine("Time elapsed: {0}", _stopwatch.Elapsed);
                    Broadcast( "stop");
                    return;
                }
                
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
            }
        }

        private void HandlePosition(string sender, string position)
        {
            var ant = new Ant(new Node { Position = JsonConvert.DeserializeObject<Point>(position), HasFood = false });
            World.AddOrUpdateAnt(sender, ant);

            var currentWorldNode = World.Graph.Keys.SingleOrDefault(node => node.Equals(ant.CurrentNode));

            if (currentWorldNode != null && currentWorldNode.HasFood)
            {
                currentWorldNode.DecreaseFoodQuantity();
                Console.WriteLine($"[{this.Name} -> {sender}]: pick-up");
                Send(sender, "pick-up");
            }
            else
            {
                var nextEdges = GetNextEdges(currentWorldNode);

                var edgesString = new StringBuilder();
                nextEdges.Select(e => new { e.NodeA, e.NodeB }).ToList().ForEach(e =>
                {
                    edgesString.Append($"<({e.NodeA.ToString()}), ({e.NodeB.ToString()})> ");
                });
                Console.WriteLine($"[{this.Name} -> {sender}]: move to [{edgesString}]");

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

            World.Graph.Remove(currentWorldNode);
            World.Graph.Add(currentWorldNode, nodeEdges);

            Console.WriteLine($"[{this.Name} -> {sender}]: return");
            Send(sender, "return");
        }

        private void HandleCarry(string sender, string edge)
        {
            var deserializedEdge = JsonConvert.DeserializeObject<Edge>(edge);

            var currentWorldNode = World.Graph.Keys.SingleOrDefault(node => node.Equals(deserializedEdge.NodeA));
            foreach (var nodeEdges in World.Graph.Values)
            {
                foreach (var nodeEdge in nodeEdges)
                {
                    if (nodeEdge.Equals(deserializedEdge))
                    {
                        nodeEdge.IncreaseWeight();
                    }
                }
            }

            Console.WriteLine($"[{this.Name} -> {sender}]: return");
            Send(sender, "return");
        }

        private void HandleUnload(string sender, string position)
        {
            var node = new Node
            {
                Position = JsonConvert.DeserializeObject<Point>(position)
            };

            var currentWorldNode = World.Graph.Keys.SingleOrDefault(n => n.Equals(node));

            if (node.Position.X == Utils.WORLD_WIDTH / 2 && node.Position.Y == Utils.WORLD_HEIGHT / 2)
            {
                _totalFood++;
            }

            if (Utils.VERSION == 2)
            {
                currentWorldNode.IncreaseFoodQuantity();

                var nextEdges = GetNextEdges(currentWorldNode);

                var edgesString = new StringBuilder();
                nextEdges.Select(e => new { e.NodeA, e.NodeB }).ToList().ForEach(e =>
                {
                    edgesString.Append($"<({e.NodeA.ToString()}), ({e.NodeB.ToString()})> ");
                });
                Console.WriteLine($"[{this.Name} -> {sender}]: move to [{edgesString}]");

                Send(sender, Utils.Serialize("move", nextEdges));
            }
            else
            {
                HandlePosition(sender, position);
            }
        }
    }
}