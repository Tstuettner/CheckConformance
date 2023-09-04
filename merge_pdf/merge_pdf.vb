Imports merge_pdf.DynaPDF

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
         Dim i As Integer
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' The ouput file is opened later

         pdf.SetPageCoords(TPageCoord.pcTopDown)

         pdf.Append()
            pdf.SetFont("Helvetica", TFStyle.fsRegular, 14.0, False, TCodepage.cp1252)
            pdf.WriteFTextEx(50.0, 50.0, pdf.GetPageWidth() - 100.0, -1.0, TTextAlign.taJustify, "The following pages were imported from " & _
               "different PDF files. DynaPDF adjusts the destinations of link annotations and bookmarks so that " & _
               "all destinations refer to the new page numbers after import." + Chr(13) + Chr(13) & _
               "Entire PDF files can be easily merged with ImportPDFFile() but it is also possible to import only specific pages of an arbitrary number " & _
               "of PDF files. You can also add further pages or edit imported pages if necessary. An existing page can be opened for editing with EditPage().")
         pdf.EndPage()

         Dim first As Boolean = True
         Dim destPage As Integer = 1
         Dim haveXFA As Boolean = False
         Dim isCollection As Boolean = False

         Dim files() As String = {"../../../../license.pdf", "../../../../dynapdf_help.pdf"}

         ' Generic code to merge arbitrary PDF files.
         For i = 0 To 1
            If pdf.OpenImportFile(files(i), TPwdType.ptOpen, Nothing) < 0 Then
               pdf = Nothing
               Return
            End If
            ' Not all PDF files can be merged:
            '   - An Interactive Form is a global structure that cannot be simply merged. The names of all form fields must be
            '     be unique. But also if name collusions will be solved, there is no guarantee that embedded Javascripts or Javascript
            '     actions will work as expected. Interactive Forms should not be merged!

            '   - A PDF collection is a special PDF file that consists of a container PDF and an array of embedded files.
            '     It is possible to merge two or more PDF Collections but it is not meaningful to merge a PDF Collection
            '     with normal PDF files or vice versa.
            If first Then
               first = False
               haveXFA = pdf.GetInIsXFAForm()
               isCollection = pdf.GetInIsCollection()
               destPage = pdf.ImportPDFFile(destPage + 1, 1.0, 1.0)
               If destPage < 0 Then Exit For
            Else
               ' Special handling for PDF Collections
               If isCollection Then
                  If pdf.GetInIsCollection() Then
                     ' Import the embedded files only
                     pdf.SetImportFlags(TImportFlags.ifEmbeddedFiles)
                     ' We could also use ImportPDFFile() but this function is more efficient since no pages will be imported.
                     If Not pdf.ImportCatalogObjects() Then Exit For
                  Else
                     pdf.CloseImportFile()
                     ' Add the file to the collection
                     pdf.AttachFile(files(i), System.IO.Path.GetFileName(files(i)), True)
                  End If
               Else
                  If pdf.GetInIsCollection() OrElse ((pdf.GetInIsXFAForm() OrElse pdf.GetInFieldCount() > 0) AndAlso (pdf.GetFieldCount() > 0 OrElse haveXFA)) Then Exit For
                  pdf.SetImportFlags(TImportFlags.ifImportAll Or TImportFlags.ifImportAsPage) ' Import anything and avoid the conversion of pages to templates
                  pdf.SetImportFlags2(TImportFlags2.if2UseProxy)                              ' This flag reduces the memory usage.
                  destPage = pdf.ImportPDFFile(destPage + 1, 1.0, 1.0)
                  If destPage < 0 Then Exit For
               End If
            End If
            pdf.CloseImportFile()
         Next i

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
