using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.Design {
    public class DesignModel {
        ObservableCollection<TileSet> tileSets_ = new ObservableCollection<TileSet>();

        public ObservableCollection<TileSet> TileSets { get { return tileSets_; } }
    }

    public class TileTheme : BaseClass {
        string name_;

        public string Name { get { return name_; } set { name_ = value; OnPropertyChanged("Name"); } }
    }

    public class Tile : BaseClass {
        TileTheme theme_;

        public TileTheme Theme { get { return theme_; } set { theme_ = value; OnPropertyChanged("Theme"); } }
    }

    public class TileChunk : NamedBaseClass {
        int width_;
        int height_;
        bool isSetPiece_;
        bool isWholePlace_;
        ObservableCollection<PropLayer> layers_ = new ObservableCollection<PropLayer>();

        public ObservableCollection<PropLayer> Layers { get { return layers_; } }
        public int Width { get { return width_; } set { width_ = value; OnPropertyChanged("Width"); } }
        public int Height { get { return height_; } set { height_ = value; OnPropertyChanged("Height"); } }
        public bool IsSetPiece { get { return isSetPiece_; } set { isSetPiece_ = value; OnPropertyChanged("IsSetPiece"); } }
        public bool IsMap { get { return isWholePlace_; } set { isWholePlace_ = value; OnPropertyChanged("IsMap"); } }
    }

    public class TileSet : NamedBaseClass {
        ObservableCollection<TileChunk> tiles_ = new ObservableCollection<TileChunk>();

        public ObservableCollection<TileChunk> TileChunks { get { return tiles_; } }
    }

    public class Prop : BaseClass {
        float x_ = 0;
        float y_ = 0;
        PropDefinition kind_;

        public PropDefinition PropKind { 
            get { return kind_; } 
            set { kind_ = value; OnPropertyChanged("PropKind"); } 
        }
        public float X { get { return x_; } set { x_ = value; OnPropertyChanged("X"); } }
    }

    public class PropDefinition : NamedBaseClass {

    }

    public class PropLayer : NamedBaseClass {
        ObservableCollection<Prop> props_ = new ObservableCollection<Prop>();

        public ObservableCollection<Prop> Props { get { return props_; } }
    }
}
