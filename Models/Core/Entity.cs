using System;
using Avalonia.Media;
using System.ComponentModel;

namespace ecosystem.Models.Core
{
    public abstract class Entity : INotifyPropertyChanged
    {
        private Position _position = null!;
        private IBrush? _color;

        public Position Position
        {
            get => _position;
            protected set
            {
                if (_position != value)
                {
                    Console.WriteLine($"Entity position changing to ({value.X}, {value.Y})");
                    _position = value ?? throw new ArgumentNullException(nameof(value));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
                }
            }
        }

        public IBrush? Color
        {
            get => _color;
            protected set
            {
                if (_color != value)
                {
                    _color = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
                }
            }
        }

        protected Entity(Position position)
        {
            _position = position ?? throw new ArgumentNullException(nameof(position));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void Update() { }
    }
}