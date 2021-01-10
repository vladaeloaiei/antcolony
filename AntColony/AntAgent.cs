using ActressMas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AntColony
{
    public class AntAgent : TurnBasedAgent
    {
        private Ant _ant;
        private readonly Stack<Node> _route;

        //Version 2
        private int _nodesToMiddle;
        private int _walkedNodes;

        public AntAgent(string name)
        {
            Name = name;

            _route = new Stack<Node>();
        }

        public override void Setup()
        {
            Console.WriteLine(@"Starting " + Name);

            var startNode = new Node
            {
                Position = new Point
                {
                    X = Utils.WORLD_WIDTH / 2,
                    Y = Utils.WORLD_HEIGHT / 2
                },
                HasFood = false
            };

            _route.Push(new Node
            {
                Position = new Point
                {
                    X = Utils.WORLD_WIDTH / 2,
                    Y = Utils.WORLD_HEIGHT / 2
                }
            });
            _ant = new Ant(startNode);

            Send("worldAgent", Utils.Serialize("position", startNode.Position));
        }

        public override void Act(Queue<Message> messages)
        {
            while (messages.Count > 0)
            {
                var message = messages.Dequeue();

                Utils.ParseMessage(message.Content, out var action, out string parameters);

                switch (action)
                {
                    case "move":
                        ChangePosition(message.Sender, parameters);
                        break;
                    case "pick-up":
                        PickUpFood(message.Sender);
                        break;
                    case "return":
                        MoveToBase(message.Sender);
                        break;
                }
            }
        }

        private void ChangePosition(string sender, string edges)
        {
            var nextEdges = JsonConvert.DeserializeObject<IList<Edge>>(edges);

            Node currentNode = null;
            if (_route.Count > 1)
            {
                currentNode = _route.Pop();
            }

            var weights = nextEdges.Where(edge => edge.Weight > 0)
                .Select(edge => edge.Weight);

            Node nextNode = null;
            if (weights.Any())
            {
                nextNode = nextEdges.Where(edge => weights.Any(w => edge.Weight.Equals(w)))
                    .ToList()[Utils.RandNoGen.Next(weights.Count() - 1)]
                    .NodeB;
            }
            else
            {
                var nextFrontEdges = nextEdges.Where(edge => !edge.NodeB.Equals(_route.Peek()))
                    .ToList();

                nextNode = nextFrontEdges.Any()
                    ? nextFrontEdges[nextFrontEdges.Count - 1].NodeB
                    : _route.Pop();
            }

            if (nextNode != null)
            {
                _ant.CurrentNode.Position = nextNode.Position;

                if (currentNode != null)
                {
                    _route.Push(GetNewNode(currentNode));
                }

                _route.Push(GetNewNode(nextNode));

                Console.WriteLine($"{"",7}[{this.Name} -> {sender}]: position [{nextNode.ToString()}]");
                Send(sender, Utils.Serialize("position", nextNode.Position));
            }
        }

        private void PickUpFood(string sender)
        {
            _ant.State = Ant.AntState.Carrying;

            if (Utils.VERSION == 2)
            {
                _nodesToMiddle = _route.Count > 1 ? _route.Count / 2 : 1;
            }

            Console.WriteLine($"{"",7}[{this.Name} -> {sender}]: pick-up from [{_ant.CurrentNode.ToString()}]");
            Send(sender, Utils.Serialize("pick-up", _ant.CurrentNode.Position));
        }

        private void MoveToBase(string sender)
        {
            switch (Utils.VERSION)
            {
                case 1:
                    if (!IsAtBase())
                    {
                        CarryFood(sender);
                    }
                    else
                    {
                        UnloadFood(sender);
                    }
                    break;

                case 2:

                    if (!IsAtMiddle())
                    {
                        CarryFood(sender);
                        _walkedNodes++;
                    }
                    else
                    {
                        UnloadFood(sender);
                        _walkedNodes = 0;
                    }
                    break;
            }
        }

        private void CarryFood(string sender)
        {
            if (_route.Count > 1) _route.Pop(); // first element in stack is the current position
            var nextNodeToReturn = _route.Peek();

            Console.WriteLine($"{"",7}[{this.Name} -> {sender}]: carry");
            Send(sender, Utils.Serialize("carry", new Edge(
                 new Node { Position = _ant.CurrentNode.Position },
                 new Node { Position = nextNodeToReturn.Position }))
                );

            _ant.CurrentNode.Position = nextNodeToReturn.Position;
        }

        private void UnloadFood(string sender)
        {
            _ant.State = Ant.AntState.Free;
            _route.Clear();
            _route.Push(GetNewNode(_ant.CurrentNode));

            Console.WriteLine($"{"",7}[{this.Name} -> {sender}]: unload to [{_ant.CurrentNode.ToString()}]");
            Send(sender, Utils.Serialize("unload", _ant.CurrentNode.Position));
        }

        private bool IsAtBase()
        {
            return (_ant.CurrentNode.Position.Y == Utils.WORLD_WIDTH / 2 &&
                    _ant.CurrentNode.Position.Y == Utils.WORLD_HEIGHT / 2);
        }

        private bool IsAtMiddle()
        {
            return _walkedNodes >= _nodesToMiddle;
        }

        private Node GetNewNode(Node node)
        {
            return new Node
            {
                Position = new Point
                {
                    X = node.Position.X,
                    Y = node.Position.Y
                }
            };
        }
    }
}