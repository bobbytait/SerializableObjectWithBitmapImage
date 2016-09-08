using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;


namespace SerializeImages
{
	public partial class MainWindow : Window
	{
		Version _version;

		public MainWindow()
		{
			InitializeComponent();

			// Get the app's version number as part of the mixed data we're serializing
			_version = typeof(MainWindow).Assembly.GetName().Version;
		}

		// Create & save the data object as serialized binary data
		private void saveButton_Click(object sender, RoutedEventArgs e)
		{
			// Create the serialized data object and save it
			MyData myData = new MyData(_version, "This is a string.", 123.456,
				new BitmapImage(new Uri(@"..\..\..\1152.jpg", UriKind.Relative)));

			using (Stream stream = File.Open(@"..\..\..\serialized.bin", FileMode.Create))
			{
				var binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(stream, myData);
			}
		}

		// Load the serialized binary data file to our object and display the image
		private void loadButton_Click(object sender, RoutedEventArgs e)
		{
			image.Source = null;
			MyData myData = null;

			using (Stream stream = File.Open(@"..\..\..\serialized.bin", FileMode.Open))
			{
				var binaryFormatter = new BinaryFormatter();
				myData = (MyData)binaryFormatter.Deserialize(stream);
			}

			image.Source = myData.MySerializedBitmapImage.Deserialize();
			versionData.Text = myData.MyVersion.ToString();
			stringData.Text = myData.MyString;
			doubleData.Text = myData.MyDouble.ToString();
		}

		private void closeButton_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		////////////////////////////////////////////////////////////////////////
		// Class we want to serialize

		[Serializable]
		public class MyData
		{
			public Version MyVersion;
			public string MyString;
			public double MyDouble;
			public SerializedBitmapImage MySerializedBitmapImage;

			public MyData(Version myVersion, string myString, double myDouble,
				BitmapImage myBitmapImage)
			{
				MyVersion = myVersion;
				MyString = myString;
				MyDouble = myDouble;
				MySerializedBitmapImage = new SerializedBitmapImage();
				MySerializedBitmapImage.Serialize(myBitmapImage);
			}
		}

		////////////////////////////////////////////////////////////////////////
		// BitmapImage broken down into serializable data

		[Serializable]
		public class SerializedBitmapImage
		{
			public byte[] _imagePixelData;
			public int _imagePixelWidth;
			public int _imagePixelHeight;
			public double _imageDpiX;
			public double _imageDpiY;
			private string _imagePixelFormat;

			public SerializedBitmapImage() { }

			public void Serialize(BitmapImage bitmapImage)
			{
				_imagePixelFormat = bitmapImage.Format.ToString();
				_imagePixelWidth = bitmapImage.PixelWidth;
				_imagePixelHeight = bitmapImage.PixelHeight;
				_imageDpiX = bitmapImage.DpiX;
				_imageDpiY = bitmapImage.DpiY;

				int stride = (bitmapImage.Format.BitsPerPixel / 8) * bitmapImage.PixelWidth;
				int bytes = stride * bitmapImage.PixelHeight;

				_imagePixelData = new byte[bytes];
				bitmapImage.CopyPixels(_imagePixelData, stride, 0);
			}

			public BitmapSource Deserialize()
			{
				PixelFormat pixelFormat = (PixelFormat)new PixelFormatConverter().ConvertFromString(_imagePixelFormat);
				int stride = (pixelFormat.BitsPerPixel / 8) * _imagePixelWidth;

				return BitmapSource.Create(_imagePixelWidth, _imagePixelHeight, _imageDpiX,
					_imageDpiY, pixelFormat, null, _imagePixelData, stride);
			}
		}
	}
}
