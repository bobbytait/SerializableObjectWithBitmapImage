# SerializableObjectWithBitmapImage
How to serialize an object containing a BitmapImage in C#

We were collecting a number of images (17-24) from a device to our tablet over a wifi connection, and then displaying them in a custom UserControl as BitmapImages.

Then we decided we also wanted to store them (and some related data) on our tablet for later use as one large block.

We found out that serializing the block wouldn't work due to BitmapImages being unserializable.

I wrote this little app to figure out how to serialize a BitmapImage without having to run our entire larger app.

Pardon the lack of error-checking. :)
