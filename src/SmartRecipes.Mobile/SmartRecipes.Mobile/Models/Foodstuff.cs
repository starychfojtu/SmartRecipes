using System;
namespace SmartRecipes.Mobile
{
    public class Foodstuff
    {
        public Foodstuff(Guid id, string name, string imageUrl)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
        }

        public Guid Id { get; }

        public string Name { get; }

        public string ImageUrl { get; }
    }
}
