using System;
using System.IO;
using System.Xml;
using UnityEngine;



public static class Serializer
{
    private const string _storagePath = "Serialized";



    public static void SerializeShip(Ship ship)
    {
        XmlDocument serializedShip = new();

        XmlNode declarationNode = serializedShip.CreateXmlDeclaration("1.0", "UTF-8", "");
        serializedShip.AppendChild(declarationNode);

        XmlNode root = serializedShip.CreateElement("Spaceship");
        serializedShip.AppendChild(root);

        XmlNode blocks = serializedShip.CreateElement("Blocks");
        root.AppendChild(blocks);

        foreach (var part in ship.Parts)
        {
            var partInfo = part.Value.GetComponent<ShipPart>().Info;

            XmlNode blockNode = serializedShip.CreateElement("Block");
            blocks.AppendChild(blockNode);

            XmlNode blockKind = serializedShip.CreateElement("Kind");
            blockNode.AppendChild(blockKind);
            blockKind.InnerText = partInfo.Kind.ToString();

            XmlNode blockLevel = serializedShip.CreateElement("Level");
            blockNode.AppendChild(blockLevel);
            blockLevel.InnerText = partInfo.Level.ToString();

            XmlNode position = serializedShip.CreateElement("Position");
            blockNode.AppendChild(position);

            XmlNode positionX = serializedShip.CreateElement("X");
            position.AppendChild(positionX);
            positionX.InnerText = part.Key.x.ToString();

            XmlNode positionY = serializedShip.CreateElement("Y");
            position.AppendChild(positionY);
            positionY.InnerText = part.Key.y.ToString();
        }


        serializedShip.Save(Path.Combine(Application.dataPath, _storagePath, ship.gameObject.name + ".xml").ToString());
    }
    public static void DeserializeShip(Player owner)
    {
        string path = Path.Combine(Application.dataPath, _storagePath, owner.Ship.gameObject.name) + ".xml";

        if (!File.Exists(path))
            return;

        XmlDocument serializedShip = new();
        serializedShip.Load(path);

        XmlElement root = serializedShip.DocumentElement;

        foreach (XmlNode node in root.ChildNodes)
        {
            if (node.Name == "Blocks")
            {
                foreach (XmlNode blockNode in node.ChildNodes)
                {
                    if (blockNode.Name == "Block")
                    {
                        Vector2Int blockPosition = new();
                        string blockKind = "";
                        string blockLevel = "";
                        foreach (XmlNode blockInfoNode in blockNode.ChildNodes)
                        {
                            switch (blockInfoNode.Name)
                            {
                                case "Kind":
                                    blockKind = blockInfoNode.InnerText;
                                    break;
                                case "Level":
                                    blockLevel = blockInfoNode.InnerText;
                                    break;
                                case "Position":
                                    {
                                        foreach (XmlNode coordinate in blockInfoNode.ChildNodes)
                                        {
                                            if (coordinate.Name == "X")
                                                blockPosition.x = int.Parse(coordinate.InnerText);
                                            if (coordinate.Name == "Y")
                                                blockPosition.y = int.Parse(coordinate.InnerText);
                                        }

                                        break;
                                    }
                                default:
                                    throw new Exception($"Found block info node with unaccounted name: {blockInfoNode.Name}");
                            }
                        }

                        owner.Builder.SelectedInfo = new(Enum.Parse<PartKind>(blockKind), int.Parse(blockLevel));
                        owner.Builder.SelectedPosition = blockPosition;
                        owner.Builder.Build(false);
                    }
                }
            }
        }

        owner.Builder.SelectedInfo = new(PartKind.Hull, 0);
    }
    public static void SerializeInventory(Inventory target)
    {
        XmlDocument inventory = new();

        XmlNode declaration = inventory.CreateXmlDeclaration("1.0", "UTF-8", "");
        inventory.AppendChild(declaration);

        XmlNode root = inventory.CreateElement("Inventory");
        inventory.AppendChild(root);

        foreach (var stack in target.ItemStacks)
        {
            XmlNode item = inventory.CreateElement("Item");
            root.AppendChild(item);

            XmlNode kind = inventory.CreateElement("Kind");
            item.AppendChild(kind);
            kind.InnerText = stack.Key.ToString();

            XmlNode count = inventory.CreateElement("Count");
            item.AppendChild(count);
            count.InnerText = stack.Value.ToString();
        }

        inventory.Save(Path.Combine(Application.dataPath, _storagePath, "Inventory") + ".xml");
    }
    public static void DeserializeInventory(Inventory target)
    {
        if (!File.Exists(Path.Combine(Application.dataPath, _storagePath, "Inventory") + ".xml"))
            return;

        XmlDocument inventory = new();
        inventory.Load(Path.Combine(Application.dataPath, _storagePath, "Inventory") + ".xml");

        XmlElement root = inventory.DocumentElement;
        foreach (XmlNode item in root.ChildNodes)
        {
            var kind = "";
            var count = "";

            foreach (XmlNode itemPart in item.ChildNodes)
            {
                if (itemPart.Name == "Kind")
                    kind = itemPart.InnerText;

                if (itemPart.Name == "Count")
                    count = itemPart.InnerText;
            }

            ItemStack itemStack = new(Enum.Parse<ItemKind>(kind), int.Parse(count));
            target.Add(itemStack);
        }
    }
}
