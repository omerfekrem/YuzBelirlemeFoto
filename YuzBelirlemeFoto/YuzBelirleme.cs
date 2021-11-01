using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.MachineLearning;
using OpenCvSharp.Blob;
using OpenCvSharp.UserInterface;
using OpenCvSharp.CPlusPlus;
using System.Diagnostics;

namespace YuzBelirlemeFoto
{
    public partial class YuzBelirleme : Form
    {
        public YuzBelirleme()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap image = (Bitmap)Bitmap.FromFile(@"C:\Users\omer_ekrem\Desktop\resim.jpg");
            IplImage imageipl = BitmapConverter.ToIplImage(image);
            img.Image = BitmapConverter.ToBitmap(scanFace(imageipl));
        }

        private IplImage scanFace(IplImage resim) 
        {
            CvColor face = new CvColor(0, 0, 255);
            const double Scale = 3.0;   //hizli islem icin resmin kac kat kucultulecegi
            const double ScaleFactor = 2.5;
            const int MinNeighbors = 1;

            using (IplImage imgMinimize = new IplImage(new CvSize(Cv.Round(resim.Width / Scale), Cv.Round(resim.Height / Scale)), BitDepth.U8, 1))
            {

                using (IplImage gri = new IplImage(resim.Size, BitDepth.U8, 1))   //renksizlestir
                {
                    Cv.CvtColor(resim, gri, ColorConversion.BgrToGray);
                    Cv.Resize(gri, imgMinimize, Interpolation.Linear);
                    Cv.EqualizeHist(imgMinimize, imgMinimize);
                }
                IplImage outImage = Cv.Clone(resim);


                //asil tespit kismina geldik


                using (CvHaarClassifierCascade cascade = CvHaarClassifierCascade.FromFile(Environment.CurrentDirectory + "\\haarcascade_frontalface_alt_tree.xml"))

                using (CvMemStorage storage = new CvMemStorage())


                {
                    storage.Clear();
                    Stopwatch watch = Stopwatch.StartNew();
                    CvSeq<CvAvgComp> faces = Cv.HaarDetectObjects(imgMinimize, cascade, storage, ScaleFactor, MinNeighbors, 0, new CvSize(30, 30));
                    watch.Stop();

                    for (int i = 0; i < faces.Total; i++)

                    {

                        CvRect r = faces[i].Value.Rect;


                        CvPoint center = new CvPoint


                        {


                            X = Cv.Round((r.X + r.Width * 0.5) * Scale),


                            Y = Cv.Round((r.Y + r.Height * 0.5) * Scale)


                        };


                        int radius = Cv.Round((r.Width + r.Height) * 0.25 * Scale);


                        resim.Circle(center, radius, face, 3, LineType.AntiAlias, 0);

                    }

                }
                return resim;
            }

        }
    }
}
