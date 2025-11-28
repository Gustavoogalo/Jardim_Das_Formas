using System;
using UnityEngine;

namespace Mechanics.Drag_Drop.FoodsPlants
{
    public enum IconFoodType
    {
        None,
        Melancia,
        Abacaxi,
        Morango
    }

    public enum PlantState
    {
        Seed,
        Growing,
        Grown
    }
    [Serializable]
    public class IconFoodData 
    {
        public IconFoodType type;
        public Sprite seedPacketSprite; // Sprite do pacote de semente (Item Draggable)
        public Sprite seedStageSprite;  // Sprite do visual da semente plantada (State.Seed)
        public Sprite grownStageSprite; // Sprite do visual da planta crescida (State.Grown)
    }
}