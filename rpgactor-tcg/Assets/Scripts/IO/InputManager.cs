using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

public class InputManager : SingletonBehavior
{
    public static InputManager Instance => Global.Instance?.Input;

    public enum Command
    {
        Primary,
        Secondary,
        Left,
        Right,
        Up,
        Down,
        Menu,
        Debug,
    };

    public enum Event
    {
        Down,
        Up,
        Hold,
    };

    private Dictionary<Command, InputAction> actions = new Dictionary<Command, InputAction>();

    private static readonly float KeyRepeatSeconds = 0.3f;

    private List<IInputListener> listeners = new List<IInputListener>();
    private List<IInputListener> listeners2 = new List<IInputListener>();
    private Dictionary<Command, float> holdStartTimes = new Dictionary<Command, float>();
    private Dictionary<string, IInputListener> anonymousListeners = new Dictionary<string, IInputListener>();

    private bool endProcessing;

    public void Awake()
    {
        foreach (Command cmd in Enum.GetValues(typeof(Command)))
        {
            SetDefaultKeybindsForCommand(cmd);
        }
    }

    public void Update()
    {
        if (actions.Count == 0)
        {
            return;
        }

        foreach (Command command in Enum.GetValues(typeof(Command)))
        {
            var up = false;
            var down = false;
            var held = false;

            var action = actions[command];
            if (action.IsPressed() && !holdStartTimes.ContainsKey(command))
            {
                down = true;
                holdStartTimes[command] = Time.time;
            }
            if (!action.IsPressed() && holdStartTimes.ContainsKey(command))
            {
                up = true;
                holdStartTimes.Remove(command);
            }
            if (action.IsPressed())
            {
                held = true;
            }

            listeners2.Clear();
            listeners2.AddRange(listeners);
            foreach (var listener in listeners2)
            {
                endProcessing = false;

                if (down)
                {
                    endProcessing = listener.OnCommand(command, Event.Down);
                    if (endProcessing)
                    {
                        break;
                    }
                }
                if (up)
                {
                    endProcessing = listener.OnCommand(command, Event.Up);
                    if (endProcessing)
                    {
                        break;
                    }
                }
                if (held && holdStartTimes.ContainsKey(command))
                {
                    endProcessing = listener.OnCommand(command, Event.Hold);
                    if (Time.time - holdStartTimes[command] > KeyRepeatSeconds)
                    {
                        endProcessing |= listener.OnCommand(command, Event.Down);
                        holdStartTimes[command] = Time.time - KeyRepeatSeconds / 2f;
                    }
                }
            }
        }
    }

    public void EndProcessing()
    {
        holdStartTimes.Clear();
        endProcessing = true;
    }

    public void PushListener(string id, Func<Command, Event, bool> responder)
    {
        IInputListener listener = new AnonymousListener(responder);
        anonymousListeners.Add(id, listener);
        PushListener(listener);
    }
    public void PushListener(IInputListener listener)
    {
        listeners.Insert(0, listener);
    }

    public void RemoveListener(string id)
    {
        listeners.Remove(anonymousListeners[id]);
        anonymousListeners.Remove(id);
    }
    public void RemoveListener(IInputListener listener)
    {
        listeners.Remove(listener);
    }

    public IInputListener PeekListener()
    {
        return listeners.Count > 0 ? listeners[0] : null;
    }

    public bool IsFastKeyDown()
    {
        return actions[Command.Primary].IsPressed() || Keyboard.current.ctrlKey.isPressed;
    }

    public void SetDefaultKeybindsForCommand(Command command)
    {
        InputAction action = new InputAction(name: command.ToString(), type: InputActionType.Value);
        switch (command)
        {
            case Command.Left:
                action.AddBinding(Keyboard.current.numpad4Key);
                action.AddBinding(Keyboard.current.leftArrowKey);
                action.AddBinding(Keyboard.current.aKey);
                action.AddBinding("<Gamepad>/leftStick/left");
                action.AddBinding("<Gamepad>/dpad/left");
                break;
            case Command.Right:
                action.AddBinding(Keyboard.current.numpad6Key);
                action.AddBinding(Keyboard.current.rightArrowKey);
                action.AddBinding(Keyboard.current.dKey);
                action.AddBinding("<Gamepad>/leftStick/right");
                action.AddBinding("<Gamepad>/dpad/right");
                break;
            case Command.Up:
                action.AddBinding(Keyboard.current.numpad8Key);
                action.AddBinding(Keyboard.current.upArrowKey);
                action.AddBinding(Keyboard.current.wKey);
                action.AddBinding("<Gamepad>/leftStick/up");
                action.AddBinding("<Gamepad>/dpad/up");
                break;
            case Command.Down:
                action.AddBinding(Keyboard.current.numpad2Key);
                action.AddBinding(Keyboard.current.downArrowKey);
                action.AddBinding(Keyboard.current.sKey);
                action.AddBinding("<Gamepad>/leftStick/down");
                action.AddBinding("<Gamepad>/dpad/down");
                break;
            case Command.Primary:
                action.AddBinding(Keyboard.current.spaceKey);
                action.AddBinding(Keyboard.current.enterKey);
                action.AddBinding(Keyboard.current.zKey);
                action.AddBinding(Keyboard.current.numpadEnterKey);
                action.AddBinding("<Gamepad>/buttonSouth");
                action.AddBinding(Mouse.current.leftButton);
                break;
            case Command.Secondary:
                action.AddBinding(Keyboard.current.xKey);
                action.AddBinding(Keyboard.current.shiftKey);
                action.AddBinding(Mouse.current.rightButton);
                action.AddBinding("<Gamepad>/buttonEast");
                break;
            case Command.Menu:
                action.AddBinding(Keyboard.current.escapeKey);
                action.AddBinding(Keyboard.current.bKey);
                action.AddBinding(Keyboard.current.backspaceKey);
                action.AddBinding("<Gamepad>/startButton");
                action.AddBinding("<Gamepad>/buttonWest");
                break;
            case Command.Debug:
                action.AddBinding(Keyboard.current.f12Key);
                break;
                break;
        }
        action.Enable();
        actions[command] = action;
    }

    public override void GameReset()
    {
        base.GameReset();
        listeners.Clear();
        anonymousListeners.Clear();
    }

    public IEnumerator ConfirmRoutine(bool eatsOthers = true)
    {
        var id = "confirm" + UnityEngine.Random.Range(0, 100000);
        var done = false;
        PushListener(id, (command, type) =>
        {
            if (type == Event.Down && command == Command.Primary)
            {
                RemoveListener(id);
                done = true;
                return true;
            }
            return eatsOthers;
        });
        while (!done && !Global.Instance.Avatar.IsCrashing)
        {
            yield return null;
        }
    }

    public Task ConfirmAsync()
    {
        var id = "confirm";
        var source = new TaskCompletionSource<bool>();
        PushListener(id, (command, type) =>
        {
            if (type == Event.Up && command == Command.Primary)
            {
                RemoveListener(id);
                source.SetResult(true);
            }
            return true;
        });
        return source.Task;
    }

    public string GetBindingForCommand(Command command)
    {
        var action = actions[command];
        return action.bindings[0].ToDisplayString();
    }

    public InputAction GetActionForCommand(Command command) => actions[command];

    public Vector2Int GetMouse()
    {
        var pos = Mouse.current.position;
        return new Vector2Int((int)pos.x.ReadValue(), (int)pos.y.ReadValue());
    }
}
