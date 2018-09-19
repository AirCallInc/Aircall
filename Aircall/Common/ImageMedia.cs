using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public class ImageMedia
{
    private BitmapSource _source;
    private byte[] _data;
    private string _copyright;

    public int Width { get { return _source.PixelWidth; } }
    public int Height { get { return _source.PixelHeight; } }
    public string Copyright { get { return _copyright; } }
	
	/// <summary>
	/// The default format this image will be saved as. For png or gif files the default will be png, for all other image files the default will be jpg.
	/// </summary>
	public string Extension { get; private set; }

    /// <summary>
    /// Resizes image to fit within the specified with and height, aspect ratio is maintained.
    /// </summary>
    /// <param name="width">The maximum width the image has to fit within, set to null to not restrict width.</param>
    /// <param name="height">The maximum height the image has to fit within, set to null to not restrict height.</param>
    /// <returns>A reference to this object to allow chaining operations together.</returns>
    public ImageMedia Resize(int? width, int? height)
    {
        if (width == null && height == null)
            return this;

        width = width ?? _source.PixelWidth;
        height = height ?? _source.PixelHeight;
        double scale = Math.Min(width.Value / (double)_source.PixelWidth, height.Value / (double)_source.PixelHeight);
        if (scale >= 1)
            return this;

        _source = new TransformedBitmap(_source, new ScaleTransform(scale, scale, 0, 0));
        _data = null;
        return this;
    }

    /// <summary>
    /// Crops the image in the middle resizing it down to fit as much of the image as possible. Note, image is not enlarged to fit desired width and height.
    /// </summary>
    /// <param name="width">The desired width to crop the image to, set to null to not perform horizontal cropping.</param>
    /// <param name="height">The desired height to crop the image to, set to null to not perform vertical cropping.</param>
    /// <returns>A reference to this object to allow chaining operations together.</returns>
    public ImageMedia Crop(int? width, int? height)
    {
        if ((width == null && height == null) || (width >= _source.PixelWidth && height >= _source.PixelHeight))
            return this;

        width = Math.Min(width ?? _source.PixelWidth, _source.PixelWidth);
        height = Math.Min(_source.PixelHeight, (height ?? _source.PixelHeight));
        double scale = Math.Max(width.Value / (double)_source.PixelWidth, height.Value / (double)_source.PixelHeight);
        if (scale < 1)
            _source = new TransformedBitmap(_source, new ScaleTransform(scale, scale, 0, 0));

        double marginX = (_source.PixelWidth - width.Value) / 2;
        double marginY = (_source.PixelHeight - height.Value) / 2;

        _source = new CroppedBitmap(_source, new System.Windows.Int32Rect((int)marginX, (int)marginY, width.Value, height.Value));
        _data = null;
        return this;
    }

    /// <summary>
    /// Returns the binary data of the image as either jpg or png as specified by the Extension property.
    /// </summary>
    public byte[] ToByteArray()
    {
        return ToByteArray(Extension);
    }

    /// <summary>
    /// Returns the binary data of the image in the specifed format.
    /// </summary>
    /// <param name="format">The format of the bitmap image specified as "gif", "jpg", "png", "bmp", "jxr" or "tiff".</param>
    public byte[] ToByteArray(string format)
    {
        //if image not altered return orig data
        if (_data != null)
            return _data;

        //placed in a thread set to STA to overcome 0xC0000005 thrown in encoder.Save
        if (!String.IsNullOrEmpty(Copyright))
        {
            byte[] data = null;
            System.Threading.Thread worker = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(delegate (object obj) {
                BitmapEncoder encoder = GetEncoder(format);
                try
                {
                    BitmapMetadata meta = new BitmapMetadata(format);
                    meta.Copyright = Copyright;
                    encoder.Frames.Insert(0, BitmapFrame.Create(_source, null, meta, null)); //using insert because we're getting error about IList not containing Add method
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        encoder.Save(memoryStream);
                        data = memoryStream.ToArray();
                    }
                }
                catch (NotSupportedException) { } //specified format does not support copyright metadata
            }));
            worker.SetApartmentState(System.Threading.ApartmentState.STA);
            worker.Start();
            worker.Join();
            if (data != null)
                return data;
        }

        BitmapEncoder targetEncoder = GetEncoder(format);
        targetEncoder.Frames.Add(BitmapFrame.Create(_source));
        using (MemoryStream memoryStream = new MemoryStream())
        {
            targetEncoder.Save(memoryStream);
            return memoryStream.ToArray();
        }
    }

    private BitmapEncoder GetEncoder(string format)
    {
        if (format == "jpg")
        {
            JpegBitmapEncoder res = new JpegBitmapEncoder();
            res.QualityLevel = 85;
            return res;
        }
        if (format == "png")
            return new PngBitmapEncoder();
        if (format == "gif")
            return new GifBitmapEncoder();
        if (format == "tiff")
            return new TiffBitmapEncoder();
        if (format == "jxr")
            return new WmpBitmapEncoder();
        if (format == "bmp")
            return new BmpBitmapEncoder();
        throw new Exception("Unrecognised image format: " + format);
    }

    /// <summary>
    /// Create a image object that can be interacted with from binary data.
    /// </summary>
    /// <exception cref="System.NotSupportedException">If the supplied binary data is not a valid image, the NotSupportedException will be thrown.</exception>
    public static ImageMedia Create(byte[] data)
    {
    


        ImageMedia result = new ImageMedia();
		var decoder = BitmapDecoder.Create(new MemoryStream(data), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
		result.Extension = (decoder.CodecInfo.FriendlyName == "GIF Decoder" || decoder.CodecInfo.FriendlyName == "PNG Decoder") ? "png" : "jpg";
		result._source = decoder.Frames[0];
        result._source = BitmapDecoder.Create(new MemoryStream(data), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None).Frames[0];
        result._data = data;
        try { result._copyright = ((BitmapMetadata)result._source.Metadata).Copyright; }
        catch (Exception) { }
        return result;
    }
}