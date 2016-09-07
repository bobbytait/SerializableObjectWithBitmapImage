using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Runtime.Serialization.Formatters.Binary;


namespace SerializeImages
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyData _myData;
        string _filename;
        Version _version;

        public MainWindow()
        {
            InitializeComponent();

            _version = typeof(MainWindow).Assembly.GetName().Version;

            _filename = System.IO.Path.Combine
                (Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "Serialized.dat");
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            _myData = new MyData(_version, "This is a string.", 123.456,
                new BitmapImage(new Uri(@"C:\Users\btait\Downloads\Images\1152.jpg")));

            using (Stream stream = File.Open(_filename, FileMode.Create))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, _myData);
            }
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            image.Source = null;
            _myData = null;

            BitmapSource bitmapSource;

            using (Stream stream = File.Open(_filename, FileMode.Open))
            {
                var binaryFormatter = new BinaryFormatter();
                _myData = (MyData)binaryFormatter.Deserialize(stream);
                PixelFormat pixelFormat = (PixelFormat)new PixelFormatConverter().
                    ConvertFromString(_myData.pixelFormat);

                int stride = (pixelFormat.BitsPerPixel / 8) * _myData.pixelWidth;
                bitmapSource = BitmapSource.Create(
                    _myData.pixelWidth, _myData.pixelHeight,
                    _myData.dpiX, _myData.dpiY,
                    pixelFormat, null,
                    _myData.imageData, stride);
            }

            image.Source = bitmapSource;
        }

        [Serializable]
        public class MyData
        {
            public Version version;
            public string data;
            public double myDouble;

            public byte[] imageData;
            public int pixelWidth;
            public int pixelHeight;
            public double dpiX;
            public double dpiY;
            public string pixelFormat;

            [NonSerialized]
            public BitmapImage bitmapImage;

            public MyData(Version appVersion, string theString, double theDouble,
                BitmapImage theBitmapImage)
            {
                version = appVersion;
                data = theString;
                myDouble = theDouble;
                bitmapImage = theBitmapImage;

                pixelFormat = theBitmapImage.Format.ToString();
                pixelHeight = theBitmapImage.PixelHeight;
                pixelWidth = theBitmapImage.PixelWidth;
                dpiX = theBitmapImage.DpiX;
                dpiY = theBitmapImage.DpiY;

                int stride = (theBitmapImage.Format.BitsPerPixel / 8) * theBitmapImage.PixelWidth;
                int bytes = stride * theBitmapImage.PixelHeight;

                imageData = new byte[bytes];
                bitmapImage.CopyPixels(imageData, stride, 0);
            }
        }
    }
}
