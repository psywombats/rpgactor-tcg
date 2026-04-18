using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

public class InputManager : SingletonBehaviour<InputManager>
{

    public enum Command
    {
        Primary,
        Secondary,
        Menu,
        Debug,
    };

    public enum Event
    {
        Down,
        Up,
        Hold,
    };

    private readonly Dictionary<Command, InputAction> actions = new();

    private const float KeyRepeatSeconds = 0.3f;

    private readonly List<IInputListener> listeners = new();
    private readonly List<IInputListener> nextFrameListeners = new();
    private readonly Dictionary<Command, float> holdStartTimes = new();
    private readonly Dictionary<string, IInputListener> anonymousListeners = new();

    private bool endProcessing;

    protected override void Awake()
    {
        base.Awake();
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

            nextFrameListeners.Clear();
            nextFrameListeners.AddRange(listeners);
            foreach (var listener in nextFrameListeners)
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
        var action = new InputAction(name: command.ToString(), type: InputActionType.Value);
        switch (command)
        {
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
            default:
                throw new NotImplementedException();
        }
        action.Enable();
        actions[command] = action;
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
        while (!done)
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
