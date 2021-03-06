using System.Collections.Generic;
using Ctrl_Space.Input.Devices;
using Microsoft.Xna.Framework;

namespace Ctrl_Space.Input
{
    class InputManager
    {
        private bool _isActive = false;

        private Game _game;
        private IDevice[] _devices;
        private ControlMapping[] _controlMapping;

        public InputManager(Game game)
        {
            _game = game;

            _devices = new IDevice[]
            {
                new XNAKeyboard(game),
                new XNAMouse(game),
                new XNAGamePad(game)
            };

            _controlMapping = new ControlMapping[]
            {
                new ControlMapping { Device = "XNAKeyboard", Event = "W", Method = "Up" },
                new ControlMapping { Device = "XNAKeyboard", Event = "S", Method = "Down" },
                new ControlMapping { Device = "XNAKeyboard", Event = "A", Method = "Left" },
                new ControlMapping { Device = "XNAKeyboard", Event = "D", Method = "Right" },
                new ControlMapping { Device = "XNAKeyboard", Event = "Escape", Method = "Exit" },
                new ControlMapping { Device = "XNAMouse", Event = "X", Method = "RotateCW" },
                new ControlMapping { Device = "XNAMouse", Event = "LMB", Method = "PrimaryWeapon" },
                new ControlMapping { Device = "XNAMouse", Event = "RMB", Method = "SecondaryWeapon" },
                
                new ControlMapping { Device = "XNAGamePad", Event = "Up", Method = "Up" },
                new ControlMapping { Device = "XNAGamePad", Event = "Down", Method = "Down" },
                new ControlMapping { Device = "XNAGamePad", Event = "Left", Method = "Left" },
                new ControlMapping { Device = "XNAGamePad", Event = "Right", Method = "Right" },
                new ControlMapping { Device = "XNAGamePad", Event = "LeftY", Method = "Up" },
                new ControlMapping { Device = "XNAGamePad", Event = "LeftX", Method = "Right" },
                new ControlMapping { Device = "XNAGamePad", Event = "Back", Method = "Exit" },
                new ControlMapping { Device = "XNAGamePad", Event = "RightX", Method = "RotateCW" },
                new ControlMapping { Device = "XNAGamePad", Event = "A", Method = "PrimaryWeapon" },
                new ControlMapping { Device = "XNAGamePad", Event = "B", Method = "SecondaryWeapon" },
                new ControlMapping { Device = "XNAGamePad", Event = "X", Method = "PrimaryWeapon" },
                new ControlMapping { Device = "XNAGamePad", Event = "Y", Method = "SecondaryWeapon" },
                new ControlMapping { Device = "XNAGamePad", Event = "RightTrigger", Method = "PrimaryWeapon" },
                new ControlMapping { Device = "XNAGamePad", Event = "LeftTrigger", Method = "SecondaryWeapon" },
                new ControlMapping { Device = "XNAGamePad", Event = "RightShoulder", Method = "PrimaryWeapon" },
                new ControlMapping { Device = "XNAGamePad", Event = "LeftShoulder", Method = "SecondaryWeapon" },
            };
        }

        public void StartUpdate()
        {
            _isActive = true;
        }

        public void StopUpdate()
        {
            _isActive = false;
        }

        private List<DeviceData> _deviceData = new List<DeviceData>();
        private InputState _inputState = new InputState();

        public void Update(GameTime gameTime)
        {
            if (!_isActive)
                return;

            _deviceData.Clear();
            _inputState.Reset();
            foreach (IDevice device in _devices)
            {
                device.GetData(_deviceData);
            }

            foreach (DeviceData d in _deviceData)
            {
                foreach (ControlMapping cm in _controlMapping)
                {
                    if (d.Device == cm.Device && d.Event == cm.Event)
                        _inputState.Process(cm.Method, d.Data);
                }
            }

            Vector2 move = new Vector2();
            move.X = _inputState.Right - _inputState.Left;
            move.Y = _inputState.Up - _inputState.Down;

            if (move.LengthSquared() > 1.0f)
                move.Normalize();

            MoveRightLeft(new InputAnalogEventArgs(move.X));
            MoveUpDown(new InputAnalogEventArgs(move.Y));
            Rotate(new InputAnalogEventArgs(_inputState.RotateCW - _inputState.RotateCCW));
            PrimaryWeapon(new InputDigitalEventArgs(_inputState.PrimaryWeapon ? InputDigitalState.Pressed : InputDigitalState.Released));
            SecondaryWeapon(new InputDigitalEventArgs(_inputState.SecondaryWeapon ? InputDigitalState.Pressed : InputDigitalState.Released));
            if (_inputState.Exit) ExitGame();
        }

        public event InputAnalogEventHandler MoveUpDown;

        public event InputAnalogEventHandler MoveRightLeft;

        public event InputAnalogEventHandler Rotate;

        public event InputDigitalEventHandler PrimaryWeapon;

        public event InputDigitalEventHandler SecondaryWeapon;

        public event InputPressEventHandler DebugMode;

        public event InputPressEventHandler PlayStopMediaPlayer;

        public event InputPressEventHandler ExitGame;
    }
}
