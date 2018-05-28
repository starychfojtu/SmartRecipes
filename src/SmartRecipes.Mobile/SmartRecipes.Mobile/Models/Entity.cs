using System;
using SQLite;

namespace SmartRecipes.Mobile.Models
{
    public abstract class Entity : IEquatable<Entity>
    {
        public Entity(Guid id)
        {
            Id = id;
        }

        [PrimaryKey]
        public Guid Id { get; set; }

        public bool Equals(Entity other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity ? Equals(obj) : false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
