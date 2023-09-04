Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Text
Imports render_page.DynaPDF

Public Class Form1

   <StructLayout(LayoutKind.Sequential, Pack:=0)> _
   Public Structure BITMAPINFOHEADER
      Public biSize As Integer
      Public biWidth As Integer
      Public biHeight As Integer
      Public biPlanes As Int16
      Public biBitCount As Int16
      Public biCompression As Integer
      Public biSizeImage As Integer
      Public biXPelsPerMeter As Integer
      Public biYPelsPerMeter As Integer
      Public biClrUsed As Integer
      Public biClrImportant As Integer

      Public Sub Init()
         biSize = Marshal.SizeOf(Me)
      End Sub
   End Structure

   <StructLayout(LayoutKind.Sequential, Pack:=0)> _
   Public Structure RGBQUAD
      Public rgbBlue As Byte
      Public rgbGreen As Byte
      Public rgbRed As Byte
      Public rgbReserved As Byte
   End Structure

   <StructLayout(LayoutKind.Sequential, Pack:=0)> _
   Class BITMAPINFO
      Public bmiHeader As BITMAPINFOHEADER
      <MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst:=1, ArraySubType:=UnmanagedType.Struct)> _
      Public bmiColors() As RGBQUAD
   End Class

   <StructLayout(LayoutKind.Sequential, Pack:=0)> _
   Class RECT
      Public left As Integer
      Public top As Integer
      Public right As Integer
      Public bottom As Integer
      Public Sub New(ByRef r As Rectangle)
         left = r.Left
         top = r.Top
         right = r.Right
         bottom = r.Bottom
      End Sub
   End Class
   ' ExtTextOut flag
   Const ETO_OPAQUE As Integer = &H2

   ' Window style flags
   Const WS_CAPTION As Integer = &HC00000
   Const WS_SYSMENU As Integer = &H80000
   Const WS_THICKFRAME As Integer = &H40000
   Const WS_MINIMIZEBOX As Integer = &H20000

   ' Class style flags
   Const CS_VREDRAW As Integer = &H1
   Const CS_HREDRAW As Integer = &H2
   Const CS_OWNDC As Integer = &H20

   Const APP_BACK_COLOR As Integer = &H505050
   Const APP_CLIENT_BORDER As Integer = 6
   Const APP_CLIENT_BORDER2 As Integer = 3

   Const DIB_RGB_COLORS As Integer = 0

   Private m_AdjWindow As Boolean
   Private m_BMPInfo As BITMAPINFO
   Private m_BorderX As Integer
   Private m_BorderY As Integer
   Private m_Buffer As IntPtr
   Private m_BufSize As Integer
   Private m_CurrPage As Integer
   Private m_CurrPageObj As IntPtr
   Private m_DC As IntPtr
   Private m_ImgH As Integer
   Private m_ImgW As Integer
   Private m_ImpPages() As Byte
   Private m_PageCount As Integer
   Private m_PDF As CPDF
   Private m_RAS As IntPtr
   Private m_RasImage As TPDFRasterImage
   Private m_RenderThread As Thread
   Private m_ScreenH As Integer
   Private m_ScreenW As Integer
   Private m_Update As Boolean

   Private Declare Auto Function ExtTextOut Lib "gdi32.dll" (ByVal hdc As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal fuOptions As Integer, ByVal lprc As RECT, ByVal lpString As IntPtr, ByVal cbCount As Integer, ByVal lpDx As IntPtr) As Integer
   Private Declare Ansi Function GetDC Lib "user32.dll" (ByVal hWnd As IntPtr) As IntPtr
   Private Declare Unicode Sub GetICMProfileW Lib "gdi32.dll" (ByVal hDC As IntPtr, ByRef Len As Integer, ByVal Filename As System.Text.StringBuilder)
   Private Declare Ansi Function ReleaseDC Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal hdc As IntPtr) As Integer
   Private Declare Ansi Function SetBkColor Lib "gdi32.dll" (ByVal hdc As IntPtr, ByVal crColor As Integer) As Integer
   Private Declare Ansi Function SetDIBitsToDevice Lib "gdi32.dll" (ByVal hdc As IntPtr, ByVal XDest As Integer, ByVal YDest As Integer, ByVal dwWidth As Integer, ByVal dwHeight As Integer, ByVal XSrc As Integer, ByVal YSrc As Integer, ByVal uStartScan As Integer, ByVal cScanLines As Integer, ByVal lpvBits As IntPtr, ByVal lpbmi As BITMAPINFO, ByVal fuColorUse As Integer) As Integer
   Private Declare Ansi Function MoveWindow Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal bRepaint As Integer) As Integer

   ' Error callback function.
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      MessageBox.Show(System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Protected Overrides ReadOnly Property CreateParams() As System.Windows.Forms.CreateParams
      Get
         Dim cp As System.Windows.Forms.CreateParams = MyBase.CreateParams
         ' We need a private dc! The flag CS_OWNDC is required.
         cp.Style = WS_CAPTION Or WS_SYSMENU Or WS_MINIMIZEBOX Or WS_THICKFRAME
         cp.ClassStyle = CS_OWNDC Or CS_VREDRAW Or CS_HREDRAW
         Return cp
      End Get
   End Property

   Public Sub New()
      MyBase.New()
      InitializeComponent()

      m_PDF = New CPDF()
      m_PDF.SetOnErrorProc(AddressOf PDFError)

      m_BMPInfo = New BITMAPINFO()
      m_BMPInfo.bmiHeader.biBitCount = 32
      m_BMPInfo.bmiHeader.biPlanes = 1
      m_BMPInfo.bmiHeader.biSize = 40
      ' Initialize the TRasterImage structure
      m_RasImage = New TPDFRasterImage()
      m_RasImage.StructSize = Marshal.SizeOf(m_RasImage)
      m_RasImage.DefScale = TPDFPageScale.psFitBest
      m_RasImage.InitWhite = 1
      ' We draw the image with SetDIBitsToDevice() and Me function does not support alpha transparency.
      ' To get a correct result we pre-blend the image with a white background.
      m_RasImage.Flags = TRasterFlags.rfDefault Or TRasterFlags.rfCompositeWhite
      m_RasImage.Matrix.a = 1.0
      m_RasImage.Matrix.d = 1.0
      ' The OnUpdateWindow callback function makes sure that we don't see a gray or white screen for a long time when rendering complex pages.
      m_RasImage.OnUpdateWindow = AddressOf OnUpdateWindow
      m_RasImage.UpdateOnImageCoverage = 0.5F
      m_RasImage.UpdateOnPathCount = 1000

      m_ScreenW = Screen.PrimaryScreen.Bounds.Width
      m_ScreenH = Screen.PrimaryScreen.Bounds.Height

      m_BorderX = Width - Me.ClientRectangle.Width + APP_CLIENT_BORDER
      m_BorderY = Height - Me.ClientRectangle.Height + APP_CLIENT_BORDER
   End Sub

   Private Sub AddPage(ByVal PageNum As Integer)
      Dim b As Byte = m_ImpPages(PageNum >> 3)
      b = b Or Convert.ToByte(&H80 >> (PageNum And 7))
      m_ImpPages(PageNum >> 3) = b
   End Sub

   Private Sub Form1_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
      StopRenderThread()
      If m_RAS <> IntPtr.Zero Then m_PDF.DeleteRasterizer(m_RAS)
      If m_Buffer <> IntPtr.Zero Then
         Marshal.FreeHGlobal(m_Buffer)
         m_Buffer = IntPtr.Zero
      End If
      m_PDF = Nothing
   End Sub

   Private Sub Form1_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
      If e.Control And e.KeyCode = Keys.O Then
         OpenFile()
      Else
         Select Case e.KeyCode
            Case Keys.Up
               RenderNextPage(m_CurrPage - 1)
            Case Keys.Down
               RenderNextPage(m_CurrPage + 1)
         End Select
      End If
   End Sub

   Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      ' Required flags to avoid flickering
      Me.SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.ResizeRedraw Or ControlStyles.Opaque, True)
      Me.UpdateStyles()

      m_DC = GetDC(Handle)
      ' Get the monitor profile
      Dim str As StringBuilder = New StringBuilder(512)
      Dim size As Integer = str.Capacity - 1
      Dim dc As IntPtr = GetDC(IntPtr.Zero)
         GetICMProfileW(dc, size, str)
      ReleaseDC(IntPtr.Zero, dc)

      ' It is important to set an absolute path here since a relative path
      ' doesn't work if the working directory will be changed at runtime.
      ' The flag lcmDelayed makes sure that the cmaps will only be loaded
      ' if necessary.
      m_PDF.SetCMapDir(System.IO.Path.GetFullPath("../../../../../../../../Resource/CMap/"), TLoadCMapFlags.lcmRecursive Or TLoadCMapFlags.lcmDelayed)
      ' Initialize color management
      Dim p As TPDFColorProfiles = New TPDFColorProfiles()
      p.StructSize = Marshal.SizeOf(p)
      p.DefInCMYKW = System.IO.Path.GetFullPath("../../../../../../test_files/ISOcoated_v2_bas.ICC")
      p.DeviceProfileW = str.ToString()
      m_PDF.InitColorManagement(p, TPDFColorSpace.csDeviceRGB, TPDFInitCMFlags.icmBPCompensation Or TPDFInitCMFlags.icmCheckBlackPoint)
      str = Nothing
   End Sub

   Private Sub Form1_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseWheel
      If e.Delta < 0 Then
         RenderNextPage(m_CurrPage + 1)
      Else
         RenderNextPage(m_CurrPage - 1)
      End If
   End Sub

   Private Sub Form1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
      'Dim b As SolidBrush = New SolidBrush(Color.FromArgb(APP_BACK_COLOR))
      'e.Graphics.FillRectangle(b, 0, 0, APP_CLIENT_BORDER2, ClientRectangle.Height)
      'e.Graphics.FillRectangle(b, APP_CLIENT_BORDER2, ClientRectangle.Bottom - APP_CLIENT_BORDER2, ClientRectangle.Right - APP_CLIENT_BORDER2, APP_CLIENT_BORDER2)
      'e.Graphics.FillRectangle(b, ClientRectangle.Right - APP_CLIENT_BORDER2, ClientRectangle.Top, APP_CLIENT_BORDER2, ClientRectangle.Height)
      'e.Graphics.FillRectangle(b, APP_CLIENT_BORDER2, ClientRectangle.Top, ClientRectangle.Right - APP_CLIENT_BORDER2, APP_CLIENT_BORDER2)

      ' Plain GDI code is faster as the new GDI+ stuff
      Dim r As RECT = New RECT(ClientRectangle)
      SetBkColor(m_DC, APP_BACK_COLOR)
      ' Left
      r.left = 0
      r.right = APP_CLIENT_BORDER2
      r.bottom = ClientRectangle.Bottom
      r.top = ClientRectangle.Top
      ExtTextOut(m_DC, 0, 0, ETO_OPAQUE, r, IntPtr.Zero, 0, IntPtr.Zero)
      ' Bottom
      r.left = APP_CLIENT_BORDER2
      r.right = ClientRectangle.Right
      r.bottom = ClientRectangle.Bottom
      r.top = ClientRectangle.Bottom - APP_CLIENT_BORDER2
      ExtTextOut(m_DC, 0, 0, ETO_OPAQUE, r, IntPtr.Zero, 0, IntPtr.Zero)
      ' Right
      r.left = APP_CLIENT_BORDER2
      r.right = ClientRectangle.Right
      r.bottom = ClientRectangle.Top + APP_CLIENT_BORDER2
      r.top = ClientRectangle.Top
      ' Top
      ExtTextOut(m_DC, 0, 0, ETO_OPAQUE, r, IntPtr.Zero, 0, IntPtr.Zero)
      r.left = ClientRectangle.Right - APP_CLIENT_BORDER2
      r.right = ClientRectangle.Right
      r.bottom = ClientRectangle.Bottom
      r.top = ClientRectangle.Top
      ExtTextOut(m_DC, 0, 0, ETO_OPAQUE, r, IntPtr.Zero, 0, IntPtr.Zero)
      If m_Update Then
         If IsNothing(m_RenderThread) Then
            StartRenderThread()
         End If
      ElseIf m_CurrPage > 0 Then
         SetDIBitsToDevice(m_DC, APP_CLIENT_BORDER2, APP_CLIENT_BORDER2, m_ImgW, m_ImgH, 0, 0, 0, m_ImgH, m_Buffer, m_BMPInfo, 0)
      End If
   End Sub

   Private Sub Form1_SizeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.SizeChanged
      Dim r As RECT = New RECT(ClientRectangle)
      SetBkColor(m_DC, APP_BACK_COLOR)
      ExtTextOut(m_DC, 0, 0, ETO_OPAQUE, r, IntPtr.Zero, 0, IntPtr.Zero)
      m_AdjWindow = True
      RenderCurrPage()
   End Sub

   Private Sub Form1_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
      OpenFile()
   End Sub

   Private Function IsPageAvailable(ByVal PageNum As Integer) As Boolean
      Return (m_ImpPages(PageNum >> 3) And (&H80 >> (PageNum And 7))) <> 0
   End Function

   Private Function OnUpdateWindow(ByVal Data As IntPtr, ByRef Area As TIntRect) As Integer
      SetDIBitsToDevice(m_DC, _
                        Area.x1 + APP_CLIENT_BORDER2, _
                        Area.y1 + APP_CLIENT_BORDER2, _
                        Area.x2 - Area.x1, _
                        Area.y2 - Area.y1, _
                        Area.x1, _
                        m_ImgH - Area.y2, _
                        0, _
                        m_ImgH, _
                        m_Buffer, _
                        m_BMPInfo, _
                        DIB_RGB_COLORS)
      Return 0
   End Function

   Private Sub OpenFile()
      Me.Text = "RenderPage()"
      Dim r As RECT = New RECT(ClientRectangle)
      SetBkColor(m_DC, APP_BACK_COLOR)
      ExtTextOut(m_DC, 0, 0, ETO_OPAQUE, r, IntPtr.Zero, 0, IntPtr.Zero)
      m_CurrPage = 0
      m_PageCount = 0
      StopRenderThread()
      If OpenFileDialog.ShowDialog() = DialogResult.OK Then
         Me.Text = OpenFileDialog.FileName
         If m_PDF.HaveOpenDoc() Then m_PDF.FreePDF()
         m_PDF.CreateNewPDF(Nothing) ' We create no PDF file in this example

         If m_PDF.OpenImportFile(OpenFileDialog.FileName, TPwdType.ptOpen, Nothing) < 0 Then Return

         ' We import pages manually in this example and therefore, no global objects will
         ' be imported as it would be the case if ImportPDFFile() would be used.
         ' However, the one and only thing we need is the output intent for correct
         ' color management. Anything else can be discarded.
         m_PDF.SetImportFlags(TImportFlags.ifContentOnly)
         m_PDF.ImportCatalogObjects()

         m_PDF.SetImportFlags(TImportFlags.ifImportAll Or TImportFlags.ifImportAsPage) ' The flag ifImportAsPage makes sure that pages will not be converted to templates.
         m_PDF.SetImportFlags2(TImportFlags2.if2UseProxy)                              ' The flag if2UseProxy reduces the memory usage.

         m_PageCount = m_PDF.GetInPageCount()
         Erase m_ImpPages
         ReDim m_ImpPages(m_PageCount >> 3)

         m_AdjWindow = True
         m_CurrPage = 1
         RenderCurrPage()
      End If
   End Sub

   Private Sub RenderCurrPage()
      StopRenderThread()
      If m_CurrPage > 0 Then
         Dim w As Integer = 0, h As Integer = 0
         If Not IsPageAvailable(m_CurrPage) Then
            ' No need to check the return value of ImportPageEx(). Nothing critical happens
            ' if the function fails. We get just an empty page in this case.
            m_PDF.EditPage(m_CurrPage)
               m_PDF.ImportPageEx(m_CurrPage, 1.0, 1.0)
            m_PDF.EndPage()
            AddPage(m_CurrPage)
         End If
         m_CurrPageObj = m_PDF.GetPageObject(m_CurrPage)
         ' This check is required to avoid critical errors.
         If m_CurrPageObj = IntPtr.Zero Then Return
         m_PDF.CalcPagePixelSize(m_CurrPageObj, TPDFPageScale.psFitBest, 1.0F, m_ScreenW - m_BorderX, m_ScreenH - m_BorderY, m_RasImage.Flags, w, h)
         If w <> m_ImgW OrElse h <> m_ImgH Then
            m_ImgW = w
            m_ImgH = h

            m_BufSize = (m_ImgW << 2) * m_ImgH
            If m_Buffer <> IntPtr.Zero Then Marshal.FreeHGlobal(m_Buffer)
            m_Buffer = Marshal.AllocHGlobal(m_BufSize)
            If m_RAS = IntPtr.Zero Then
               m_RAS = m_PDF.CreateRasterizer(IntPtr.Zero, m_Buffer, m_ImgW, m_ImgH, m_ImgW << 2, TPDFPixFormat.pxfBGRA)
               If m_RAS = IntPtr.Zero Then Return
            Else
               If Not m_PDF.AttachImageBuffer(m_RAS, IntPtr.Zero, m_Buffer, m_ImgW, m_ImgH, m_ImgW << 2) Then Return
            End If
            UpdateBitmapInfo()
         End If
         If m_AdjWindow Then
            w = m_ImgW + m_BorderX
            h = m_ImgH + m_BorderY
            If ClientRectangle.Width - APP_CLIENT_BORDER <> m_ImgW OrElse ClientRectangle.Height - APP_CLIENT_BORDER <> m_ImgH Then
               m_AdjWindow = False
               MoveWindow(Me.Handle, (m_ScreenW - w) >> 1, (m_ScreenH - h) >> 1, w, h, 1)
            End If
         End If
         m_Update = True
         Invalidate()
      End If
   End Sub

   Private Sub RenderNextPage(ByVal PageNum As Integer)
      If PageNum < 1 OrElse PageNum > m_PageCount OrElse PageNum = m_CurrPage Then Return
      m_CurrPage = PageNum
      m_AdjWindow = True
      RenderCurrPage()
   End Sub

   Private Sub RenderPageFunc()
      m_PDF.RenderPage(m_CurrPageObj, m_RAS, m_RasImage)
      m_Update = False
   End Sub

   Private Sub StartRenderThread()
      StopRenderThread()
      m_Update = False
      m_RenderThread = New Thread(AddressOf RenderPageFunc)
      m_RenderThread.Priority = ThreadPriority.Lowest
      m_RenderThread.Start()
   End Sub

   Private Sub StopRenderThread()
      If Not IsNothing(m_RenderThread) Then
         m_PDF.Abort(m_RAS)
         m_RenderThread.Join()
         m_RenderThread = Nothing
      End If
   End Sub

   Private Sub UpdateBitmapInfo()
      m_BMPInfo.bmiHeader.biSizeImage = m_ImgH * m_ImgW * 4
      m_BMPInfo.bmiHeader.biWidth = m_ImgW
      m_BMPInfo.bmiHeader.biHeight = -m_ImgH
   End Sub

End Class
