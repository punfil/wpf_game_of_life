using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace wpf_game_of_life.Game
{
    public class CellHealthColorConventer : IValueConverter
    {
        public SolidColorBrush AliveColour { get; private set; }
        public SolidColorBrush DeadColour { get; private set; }

        public CellHealthColorConventer(SolidColorBrush aliveColour, SolidColorBrush deadColour)
        {
            AliveColour = aliveColour;
            DeadColour = deadColour;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool IsAlive = false;

            if (value is bool)
                IsAlive = (bool)value;

            return IsAlive ? AliveColour : DeadColour;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush)
                return ((SolidColorBrush)value) == AliveColour;

            return false;
        }
    }
}
