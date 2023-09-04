Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Windows.Forms
Imports print_pdf.DynaPDF

Module Module1

   Private Declare Auto Function CreateDC Lib "gdi32.dll" (ByVal lpszDriver As String, ByVal lpszDevice As String, ByVal lpszOutput As IntPtr, ByVal lpInitData As IntPtr) As IntPtr
   Private Declare Ansi Function DeleteDC Lib "gdi32.dll" (ByVal hdc As IntPtr) As Integer

   Private Function GetPrinterDC() As IntPtr
      Dim d As PrintDialog = New PrintDialog()
      d.AllowCurrentPage = False
      d.AllowPrintToFile = False
      d.AllowSelection = False
      d.AllowSomePages = False
      d.UseEXDialog = True ' Required in 64 bit apps
      If d.ShowDialog() = DialogResult.OK Then
         Return CreateDC(Nothing, d.PrinterSettings.PrinterName, IntPtr.Zero, IntPtr.Zero)
      Else
         Return IntPtr.Zero
      End If
   End Function

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
         ' We print only the first page in this example.
         pdf.EditPage(1)
            pdf.ImportPageEx(1, 1.0, 1.0)
         pdf.EndPage()

         ' If the file contains layers then ApplyAppEvent() makes sure that the same result will be printed
         ' that Adobe's Acrobat would print. However, you can also print the view or export state if you want.
         ' The help file doesn't contain layers and this command does just nothing but in a real world
         ' application ApplyAppEvent() should be called.
         pdf.ApplyAppEvent(TOCAppEvent.aePrint, False)

         Dim dc As IntPtr = GetPrinterDC()
         If dc <> IntPtr.Zero Then
            Dim margin As TRectL = New TRectL()
            If pdf.PrintPDFFile(Nothing, "Test Print", dc, TPDFPrintFlags.pffDefault Or TPDFPrintFlags.pffAutoRotateAndCenter Or TPDFPrintFlags.pffShrinkToPrintArea, margin) Then
               Console.Write("Page 1 successfully printed\n")
            End If
            DeleteDC(dc)
         End If
      Catch e As Exception
         Console.Write(e.Message + Chr(10))
         Console.Read()
      End Try
   End Sub

End Module
