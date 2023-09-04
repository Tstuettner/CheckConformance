Imports metafiles.DynaPDF

Module Module1
   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.Write("{0}" + Chr(10), System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Private Sub PlaceEMFCentered(ByVal PDF As CPDF, ByVal FileName As String, ByVal Width As Double, ByVal Height As Double)
      Dim r As TRectL
      Dim x, y, w, h, sx As Double
      ' We place the EMF file horizontal and vertically centered in this example.
      ' We need the picture size of the EMF file
      If Not PDF.GetLogMetafileSize(FileName, r) Then Exit Sub

      w = r.rRight - r.rLeft
      h = r.rBottom - r.rTop
      ' We place the EMF file into a border of 2 units around the page so that we can better see
      ' the bounding box of the EMF file.
      Width -= 4.0
      Height -= 4.0
      sx = Width / w
      ' An important note about the bounding rectangle: DynaPDF calculates the zero point
      ' of the EMF picture automatically so that we don't need to consider the coordinate origin.
      ' The coordinate origin for our calculation here is always 0.0, 0.0 independent of the real
      ' origin of the EMF picture.
      If h * sx <= Height Then
         h *= sx
         x = 2.0
         ' If the file should not be centered vertically then set y to 2.0.
         y = (Height - h) / 2.0
         PDF.InsertMetafile(FileName, x, y, Width, 0.0)
         ' The rectangle represents the real bounding box of the EMF picture.
         PDF.SetStrokeColor(CPDF.PDF_RED)
         PDF.Rectangle(x, y, Width, h, TPathFillMode.fmStroke)
      Else
         sx = Height / h
         w *= sx
         x = (Width - w) / 2.0
         y = 2.0
         PDF.InsertMetafile(FileName, x, y, 0.0, Height)
         ' The rectangle represents the real bounding box of the EMF picture.
         PDF.SetStrokeColor(CPDF.PDF_RED)
         PDF.Rectangle(x, y, w, Height, TPathFillMode.fmStroke)
      End If
   End Sub

   Sub Main()
      Try
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' The ouput file is opened later

         pdf.SetDocInfo(TDocumentInfo.diAuthor, "Jens Boschulte")
         pdf.SetDocInfo(TDocumentInfo.diCreator, "VB .Net test application")
         pdf.SetDocInfo(TDocumentInfo.diTitle, "Metafiles")
         pdf.SetViewerPreferences(TViewerPreference.vpDisplayDocTitle, TViewPrefAddVal.avNone)

         pdf.SetPageCoords(TPageCoord.pcTopDown)

         pdf.Append()

         'We use a landscape paper format in this example. SetOrientationEx() rotates the coordinate
         'system according to the orientation and sets the page orientation. You can now work with page
         'as if it was not rotated. The real page format is still DIN A4 (this is the default format).
         'The difference to SetOrientation() is that this function does not change the page's coordinate
         'system.

         'It would also be possible to use a user defined paper format without changing the page
         'orientation but the disadvantage is that a printer driver must then manually rotate the page
         'because landscape paper formats do not exist in most printers. This step requires an
         'additional interaction with the user which is simply not required when creating landscape
         'paper formats in the right way.
         pdf.SetOrientationEx(90)

         ' This file transforms the coordinate system very often and uses clipping regions. The metafile
         ' is scaled to the page width without changing its aspect ratio.
         PlaceEMFCentered(pdf, "../../../test_files/coords.emf", pdf.GetPageWidth(), pdf.GetPageHeight())
         pdf.EndPage()

         pdf.Append()
         pdf.SetOrientationEx(90)
         ' Simple test of line and standard patterns
         PlaceEMFCentered(pdf, "../../../test_files/fulltest.emf", pdf.GetPageWidth(), pdf.GetPageHeight())
         pdf.EndPage()

         pdf.Append()
         pdf.SetOrientationEx(90)
         ' Outlined text, dash patterns with text. This file requires the font Bookman Old Style. If not available,
         ' the result will be wrong!
         PlaceEMFCentered(pdf, "../../../test_files/gdi.emf", pdf.GetPageWidth(), pdf.GetPageHeight())
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
         Console.Read()
      End Try
   End Sub

End Module
