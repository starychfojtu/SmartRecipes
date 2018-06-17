using System;
using SQLite;

namespace SmartRecipes.Mobile.Models
{
    public abstract class Entity : IEquatable<Entity>
    {
        protected Entity(Guid id)
        {
            Id = id;
        }

        [PrimaryKey]
        public Guid Id { get; set; }

        public bool Equals(Entity other)
        {
            return other != null && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity e && Equals(e);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
