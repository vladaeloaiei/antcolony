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

                Console.WriteLine($"{"",7}[{message.Sender} -> {this.Name}]: {message.Content}");

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

            var maxWeight = nextEdges.Where(edge => !_route.Peek().Equals(edge.NodeB))
                .Select(edge => edge.Weight)
                .Max();

            //TODO Refactor this
            var nextNode = nextEdges.Select(edge => edge.Weight).Distinct().Count() == 1
                ? nextEdges[Utils.RandNoGen.Next(Utils.EDGE_PER_NODE_COUNT)].NodeB
                : nextEdges.First(edge => edge.Weight.Equals(maxWeight))?.NodeB;

            //var nextNode = maxWeight != 0
            //    ? nextEdges.First(edge => edge.Weight.Equals(maxWeight))?.NodeB
            //    : _route.Pop();

            if (nextNode != null)
            {
                _ant.CurrentNode.Position = nextNode.Position;
                _route.Push(nextNode);

                Send(sender, Utils.Serialize("position", nextNode.Position));
            }
        }

        private void PickUpFood(string sender)
        {
            _ant.State = Ant.AntState.Carrying;
            Send(sender, Utils.Serialize("pick-up", _ant.CurrentNode.Position));

        }

        private void MoveToBase(string sender)
        {
            if (Utils.VERSION == 1)
            {
                if (!IsAtBase())
                {
                    Send(sender, Utils.Serialize("carry", new Edge(
                        new Node { Position = _ant.CurrentNode.Position },
                        new Node { Position = _route.Peek().Position })
                ));

                    if (_route.Peek().Position.X == Utils.WORLD_WIDTH / 2 &&
                        _route.Peek().Position.Y == Utils.WORLD_HEIGHT / 2)
                    {
                        _ant.CurrentNode.Position = _route.Peek().Position;
                    }
                    else
                    {
                        _ant.CurrentNode.Position = _route.Pop().Position;
                    }
                }
                else
                {
                    _ant.State = Ant.AntState.Free;

                    Send(sender, Utils.Serialize("unload", _ant.CurrentNode.Position));
                }
            }
            else
            {
                //TODO version 2 logic
            }
        }

        private bool IsAtBase()
        {
            return (_ant.CurrentNode.Position.Y == Utils.WORLD_WIDTH / 2 &&
                    _ant.CurrentNode.Position.Y == Utils.WORLD_HEIGHT / 2);
        }
    }
}