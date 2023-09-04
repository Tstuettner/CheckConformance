Imports layers.DynaPDF

Module Module1

   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(CPDF.TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.WriteLine("{0}", System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Sub Main()
      Try
         Dim annot As Integer, ocmd As Integer, oc1 As Integer, oc2 As Integer, oc3 As Integer
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' We do not create a PDF file in this example

            pdf.SetPageCoords(TPageCoord.pcTopDown)

            ' Disable color key masking for images
            pdf.SetUseTransparency(False)

            ' Create three layers
            oc1 = pdf.CreateOCG("All", True, True, TOCGIntent.oiAll)
            oc2 = pdf.CreateOCG("Text and Annotations", True, True, TOCGIntent.oiAll)
            oc3 = pdf.CreateOCG("Images", True, True, TOCGIntent.oiAll)

            pdf.Append()
               ' The main layer controls the visibility of all three layers in this example.
               pdf.BeginLayer(oc1)
                  pdf.BeginLayer(oc2)
                     pdf.SetFont("Helvetica", TFStyle.fsRegular, 12.0, False, TCodepage.cp1252)
                     Dim someText As String = "Some text with a link!!!"
                     pdf.SetFillColor(CPDF.PDF_BLUE)
                     pdf.WriteText(50.0, 50.0, someText)
                     Dim tw As Double = pdf.GetTextWidth(someText)
                     ' To reflect the same nesting as the text layer we must use an OCMD for the annotation
                     ' because the visibility of the layer oc2 depends on oc1 at this position.
                     pdf.SetBorderStyle(TBorderStyle.bsUnderline)
                     pdf.SetStrokeColor(CPDF.PDF_BLUE)
                     annot = pdf.WebLink(50.0, 51.0, tw, 12.0, "www.dynaforms.com")

                     Dim array() As Integer = {oc1, oc2}
                     ocmd = pdf.CreateOCMD(TOCVisibility.ovAllOn, array)
                     pdf.AddObjectToLayer(ocmd, TOCObject.ooAnnotation, annot)
                  pdf.EndLayer()

                  pdf.BeginLayer(oc3)
                     pdf.InsertImageEx(50.0, 70.0, 300.0, 200.0, "../../../../test_files/images/margarita-102572_640.jpg", 1)
                  pdf.EndLayer()
               pdf.EndLayer()

               pdf.SetFillColor(CPDF.PDF_BLACK)
               pdf.WriteText(50.0, 300.0, "This text is not part of a layer!")
            pdf.EndPage()

            pdf.SetPageMode(TPageMode.pmUseOC)
         ' No fatal error occurred?
         If pdf.HaveOpenDoc() Then
            ' We write the output file into the current directory.
            Dim filePath As String = System.IO.Directory.GetCurrentDirectory() + "\out.pdf"
            If pdf.OpenOutputFile(filePath) Then
               If pdf.CloseFile() Then
                  Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
                  p.StartInfo.FileName = filePath
                  p.Start()
               End If
            End If
         End If
      Catch e As Exception
         Console.Write(e.Message + Chr(10))
         Console.Read()
      End Try
   End Sub

End Module
