using ActressMas;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AntColony
{
    public class AntAgent : TurnBasedAgent, DrawableEntity
    {
        private readonly World _world;
        private State _state;
        private Stack<Node> _route;
        private Point _currentPosition;
        private string _resourceCarried;

        public Edge Edge { get; set; }

        private enum State
        {
            Free,
            Carrying
        };

        public AntAgent(World world, string name)
        {
            _world = world;
            _route = new Stack<Node>();
            _route.Push(world.AntHill);
            _currentPosition = world.AntHill.Position;
            Name = name;
        }

        public override void Setup()
        {
            //TODO :this is dummy code. we should add features here
            Console.WriteLine(@"Starting " + Name);

            _state = State.Free;

            while (IsAtBase())
            {
                _currentPosition.X = Utils.RandNoGen.Next(Utils.NODE_COUNT);
                _currentPosition.Y = Utils.RandNoGen.Next(Utils.NODE_COUNT);
            }

            Send("world", Utils.Str("position", _currentPosition.X, _currentPosition.Y));
        }

        private bool IsAtBase()
        {
            //TODO :this is dummy code. we should add features here
            return (_currentPosition.Y == Utils.NODE_COUNT / 2 &&
                    _currentPosition.Y == Utils.NODE_COUNT / 2); // the position of the base
        }

        public override void Act(Queue<Message> messages)
        {
            //TODO :this is dummy code. we should add features here
            while (messages.Count > 0)
            {
                var message = messages.Dequeue();

                Console.WriteLine(@"    [{1} -> {0}]: {2}", this.Name, message.Sender, message.Content);

                Utils.ParseMessage(message.Content, out var action, out List<string> parameters);

                switch (action)
                {
                    case "block":
                        // R1. If you detect an obstacle, then change direction
                        MoveRandomly();
                        Send("world", Utils.Str("change", _currentPosition.Y, _currentPosition.Y));
                        break;
                    case "move" when _state == State.Carrying && IsAtBase():
                        // R2. If carrying samples and at the base, then unload samples
                        _state = State.Free;
                        Send("world", Utils.Str("unload", _resourceCarried));
                        break;
                    case "move" when _state == State.Carrying && !IsAtBase():
                        // R3. If carrying samples and not at the base, then travel up gradient
                        MoveToBase();
                        Send("world", Utils.Str("carry", _currentPosition.Y, _currentPosition.Y));
                        break;
                    case "rock":
                        // R4. If you detect a sample, then pick sample up
                        _state = State.Carrying;
                        _resourceCarried = parameters[0];
                        Send("world", Utils.Str("pick-up", _resourceCarried));
                        break;
                    case "move":
                        // R5. If (true), then move randomly
                        MoveRandomly();
                        Send("world", Utils.Str("change", _currentPosition.Y, _currentPosition.Y));
                        break;
                }
            }
        }

        private void MoveRandomly()
        {
            //TODO :this is dummy code. we should add features here
            var d = Utils.RandNoGen.Next(4);
            switch (d)
            {
                case 0:
                    if (_currentPosition.Y > 0) _currentPosition.Y--;
                    break;
                case 1:
                    if (_currentPosition.Y < Utils.NODE_COUNT - 1) _currentPosition.Y++;
                    break;
                case 2:
                    if (_currentPosition.Y > 0) _currentPosition.Y--;
                    break;
                case 3:
                    if (_currentPosition.Y < Utils.NODE_COUNT - 1) _currentPosition.Y++;
                    break;
            }
        }

        private void MoveToBase()
        {
            //TODO :this is dummy code. we should add features here
            var dx = _currentPosition.Y - Utils.NODE_COUNT / 2;
            var dy = _currentPosition.Y - Utils.NODE_COUNT / 2;

            if (Math.Abs(dx) > Math.Abs(dy))
                _currentPosition.Y -= Math.Sign(dx);
            else
                _currentPosition.Y -= Math.Sign(dy);
        }


        public void draw(Graphics g)
        {
            g.FillEllipse(getBrush(), _currentPosition.X, _currentPosition.Y, Utils.ANT_WIDTH, Utils.ANT_HEIGHT);
        }

        protected Brush getBrush()
        {
            return (_state == State.Carrying) ? Utils.FOOD_ANT_BRUSH : Utils.EMPTY_ANT_BRUSH;
        }
    }
}