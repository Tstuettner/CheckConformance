Imports System.Runtime.InteropServices
Imports render_page_to_image.DynaPDF

Module Module1

   Const HORZRES As Integer = 8

   Private Declare Ansi Function GetDC Lib "user32.dll" (ByVal hWnd As IntPtr) As IntPtr
   Private Declare Ansi Function GetDeviceCaps Lib "gdi32.dll" (ByVal hdc As IntPtr, ByVal nIndex As Integer) As Integer
   Private Declare Ansi Function ReleaseDC Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal hdc As IntPtr) As Integer

   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(CPDF.TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.WriteLine("{0}", System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Sub Main()
      Try
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' We do not create a PDF file in this example

         ' Import anything and don't convert pages to templates
         pdf.SetImportFlags(TImportFlags.ifImportAll Or TImportFlags.ifImportAsPage)
         If pdf.OpenImportFile("../../../../../dynapdf_help.pdf", TPwdType.ptOpen, Nothing) < 0 Then
            pdf = Nothing
            Exit Sub
         End If
         ' We must import an external page before we can render it
         pdf.EditPage(1)
            pdf.ImportPageEx(1, 1.0, 1.0)
         pdf.EndPage()

         ' External cmaps should always be loaded when rendering PDF files.
         pdf.SetCMapDir(System.IO.Path.GetFullPath("../../../../../Resource/CMap/"), TLoadCMapFlags.lcmRecursive Or TLoadCMapFlags.lcmDelayed)
         ' Initialize color management.  The default device profile is sRGB if no profile is set.
         Dim p As TPDFColorProfiles = New TPDFColorProfiles()
         p.StructSize = Marshal.SizeOf(p)
         p.DefInCMYKW = System.IO.Path.GetFullPath("../../../../test_files/ISOcoated_v2_bas.ICC")
         pdf.InitColorManagement(p, TPDFColorSpace.csDeviceRGB, TPDFInitCMFlags.icmBPCompensation Or TPDFInitCMFlags.icmCheckBlackPoint)

         ' Get the width of the screen
         Dim dc As IntPtr = GetDC(IntPtr.Zero)
         Dim w As Integer = GetDeviceCaps(dc, HORZRES)
         ReleaseDC(IntPtr.Zero, dc)

         Dim filePath As String = System.IO.Directory.GetCurrentDirectory() + "\out.tif"
         If pdf.RenderPageToImage(1, filePath, 0, w, 0, TRasterFlags.rfDefault, TPDFPixFormat.pxfRGB, TCompressionFilter.cfLZW, TImageFormat.ifmTIFF) Then
            Dim pcs As System.Diagnostics.Process = New System.Diagnostics.Process()
            pcs.StartInfo.FileName = filePath
            pcs.Start()
         End If
         pdf = Nothing
      Catch e As Exception
         Console.Write(e.Message + Chr(10))
         Console.Read()
      End Try
   End Sub

End Module
