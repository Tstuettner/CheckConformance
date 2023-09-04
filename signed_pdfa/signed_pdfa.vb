Imports signed_pdfa.DynaPDF

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

         pdf.SetFont("Arial", TFStyle.fsNone, 10.0, True, TCodepage.cp1252)
         pdf.WriteFText(TTextAlign.taLeft, "This is a PDF/A 1b compatible PDF file that was digitally signed with " & _
         "a self sign certificate. Because PDF/A requires that all fonts are embedded it is important " & _
         "to avoid the usage of the 14 Standard fonts." & Chr(13) & Chr(13) & _
         "When signing a PDF/A compliant PDF file with the default settings (without creation of a user " & _
         "defined appearance) the font Arial must be available on the system because it is used to print " & _
         "the certificate properties into the signature field." & Chr(13) & Chr(13) & _
         "The font Arial must also be available if an empty signature field was added to the file " & _
         "without signing it when closing the PDF file. Yes, it is still possible to sign a PDF/A " & _
         "compliant PDF file later with Adobe's Acrobat. The signed PDF file is still compatible " & _
         "to PDF/A. If you use a third party solution to digitally sign the PDF file then test " & _
         "whether the signed file is still valid with the PDF/A 1b preflight tool included in Acrobat 8 " & _
         "Professional." & Chr(13) & Chr(13) & _
         "Signature fields must be visible and the print flag must be set (default). CheckConformance() " & _
         "adjusts these flags if necessary and produces a warning if changes were applied. If no changes " & _
         "should be allowed, just return -1 in the error callback function. If the error callback function " & _
         "returns 0, DynaPDF assumes that the prior changes were accepted and processing continues." & Chr(13) & Chr(13) & _
         "\FC[255]Notice:\FC[0]" & Chr(13) & _
         "It makes no sense to execute CheckConformance() without an error callback function or error event " & _
         "in VB. If you cannot see what happens during the execution of CheckConformance(), it is " & _
         "completely useless to use this function!" & Chr(13) & Chr(13) & _
         "CheckConformance() should be used to find the right settings to create PDF/A compatible PDF files. " & _
         "Once the the settings were found it is usually not longer recommended to execute this function. " & _
         "However, it is of course possible to use CheckConformance() as a general approach to make sure " & _
         "that files created with DynaPDF are PDF/A compatible.")

         ' ---------------------- Signature field appearance ----------------------
         Dim sigField, sh As Integer
         sigField = pdf.CreateSigField("Signature", -1, 200.0, 400.0, 200.0, 80.0)
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
         ' Check whether the file is compatible to PDF/A 1b
         Select Case pdf.CheckConformance(TConformanceType.ctPDFA_1b_2005, 0, IntPtr.Zero, Nothing, Nothing)
            Case 1 And 3 ' RGB, Gray
               pdf.AddOutputIntent("../../../test_files/sRGB.icc")
            Case 2 ' CMYK
               pdf.AddOutputIntent("../../../test_files/ISOcoated_v2_bas.ICC") ' Thisis just an example profile that can be delivered with DynaPDF.
         End Select
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
