Imports image_extraction.DynaPDF

Module Module1

   'Private m_Counter As Integer
   Private m_Images As ArrayList
   Private m_HaveError As Boolean
   Private m_PDF As CPDF
   Private m_Templates As ArrayList

   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (m_PDF.SetErrorMode(TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.WriteLine("{0}", System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      m_HaveError = True
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Private Function parseBeginTemplate(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Handle As Integer, ByRef BBox As TPDFRect, ByVal Matrix As IntPtr) As Integer
      If (m_Templates.Contains(Handle)) Then Return 1 ' Already handled
      Try
         m_Templates.Add(Handle)
         Return 0
      Catch
         Return -1
      End Try
   End Function

   Private Function parseInsertImage(ByVal Data As IntPtr, ByRef Image As TPDFImage) As Integer
      If Image.InlineImage = 0 Then
         If m_Images.Contains(Image.ObjectPtr) Then Return 0 ' Already handled?
         Try
            m_Images.Add(Image.ObjectPtr)
         Catch
            Return -1
         End Try
      End If
      ' If an image cannot be decompressed due to an unsupported filter or if the decoder
      ' failed to decompress it then we can get a compressed image here.
      If Image.Filter <> TDecodeFilter.dfNone Then Return 0
      ' Note that Flate compression is no standard filter; Photoshop does not support this filter.
      If Image.BitsPerPixel = 1 Then
         m_PDF.AddImage(TCompressionFilter.cfCCITT4, TImageConversionFlags.icNone, Image)
      Else
         m_PDF.AddImage(TCompressionFilter.cfLZW, TImageConversionFlags.icNone, Image)
      End If
      Return 0
   End Function

   Sub Main()
      Try
         m_PDF = New CPDF()
         m_Images = New ArrayList(1024)    ' List of already handled images to enable a duplicate check
         m_Templates = New ArrayList(1024) ' List to store template handles
         ' You can either use events or declare a callback function.
         m_PDF.SetOnErrorProc(AddressOf PDFError)
         m_PDF.CreateNewPDF(Nothing) ' We do not create a PDF file in this example

         ' We avoid the conversion of pages to templates
         m_PDF.SetImportFlags(TImportFlags.ifContentOnly Or TImportFlags.ifImportAsPage)
         If m_PDF.OpenImportFile("../../../../../dynapdf_help.pdf", TPwdType.ptOpen, Nothing) < 0 Then
            Console.Write("Input file ""../../../../../dynapdf_help.pdf"" not found!" + Chr(10))
            Console.Read()
            Exit Sub
         End If
         m_PDF.ImportPDFFile(1, 1.0, 1.0)
         m_PDF.CloseImportFile()

         ' We flatten form fields so that we can extract images of these objects too.
         m_PDF.FlattenForm()

         Dim stack As TPDFParseInterface = New TPDFParseInterface
         stack.BeginTemplate = AddressOf parseBeginTemplate
         stack.InsertImage = AddressOf parseInsertImage

         ' We write the output file into the current directory.
         Dim outFile As String = System.IO.Directory.GetCurrentDirectory() + "\out.tif"

         ' We create a multi-page TIFF image in this example.
         If Not m_PDF.CreateImage(outFile, TImageFormat.ifmTIFF) Then Return

         Dim i As Integer
         For i = 1 To m_PDF.GetPageCount()
            m_PDF.EditPage(i)
            'If you want to convert the images into a specific color space then set
            'one of the folowing flags (see also TParseFlags in vb):
            '   pfDitherImagesToBW ' Floyd-Steinberg dithering.
            '   pfConvImagesToGray
            '   pfConvImagesToRGB
            '   pfConvImagesToCMYK
            'Only one color space conversion flag must be set at time.
            m_PDF.ParseContent(stack, TParseFlags.pfDecomprAllImages)
            m_PDF.EndPage()
         Next i
         m_PDF.CloseImage()

         If Not m_HaveError Then Console.Write("TIFF image successfully extracted to ""{0}""!" + Chr(10), outFile)
         Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
         p.StartInfo.FileName = outFile
         p.Start()
      Catch e As Exception
         Console.Write(e.Message + Chr(10))
      End Try
      Console.Read()
   End Sub

End Module
