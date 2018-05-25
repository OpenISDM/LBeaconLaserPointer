using LBeacon.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace LBeacon.Class
{
    public class Barcode
    {
        private static BarcodeWriter BarcodeGenerator = new BarcodeWriter()
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions()
            {
                ErrorCorrection = ErrorCorrectionLevel.H,
                Margin = 0,
                Width = 300,
                Height = 300,
                CharacterSet = "UTF-8"
            }
        };

        public static Bitmap QRcode(string Content)
        {
            using (Bitmap QRCode = BarcodeGenerator.Write(Content))
            {
                using (Image Logo = Resources._30784970_1004515239698589_1166439437_n)
                {
                    int middleImgW = Math.Min((int)(QRCode.Width / 4), Logo.Width);
                    int middleImgH = Math.Min((int)(QRCode.Height / 4), Logo.Height);
                    int middleImgL = Convert.ToInt16((Logo.Width - middleImgW) / 2.2);
                    int middleImgT = Convert.ToInt16((Logo.Height - middleImgH) / 2.8);

                    Bitmap LogoQRCode = new Bitmap(QRCode.Width, QRCode.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(LogoQRCode))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.DrawImage(QRCode, 0, 0);
                    }

                    using (Graphics MyGraphic = Graphics.FromImage(LogoQRCode))
                    {
                        MyGraphic.DrawImage(Logo, middleImgL, middleImgT, middleImgW, middleImgH);
                    }

                    return LogoQRCode;
                }
            }
        }

        public static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                img.Dispose();
                return stream.ToArray();
            }
        }
    }
}