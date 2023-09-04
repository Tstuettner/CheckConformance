Imports signature_ap.DynaPDF

Module Module1
   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.Write("{0}" + Chr(10), System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Sub Main()
      Try
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' The ouput file is opened later

         pdf.SetDocInfo(TDocumentInfo.diCreator, "VB .Net test application")
         pdf.SetDocInfo(TDocumentInfo.diTitle, "Custom digital signature appearance")
         pdf.SetViewerPreferences(TViewerPreference.vpDisplayDocTitle, TViewPrefAddVal.avNone)

         pdf.Append()

         pdf.SetFont("Arial", TFStyle.fsNone, 14.0, True, TCodepage.cp1252)
         pdf.WriteFText(TTextAlign.taLeft, "This file was digitally signed with a self sign certificate." & _
         "The appearance of the signature field is created with normal DynaPDF functions. However, it " & _
         "would also be possible to import a PDF page, an EMF file, or an image into the " & _
         "appearance template." + Chr(10) + Chr(10) & _
         "When creating an individual signature appearance make sure to place the validation icon " & _
         "properly with PlaceSigFieldValidateIcon(). The appearance of the validation icon " & _
         "depends on the Acrobat version with which the file is opened. However, the unscaled size " & _
         "of that icon is always 100.0 x 100.0 Units. It can be scaled to every size you want " & _
         "but it is usually best to preserve the aspect ratio and the icon must be placed fully " & _
         "inside the appearance template.")

         ' ---------------------- Signature field appearance ----------------------
         Dim sigField, sh As Integer
         sigField = pdf.CreateSigField("Signature", -1, 200.0, 500.0, 200.0, 80.0)
         pdf.SetFieldColor(sigField, TFieldColor.fcBorderColor, TPDFColorSpace.csDeviceRGB, CPDF.NO_COLOR)
         ' Place the validation icon on the left side of the signature field.
         pdf.PlaceSigFieldValidateIcon(sigField, 0.0, 15.0, 50.0, 50.0)

         'This function creates a template into which you can draw whatever you want. The template
         'is already opened when calling the function; it must be closed when finish with EndTemplate().
         'An appearance template of a signature field is reserved for this field. It must not be placed
         'on another page!

         'In addition, it makes no sense to create an appearance template when the file is not digitally
         'signed later. Empty signature fields cannot contain a user defined appearance.

         pdf.CreateSigFieldAP(sigField)

         pdf.SaveGraphicState()
         pdf.Rectangle(0.0, 0.0, 200.0, 80.0, TPathFillMode.fmNoFill)
         pdf.ClipPath(TClippingMode.cmWinding, TPathFillMode.fmNoFill)
         sh = pdf.CreateAxialShading(0.0, 0.0, 200.0, 0.0, 0.5, RGB(120, 120, 220), CPDF.PDF_WHITE, True, True)
         pdf.ApplyShading(sh)
         pdf.RestoreGraphicState()

         pdf.SaveGraphicState()
         pdf.Ellipse(50.5, 1.0, 148.5, 78.0, TPathFillMode.fmNoFill)
         pdf.ClipPath(TClippingMode.cmWinding, TPathFillMode.fmNoFill)
         sh = pdf.CreateAxialShading(0.0, 0.0, 0.0, 78.0, 2.0, CPDF.PDF_WHITE, RGB(120, 120, 220), True, True)
         pdf.ApplyShading(sh)
         pdf.RestoreGraphicState()

         pdf.SetFont("Arial", TFStyle.fsBold Or TFStyle.fsUnderlined, 11.0, True, TCodepage.cp1252)
         pdf.SetFillColor(RGB(120, 120, 220))
         pdf.WriteFTextEx(50.0, 60.0, 150.0, -1.0, TTextAlign.taCenter, "Digitally signed by:")
         pdf.SetFont("Arial", TFStyle.fsBold Or TFStyle.fsItalic, 18.0, True, TCodepage.cp1252)
         pdf.SetFillColor(RGB(100, 100, 200))
         pdf.WriteFTextEx(50.0, 45.0, 150.0, -1.0, TTextAlign.taCenter, "DynaPDF")

         pdf.EndTemplate() ' Close the appearance template.

         ' ------------------------------------------------------------------------

         pdf.EndPage() ' Close the open page

         ' No fatal error occurred?
         If pdf.HaveOpenDoc() Then
            ' We write the output file into the current directory.
            Dim filePath As String = System.IO.Directory.GetCurrentDirectory() + "\out.pdf"
            ' OK, now we can open the output file.
            If Not pdf.OpenOutputFile(filePath) Then Return
            If pdf.CloseAndSignFile("../../../test_files/test_cert.pfx", "123456", "Test", Nothing) Then
               Console.Write("PDF file ""{0}"" successfully created!" + Chr(10), filePath)
               Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
               p.StartInfo.FileName = filePath
               p.Start()
            End If
         End If
      Catch e As Exception
         Console.Write(e.Message + Chr(10))
         Console.Read()
      End Try
   End Sub

End Module
