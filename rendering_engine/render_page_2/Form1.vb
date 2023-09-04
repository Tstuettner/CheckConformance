Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Text
Imports render_page_2.DynaPDF

Public Class Form1

   Private m_PDF As CPDF

   ' Error callback function.
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      MessageBox.Show(System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      m_PDF = New CPDF()
      m_PDF.SetOnErrorProc(AddressOf PDFError)
      ' Initialize color management (optional but recommended). The default device profile is sRGB if no profile is set.
      Dim p As TPDFColorProfiles = New TPDFColorProfiles()
      p.StructSize = Marshal.SizeOf(p)
      p.DefInCMYKW = System.IO.Path.GetFullPath("../../../../../../test_files/ISOcoated_v2_bas.ICC")
      m_PDF.InitColorManagement(p, TPDFColorSpace.csDeviceRGB, TPDFInitCMFlags.icmBPCompensation Or TPDFInitCMFlags.icmCheckBlackPoint)

      ' We don't create a PDF file in this example
      m_PDF.CreateNewPDF(Nothing)

      m_PDF.SetPageCoords(TPageCoord.pcTopDown)
      m_PDF.Append()
         m_PDF.SetFont("Arial", TFStyle.fsRegular, 20.0, False, TCodepage.cpUnicode)
         m_PDF.WriteFTextEx(50.0, 50.0, 495.0, -1.0, TTextAlign.taCenter, "A small example that shows how to render into a Bitmap.")
      m_PDF.EndPage()
   End Sub

   Private Sub Form1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
      If IsNothing(m_PDF) Then Exit Sub
      ' Get the page object
      Dim pagePtr As IntPtr = m_PDF.GetPageObject(1)

      Dim w As Integer = PictureBox1.Width  ' The picture box was placed on the form.
      Dim h As Integer = PictureBox1.Height

      ' Calculate the image size
      m_PDF.CalcPagePixelSize(pagePtr, TPDFPageScale.psFitBest, 1.0F, w, h, TRasterFlags.rfDefault, w, h)

      ' Create a bitmap in this size
      Dim bmp As Bitmap = New Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
      Dim bd As System.Drawing.Imaging.BitmapData = bmp.LockBits(New Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb)

      ' Create a rasterizer for the bitmap
      Dim ras As IntPtr = m_PDF.CreateRasterizer(IntPtr.Zero, bd.Scan0, w, h, bd.Stride, DynaPDF.TPDFPixFormat.pxfBGRA)

      Dim img As TPDFRasterImage = New DynaPDF.TPDFRasterImage()
      img.InitWhite = 1
      img.DefScale = TPDFPageScale.psFitBest
      img.Flags = TRasterFlags.rfInitBlack Or TRasterFlags.rfCompositeWhite ' A 32 bit image has a transparent background. The flag rfCompositeWhite makes sure that the image gets pre-blended with a white background.
      img.Matrix.a = 1.0 ' Identity matrix
      img.Matrix.d = 1.0 ' Identity matrix

      ' Render the page
      m_PDF.RenderPage(pagePtr, ras, img)

      bmp.UnlockBits(bd)
      PictureBox1.Image = bmp
      PictureBox1.SizeMode = PictureBoxSizeMode.CenterImage

      m_PDF.DeleteRasterizer(ras)
   End Sub

End Class
