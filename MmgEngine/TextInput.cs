using System;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MmgEngine;

public class TextInput : TextComponent
{
    private static uint _totalEnabledInstances;
    public static bool AnyEnabled => _totalEnabledInstances is not 0;
    
    private int _caretIndex;
    public Vector2 CaretPosition { get; private set; }
    public int CaretIndex
    {
        get => _caretIndex;
        set
        {
            _caretIndex = Math.Clamp(value, 0, Text.Length);
            CaretPosition = Position + Font.MeasureString(Text[.._caretIndex]) with { Y = 0 };
        }
    }

    private readonly bool _useBell;
    private readonly bool _revertText;
    private readonly Regex? _regex;
    private readonly bool _allowCaretMoving;
    private readonly Func<char, char>? _charFunc;
    private string _originalText;
    
    public event EventHandler? Escaped;
    public event EventHandler? Returned; 
    
    public TextInput(Game game, SpriteFont font, string defaultText, Vector2 position, int layer, bool allowCaretMoving,
        Regex? regex = null, bool escapingRevertsText = true, bool useBell = false, Func<char, char>? charFunc = null,
        Alignment anchor = Alignment.TopLeft)
        : base(game, font, defaultText, position, layer, anchor)
    {
        ++_totalEnabledInstances;
        _regex = regex;
        _useBell = useBell;
        _revertText = escapingRevertsText;
        _caretIndex = Text.Length;
        _originalText = Text;
        _charFunc = charFunc;
        Game.Window.TextInput += OnTextInput;
        // ReSharper disable once AssignmentInConditionalExpression
        if (_allowCaretMoving = allowCaretMoving)
            Game.Window.KeyDown += OnKeyDown;
        CaretIndex = Text.Length;
    }

    protected override void OnEnabledChanged(object sender, EventArgs args)
    {
        if (Enabled)
        {
            ++_totalEnabledInstances;
            Game.Window.TextInput += OnTextInput;
            if (_allowCaretMoving)
                Game.Window.KeyDown += OnKeyDown;
            if (_revertText)
                _originalText = Text;
        }
        else
        {
            --_totalEnabledInstances;
            Game.Window.TextInput -= OnTextInput;
            if (_allowCaretMoving)
                Game.Window.KeyDown -= OnKeyDown;
        }
        base.OnEnabledChanged(sender, args);
    }

    private void OnKeyDown(object? sender, InputKeyEventArgs e)
    {
        switch (e.Key)
        {
            case Keys.Left:
                --CaretIndex;
                break;
            case Keys.Right:
                ++CaretIndex;
                break;
            case Keys.Home:
                CaretIndex = 0;
                break;
            case Keys.End:
                CaretIndex = Text.Length;
                break;
        }
    }
    
    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (char.IsControl(e.Character))
        {
            switch (e.Key)
            {
                case Keys.Back:
                    if (CaretIndex-- is not 0)
                        Text = Text.Remove(CaretIndex, 1);
                    else if (_useBell)
                        Console.Beep();
                    break;
                case Keys.Delete:
                    if (CaretIndex < Text.Length)
                        Text = Text.Remove(CaretIndex, 1);
                    else if (_useBell)
                        Console.Beep();
                    break;
                case Keys.Enter:
                    Enabled = false;
                    Returned?.Invoke(this, EventArgs.Empty);
                    break;
                case Keys.Escape:
                    Enabled = false;
                    if (_revertText)
                        Text = _originalText;
                    Escaped?.Invoke(this, EventArgs.Empty);
                    break;
                case Keys.Left:
                    if (CaretIndex-- is 0 && _useBell)
                        Console.Beep();
                    break;
                case Keys.Right:
                    if (CaretIndex++ == Text.Length && _useBell)
                        Console.Beep();
                    break;
            }
            return;
        }
        
        var finalChar = _charFunc?.Invoke(e.Character) ?? e.Character;
        var newText = Text.Insert(CaretIndex, finalChar.ToString());
        if (_regex is not null && !_regex.IsMatch(newText))
        {
            if (_useBell)
                Console.Beep();
            return;
        }
        Text = newText;
        ++CaretIndex;
    }

    protected override void Dispose(bool disposing)
    {
        if (Enabled)
            --_totalEnabledInstances;
        Game.Window.TextInput -= OnTextInput;
        Game.Window.KeyDown -= OnKeyDown;
        base.Dispose(disposing);
    }
    
    public sealed class Caret : SimpleImage
    {
        private readonly TextInput _plug;
        public Vector2 OffsetFromCaret { get; set; }
        
        public Caret(Game game, string texturePath, Vector2 offsetFromCaret, TextInput textInput, Alignment anchor = Alignment.TopLeft)
            : base(game, texturePath, textInput.CaretPosition + offsetFromCaret, textInput.DrawOrder, anchor)
        {
            OffsetFromCaret = offsetFromCaret;
            _plug = textInput;
        }

        public Caret(Game game, Texture2D texture, Vector2 offsetFromCaret, TextInput textInput, Alignment anchor = Alignment.TopLeft)
            : base(game, texture, textInput.CaretPosition + offsetFromCaret, textInput.DrawOrder, anchor)
        {
            OffsetFromCaret = offsetFromCaret;
            _plug = textInput;
        }

        public override void Update(GameTime gameTime)
        {
            if (_plug is { Enabled: true, Visible: true })
                Position = _plug.CaretPosition + OffsetFromCaret;
        }

        public override void Draw(GameTime gameTime)
        {
            if (_plug is { Enabled: true, Visible: true })
                base.Draw(gameTime);
        }
    }
}