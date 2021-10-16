using ReversePack.HeatMapFilters;
using ReversePack.MapGenerators;
using ReversePack.PluginCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace ReversePack.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private InitializationInfo _initInfo;
        private IMapFunction _selectedMapFunction;
        private IHeatFilter _selectedHeatFilter;
        private List<List<BitViewModel>> _bitRows;
        private string _uintIndex;
        private string _offset;
        private string _value;
        private uint _selectedItem;
        private bool _notDivisibleWarning;

        public ICommand CopySelectedItemBitsCommand { get; }
        public ICommand ExportInitializationInfoCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand LoadInfoCommand { get; }

        public MainWindowViewModel()
        {
            MapFunctions = new List<IMapFunction>()
            {
                new FluctuationMapFunction(),
                new ParityMapFunction(),
                new AverageMapFunction(),
            };

            HeatFilters = new List<IHeatFilter>()
            {
                new LinearHeatFilter(),
                new GrayscaleHeatFilter(),
            };

            // Load Plugins
            var plugins = new PluginLoader().LoadPlugins(out PluginLoader.FailureInfo failures);
            MapFunctions.AddRange(plugins.MapFunctions);
            HeatFilters.AddRange(plugins.HeatFilters);

            _selectedMapFunction = MapFunctions.First();
            _selectedHeatFilter = HeatFilters.First();

            InitInfo = InitializationInfo.Pokemon;

            if (failures.AnyFailures)
            {
                _ = MessageBox.Show(string.Join("\n\n",
                    "Failed to load assemblies:",
                    string.Join("\n\n", failures.FailedToLoadAssemblies.Select(i => $"{i.Item1.FullName}:\n{i.Item2.Message}")),
                    "Failed to get types from assemblies:",
                    string.Join("\n\n", failures.FailedToGetTypesFromAssembly.Select(i => $"{i.Item1.Location}:\n{i.Item2.Message}")),
                    "Failed to activate types",
                    string.Join("\n\n", failures.FailedToActivateType.Select(i => $"{i.Item1.FullName}:\n{i.Item2.Message}"))
                    ),
                    "Failed to load some of the plugins"
                    );
            }

            CopySelectedItemBitsCommand = new RelayCommand(() =>
            {
                string str = GenerateItemBitString(_bytes
                        .Skip((int)SelectedItem * InitInfo.ItemLength)
                        .Take(InitInfo.ItemLength)
                        .ToArray(),
                    InitInfo.BitsPerRow);

                Clipboard.SetText(str);
            });

            ExportInitializationInfoCommand = new RelayCommand(() =>
            {
                string path = MakeUniquePath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "InitializationInfo.xml"));
                var xdoc = new XDocument(InitInfo.Serialize());

                using (var file = File.Create(path))
                {
                    xdoc.Save(file);
                }
            });

            UpdateCommand = new RelayCommand(UpdateDisplay);

            LoadInfoCommand = new RelayCommand(() =>
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Select an Initialization Info File",
                    DefaultExt = ".xml",
                    Filter = "Initialization Info Xml File (.xml)|*.xml",
                    CheckFileExists = true,
                    CheckPathExists = true,
                };

                // Show save file dialog box
                bool? proceed = dialog.ShowDialog();

                if (proceed == true)
                {
                    string file = dialog.FileName;
                    if (File.Exists(file))
                    {
                        XDocument doc;
                        try
                        {
                            doc = XDocument.Load(file);
                        }
                        catch
                        {
                            return;
                        }

                        if (InitializationInfo.TryDeserialize(doc.Root, out InitializationInfo result))
                        {
                            InitInfo = result;
                        }
                    }
                }
            });
        }

        private static string MakeUniquePath(string path)
        {
            string testPath = path;
            string root = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
            string ext = Path.GetExtension(path);
            int count = 1;
            while (File.Exists(testPath) || Directory.Exists(testPath))
            {
                testPath = $"{root} [{count++}]{ext}";
            }
            return testPath;
        }

        public List<IMapFunction> MapFunctions { get; }
        public List<IHeatFilter> HeatFilters { get; }

        public IMapFunction SelectedMapFunction
        {
            get => _selectedMapFunction;
            set
            {
                if (RaiseAndSetIfChanged(ref _selectedMapFunction, value))
                {
                    UpdateDisplay();
                }
            }
        }

        public IHeatFilter SelectedHeatFilter
        {
            get => _selectedHeatFilter;
            set
            {
                if (RaiseAndSetIfChanged(ref _selectedHeatFilter, value))
                {
                    UpdateDisplay();
                }
            }
        }

        public InitializationInfo InitInfo
        {
            get => _initInfo;
            set
            {
                if (RaiseAndSetIfChanged(ref _initInfo, value))
                {
                    UpdateDisplay();
                }
            }
        }

        public List<List<BitViewModel>> BitRows
        {
            get => _bitRows;
            private set => RaiseAndSetIfChanged(ref _bitRows, value);
        }

        public string UIntIndex
        {
            get => _uintIndex;
            set => RaiseAndSetIfChanged(ref _uintIndex, value);
        }

        public string Offset
        {
            get => _offset;
            set => RaiseAndSetIfChanged(ref _offset, value);
        }

        public string Value
        {
            get => _value;
            set => RaiseAndSetIfChanged(ref _value, value);
        }

        public uint SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (RaiseAndSetIfChanged(ref _selectedItem, value))
                {
                    UpdateDisplay();
                }
            }
        }

        public bool NotDivisibleWarning
        {
            get => _notDivisibleWarning;
            private set => RaiseAndSetIfChanged(ref _notDivisibleWarning, value);
        }

        private byte[] _bytes;
        private void UpdateDisplay()
        {
            byte[] bytes;
            if (!File.Exists(InitInfo.File))
            {
                return;
            }
            using (var file = new BinaryReader(File.OpenRead(InitInfo.File)))
            {
                if (file.BaseStream.Length - InitInfo.StartOffset < InitInfo.TotalLength)
                {
                    return;
                }
                file.BaseStream.Position = InitInfo.StartOffset;
                bytes = file.ReadBytes(InitInfo.TotalLength);
                _bytes = bytes;
            }
            if (InitInfo.TotalLength < InitInfo.ItemLength)
            {
                return;
            }
            if ((SelectedItem + 1) * InitInfo.ItemLength > InitInfo.TotalLength)
            {
                // change without triggering update display
                _selectedItem = 0;
                RaisePropertyChanged(nameof(SelectedItem));
            }

            NotDivisibleWarning = bytes.Length % InitInfo.ItemLength != 0;

            List<List<BitViewModel>> rows = new List<List<BitViewModel>>();

            var bm = GenerateBitMap(bytes, InitInfo.ItemLength);
            var am = bm.Select(SelectedMapFunction.ApplyTo)
                .ToArray();

            int[] vm = GenerateItemMap(bytes
                .Skip((int)SelectedItem * InitInfo.ItemLength)
                .Take(InitInfo.ItemLength)
                .ToArray());

            int uintCount = am.Length / InitInfo.BitsPerRow;
            for (int uintIndex = 0; uintIndex < uintCount; uintIndex++)
            {
                var amGroup = am
                    .Zip(vm)
                    .Skip(uintIndex * InitInfo.BitsPerRow)
                    .Take(InitInfo.BitsPerRow)
                    .Select((tup, index) => new BitViewModel(
                        tup.Second,
                        SelectedHeatFilter.ApplyTo(tup.First),
                        tup.First,
                        InitInfo.BitsPerRow - index - 1,
                        uintCount - uintIndex - 1,
                        this))
                    .ToList();

                rows.Add(amGroup);
            }
            BitRows = rows;
        }

        private int[] GenerateItemMap(byte[] item)
        {
            int[] result = new int[item.Length * 8];
            for (int j = 0; j < item.Length; j++)
            {
                byte currentByte = item[j];
                for (int k = 0; k < 8; k++)
                {
                    result[(j * 8) + k] = (currentByte >> k) & 1;
                }
            }
            return result.Reverse().ToArray();
        }

        private string GenerateItemBitString(byte[] item, int bitsPerRow)
        {
            var itemBits = GenerateItemMap(item).Reverse();

            var sb = new StringBuilder();

            int count = 0;

            foreach (int bit in itemBits)
            {
                sb.Insert(0, bit);
                count++;
                if (count == bitsPerRow)
                {
                    sb.Insert(0, '\n');
                    count = 0;
                }
            }
            return sb.ToString();
        }

        public int[][] GenerateBitMap(byte[] data, int bytesPerItem)
        {
            int[][] result = new int[bytesPerItem * 8][];

            int itemCount = data.Length / bytesPerItem;

            for (int r = 0; r < result.Length; r++)
            {
                result[r] = new int[itemCount];
            }

            int maxBitIndex = (bytesPerItem * 8) - 1;
            int maxItemIndex = itemCount - 1;

            for (int i = 0; i < itemCount; i++)
            {
                for (int j = 0; j < bytesPerItem; j++)
                {
                    int currentByte = data[(i * bytesPerItem) + j];
                    for (int k = 0; k < 8; k++)
                    {
                        result[maxBitIndex - (j * 8) - k][maxItemIndex - i] = (currentByte >> k) & 1;
                    }
                }
            }

            return result;
        }
    }
}
