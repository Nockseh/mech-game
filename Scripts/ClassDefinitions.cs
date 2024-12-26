using Godot;
using System;
using System.Collections.Generic;

namespace Godot {

    public class Mech {
        public CharacterBody2D CharacterBody;
        public Chassis Chassis;
        public Hull Hull;
        public float Heat;
        //Constructors
        public Mech(Chassis chassis, Hull hull) {
            this.CharacterBody = ResourceLoader.Load<PackedScene>("res://Scenes/MechScene.tscn").Instantiate<CharacterBody2D>();
            this.Chassis = chassis;
            this.CharacterBody.GetNode<Sprite2D>("ChassisSprite").Texture = chassis.Texture;
            this.Hull = hull;
            this.CharacterBody.GetNode<Sprite2D>("HullSprite").Texture = hull.Texture;
        }
        public Mech() : this(new Chassis(), new Hull()) { //empty args, uses defaults
            
        }
        //Methods
        public void AssignCamera() {

        }
    }

    public class MechPart {
        public float Integrity;
        public float Armor;
        public Texture2D Texture;
        //Methods
        protected static Collections.Dictionary GetRawPartData(string partName, string partType) {
            var data = (Collections.Dictionary)ResourceLoader.Load<Json>("res://MechPartData.json").Data;
            var rawPartData = (Collections.Dictionary)((Collections.Dictionary)data[partType])[partName];
            return rawPartData;
        }
    }

    public class Chassis : MechPart {
        public float WeightCapacity;
        public Chassis(string partName = "Placeholder") {
            var rawPartData = GetRawPartData(partName, "Chassis");
            this.Integrity = (float)rawPartData["Integrity"];
            this.Armor = (float)rawPartData["Armor"];
            this.Texture = ResourceLoader.Load<Texture2D>((string)rawPartData["Texture"]);
            this.WeightCapacity = (float)rawPartData["WeightCapacity"];
        }
    }

    public class Hull : MechPart {
        public float Weight;
        public List<Primary> Primaries = new List<Primary>();
        public List<Vector2> PrimaryOffsets = new List<Vector2>();
        public List<Secondary> Secondaries = new List<Secondary>();
        public List<Vector2> SecondaryOffsets = new List<Vector2>();
        public List<Utility> Utilities = new List<Utility>();
        public List<Vector2> UtilityOffsets = new List<Vector2>();
        //Constructors
        public Hull(string partName = "Placeholder") {
            var rawPartData = GetRawPartData(partName, "Hull");
            this.Integrity = (float)rawPartData["Integrity"];
            this.Armor = (float)rawPartData["Armor"];
            this.Texture = ResourceLoader.Load<Texture2D>((string)rawPartData["Texture"]);
            this.Weight = (float)rawPartData["Weight"];
            //attachment offset points
            for (int i = 1; i <= (int)rawPartData["PrimarySlots"]; i += 1) {
                PrimaryOffsets.Add(new Vector2((float)rawPartData["Primary"+i.ToString()+"X"], (float)rawPartData["Primary"+i.ToString()+"Y"]));
            }
            for (int i = 1; i <= (int)rawPartData["SecondarySlots"]; i += 1) {
                SecondaryOffsets.Add(new Vector2((float)rawPartData["Secondary" + i.ToString() + "X"], (float)rawPartData["Secondary" + i.ToString() + "Y"]));
            }
            for (int i = 1; i <= (int)rawPartData["UtilitySlots"]; i += 1) {
                UtilityOffsets.Add(new Vector2((float)rawPartData["Utility" + i.ToString() + "X"], (float)rawPartData["Utility" + i.ToString() + "Y"]));
            }
        }
    }

    public class Primary : MechPart {
        
    }
    public class  Secondary : MechPart {
        
    }
    public class Utility : MechPart {
        
    }
}