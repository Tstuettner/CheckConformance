Imports personalize.DynaPDF

Module Module1
   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.Write("{0} + chr(10)", System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Sub Main()
      Try
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' The ouput file is opened later

         pdf.SetDocInfo(TDocumentInfo.diCreator, "VB .Net test application")
         pdf.SetDocInfo(TDocumentInfo.diTitle, "How to edit PDF files")
         pdf.SetViewerPreferences(TViewerPreference.vpDisplayDocTitle, TViewPrefAddVal.avNone)

         ' Conversion of pages to templates is normally not required. Templates are required if
         ' the page should be scaled or used multiple times in a document, e.g. as a page background.
         ' See help file for further information.
         pdf.SetImportFlags(TImportFlags.ifImportAll Or TImportFlags.ifImportAsPage)
         If (pdf.OpenImportFile("../../../test_files/taxform.pdf", TPwdType.ptOpen, Nothing) < 0) Then
            Console.Write("Input file ""../../../test_files/taxform.pdf"" not found!" + Chr(10))
            Console.Read()
            Exit Sub
         End If
         pdf.ImportPDFFile(1, 1.0, 1.0)
         pdf.CloseImportFile()

         pdf.EditPage(1)
         ' We use the 14 standard fonts only because they are always available. However, in a real
         ' project you should use fonts which can be embedded.
         pdf.SetFont("Courier", TFStyle.fsBold, 14.0, False, TCodepage.cp1252)

         pdf.WriteText(72.5, 748.5, "X")
         pdf.WriteText(74.0, 701.0, "Musterstadt")
         pdf.WriteText(74.0, 677.0, "252/1062/3323")

         pdf.BeginContinueText(74.0, 628.0)
         pdf.SetLeading(24.0)
         pdf.SetCharacterSpacing(5.8)
         pdf.AddContinueText("Mustermann")
         pdf.AddContinueText("Hermann")
         pdf.AddContinueText("22021963keineKaufmann")
         pdf.AddContinueText("Musterstraße 145")
         pdf.AddContinueText("12345Musterstadt")
         pdf.SetCharacterSpacing(0.0)
         pdf.SetFont("Courier", TFStyle.fsBold, 10.0, False, TCodepage.cp1252)
         pdf.SetLeading(47.5)
         pdf.AddContinueText("04.05.1994")
         pdf.SetFont("Courier", TFStyle.fsBold, 14.0, False, TCodepage.cp1252)
         pdf.SetCharacterSpacing(5.8)
         pdf.AddContinueText("Sabine")
         pdf.AddContinueText("18121966 ev  Hausfrau")
         pdf.EndContinueText()

         pdf.WriteText(72.5, 365.0, "X")
         pdf.WriteText(396.0, 365.0, "X")

         pdf.BeginContinueText(74.0, 316.0)
         pdf.SetLeading(24.0)
         pdf.AddContinueText("2346256780     76834560")
         pdf.AddContinueText("Sparkasse Musterstadt")
         pdf.EndContinueText()

         pdf.WriteText(72.5, 269.0, "X")
         pdf.SetCharacterSpacing(0.0)
         pdf.SetFont("Courier", TFStyle.fsNone, 10.0, False, TCodepage.cp1252)

         pdf.WriteText(53.0, 48.0, DateTime.Now.ToString())
         pdf.SetFillColor(RGB(&HFF, &H66, &H66))
         pdf.SetFont("Helvetica", TFStyle.fsBold, 22.0, False, TCodepage.cp1252)
         pdf.WriteText(340, 70, "www.dynaforms.de")
         pdf.SetLineWidth(0.0)
         pdf.SetLinkHighlightMode(THighlightMode.hmPush)
         pdf.SetAnnotFlags(TAnnotFlags.afReadOnly)
         pdf.WebLink(340, 64, 204, 22, "http:'www.dynaforms.de")
         pdf.EndPage()

         ' No fatal error occurred?
         If pdf.HaveOpenDoc() Then
            ' We write the output file into the current directory.
            Dim filePath As String = System.IO.Directory.GetCurrentDirectory() + "\out.pdf"
            ' OK, now we can open the output file.
            If Not pdf.OpenOutputFile(filePath) Then Return
            If pdf.CloseFile() Then
               Console.Write("PDF file ""{0}"" successfully created!" + Chr(10), filePath)
               Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
               p.StartInfo.FileName = filePath
               p.Start()
            End If
         End If
      Catch e As Exception
         Console.Write(e.Message + Chr(10))
      End Try
      Console.Read()
   End Sub

End Module
