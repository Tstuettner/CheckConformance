Imports edit_page.DynaPDF

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
         Dim f As Integer, orientation As Integer
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' The ouput file is opened later

            pdf.SetPageCoords(TPageCoord.pcTopDown)

            ' Import anything and don't convert pages to templates
            pdf.SetImportFlags(TImportFlags.ifImportAll Or TImportFlags.ifImportAsPage)
            If pdf.OpenImportFile("../../../test_files/rotated_270.pdf", TPwdType.ptOpen, Nothing) < 0 Then
               pdf = Nothing
               Return
            End If
            pdf.ImportPDFFile(1, 1.0, 1.0)
            pdf.CloseImportFile()

            pdf.SetPageCoords(TPageCoord.pcTopDown)

            ' This property moves the coordinate origin into the visible area (default value == false).
            pdf.SetUseVisibleCoords(True)

            ' Open page 1 for editing
            pdf.EditPage(1)
               ' Check whether the page is rotated.
               orientation = pdf.GetOrientation()
               If orientation <> 0 Then
                  ' SetOrientationEx() rotates the coordinate system into the opposite direction of the page orientation.
                  ' There is no guarantee that the contents in a page is rotated in the same way. If the result is wrong
                  ' then don"t call this function.
                  pdf.SetOrientationEx(orientation)
               End If
               f = pdf.SetFont("Helvetica", TFStyle.fsRegular, 12.0, False, TCodepage.cp1252)
               ' We use this font also as list font. In this example we use a bullet as list symbol (character index 143 of the code page 1252).
               pdf.SetListFont(f)
               pdf.WriteFTextEx(50.0, 200.0, pdf.GetPageWidth() - 100.0, -1.0, TTextAlign.taJustify, "It is not difficult to edit an imported " & _
                  "page but two things must be considered:" + Chr(13) + Chr(13) + "\LI[20,143]\LD[16]The page's " & _
                  "orientation.\EL#\LI[20,143]\LD[12]The coordinate origin. The coordinate origin can be taken from the crop box if present, or from the media box (Left and Bottom).\EL#" + Chr(13) + "\LD[12]" & _
                  "Although it is possible to correct the coordinate origin manually, it is much easier to set the property SetUseVisibleCoords() to true. DynaPDF moves the zero point then automatically " & _
                  "into the visible area of the page." + Chr(13) + Chr(13) & _
                  "The functions GetPageWidth() and GetPageHeight() return then also the logical width or height of the page depending on the orientation and whether a crop box is present." + Chr(13) + Chr(13) & _
                  "The handling of rotated pages is a bit more complicated since the orientation is just a property. That means there is no guarantee that the contents is rotated " & _
                  "into the opposite direction like the contents in this page. Whether this is the case depends on the creator of the PDF file." + Chr(13) + Chr(13) & _
                  "However, by default it is probably best to assume that the contents is rotated. SetOrientationEx() rotates the coordinate system so that we can work with the page as if it was " & _
                  "not rotated. If this produces a wrong result then don't call SetOrientationEx()." + Chr(13) + Chr(13) & _
                  "Now you ask probably yourself whether it is possible to identify the orientation of the contents in a page. The answer is maybe. It is possible to parse a page with ParseContent() " & _
                  "and to inspect the transformation matrices but this can produce wrong results especially if a page contains not much contents.")
            pdf.EndPage()
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
