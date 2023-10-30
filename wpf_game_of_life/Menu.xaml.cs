using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wpf_game_of_life
{
    /// <summary>
    /// Logika interakcji dla klasy Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        public void StartGame(object sender, RoutedEventArgs e)
        {
            var gridSize = 0;

            try
            {
                gridSize = Int32.Parse(this.gridSize.Text);
                if (gridSize <= 0)
                {
                    throw (new FormatException("Rozmiar powinien być większy od zera"));
                }
            } catch (ArgumentNullException ex)
            {
                this.errorText.Text = "Pusty rozmiar siatki: " + ex.Message;
                return;
            }
            catch (FormatException ex)
            {
                this.errorText.Text = "Nieprawidłowy rozmiar siatki: " + ex.Message;
                return;
            }
            catch (OverflowException ex)
            {
                this.errorText.Text = "Za duży rozmiar siatki: " + ex.Message;
                return;
            }

            MainWindow game = new MainWindow(gridSize);
            game.Show();
            this.Close();
        }

        public void QuitGame(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown(0);
        }
    }
}
