using System.Globalization;
using System.Xml.Linq;

namespace ReversePack
{
    public class InitializationInfo
    {
        public long StartOffset { get; set; } = 0;
        public int BitsPerRow { get; set; } = 0x20;
        public int TotalLength { get; set; }
        public int ItemLength { get; set; }
        public string File { get; set; }

        private static class ElementNames
        {
            public const string InitializationInfo = "InitializationInfo";
            public const string StartOffset = "StartOffset";
            public const string BitsPerRow = "BitsPerRow";
            public const string TotalLength = "TotalLength";
            public const string ItemLength = "ItemLength";
            public const string File = "File";
        }

        public XElement Serialize()
        {
            return new XElement(ElementNames.InitializationInfo,
                new XElement(ElementNames.StartOffset, StartOffset.ToString("X")),
                new XElement(ElementNames.BitsPerRow, BitsPerRow.ToString("X")),
                new XElement(ElementNames.TotalLength, TotalLength.ToString("X")),
                new XElement(ElementNames.ItemLength, ItemLength.ToString("X")),
                new XElement(ElementNames.File, File)
                );
        }

        public static bool TryDeserialize(XElement element, out InitializationInfo result)
        {
            result = new InitializationInfo();

            string soString = element.Element(ElementNames.StartOffset)?.Value;
            if (soString != null && long.TryParse(soString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var soVal))
            {
                result.StartOffset = soVal;
            }

            string brString = element.Element(ElementNames.BitsPerRow)?.Value;
            if (brString != null && int.TryParse(brString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var brVal))
            {
                result.BitsPerRow = brVal;
            }

            string tlString = element.Element(ElementNames.TotalLength)?.Value;
            if (!(tlString != null && int.TryParse(tlString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var tlVal)))
            {
                return false;
            }
            result.TotalLength = tlVal;

            string ilString = element.Element(ElementNames.ItemLength)?.Value;
            if (!(ilString != null && int.TryParse(ilString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var ilVal)))
            {
                return false;
            }
            result.ItemLength = ilVal;

            result.File = element.Element(ElementNames.File)?.Value;
            if (string.IsNullOrEmpty(result.File))
            {
                return false;
            }

            return true;
        }

        public static readonly InitializationInfo Pokemon = new InitializationInfo()
        {
            TotalLength = 0x2580,
            ItemLength = 0x30,
            File = @"C:\Users\Mia\Desktop\ConquestData\Pokemon.dat"
        };

        public static readonly InitializationInfo Saihai = new InitializationInfo()
        {
            TotalLength = 0x7FC,
            ItemLength = 0x1C,
            File = @"C:\Users\Mia\Desktop\ConquestData\Saihai.dat"
        };

        public static readonly InitializationInfo ScenarioPokemon = new InitializationInfo()
        {
            TotalLength = 0x640,
            ItemLength = 0x8,
            File = @"C:\Users\Mia\Desktop\ConquestData\Scenario\Scenario00\ScenarioPokemon.dat"
        };

        public static readonly InitializationInfo ScenarioWarrior = new InitializationInfo()
        {
            TotalLength = 0x1A40,
            ItemLength = 0x20,
            File = @"C:\Users\Mia\Desktop\ConquestData\Scenario\Scenario00\ScenarioBushou.dat"
        };

        public static readonly InitializationInfo Item = new InitializationInfo()
        {
            TotalLength = 0x12D8,
            ItemLength = 0x24,
            File = @"C:\Users\Mia\Desktop\ConquestData\Item.dat"
        };

        public static readonly InitializationInfo Building = new InitializationInfo()
        {
            TotalLength = 0x10BC,
            ItemLength = 0x24,
            File = @"C:\Users\Mia\Desktop\ConquestData\Building.dat"
        };

        public static readonly InitializationInfo BaseBushou = new InitializationInfo()
        {
            TotalLength = 0x13B0,
            ItemLength = 0x14,
            File = @"C:\Users\Mia\Desktop\ConquestData\BaseBushou.dat"
        };
    }
}
