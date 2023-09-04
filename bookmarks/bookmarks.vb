Imports bookmarks.DynaPDF

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
         Dim x As Double, y As Double
         Dim act As Integer, lnk As Integer, f As Integer, bmk As Integer, root As Integer
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' The ouput file is opened later

            pdf.SetPageCoords(TPageCoord.pcTopDown)

            pdf.SetPageHeight(500.0)
            pdf.SetPageWidth(800.0)

            pdf.Append()
               f = pdf.SetFont("Helvetica", TFStyle.fsRegular, 20, False, TCodepage.cp1252)
               pdf.WriteText(50, 50, "Bookmark destination type dtFit")
               root = pdf.AddBookmark("DestType dtFit", -1, 1, True)
               pdf.SetBookmarkDest(root, TDestType.dtFit, 0, 0, 0, 0)
               pdf.SetBookmarkStyle(root, TFStyle.fsItalic, CPDF.PDF_RED)
            pdf.EndPage()

            pdf.Append()
               pdf.ChangeFont(f)
               pdf.WriteText(50, 50, "Bookmark destination type dtXY_Zoom")
               pdf.WriteText(50, 70, "Zoom factor 3, Top position 50 (TopDown coordinates)")
               bmk = pdf.AddBookmark("DestType: dtXY_Zoom, zoom factor 3", root, 2, False)
               pdf.SetBookmarkDest(bmk, TDestType.dtXY_Zoom, 50, 50, 3, 0)
               pdf.SetBookmarkStyle(bmk, TFStyle.fsBold, CPDF.PDF_MAROON)
            pdf.EndPage()

            pdf.Append()
               pdf.ChangeFont(f)
               pdf.WriteText(50, 50, "Bookmark destination type dtXY_Zoom")
               pdf.WriteText(50, 70, "Zoom factor 0.5, Top position 50 (TopDown coordinates)")
               bmk = pdf.AddBookmark("DestType: dtXY_Zoom, zoom factor 0.5", root, 3, False)
               pdf.SetBookmarkDest(bmk, TDestType.dtXY_Zoom, 50, 50, 0.5, 0)
               pdf.SetBookmarkStyle(bmk, TFStyle.fsBold Or TFStyle.fsItalic, CPDF.PDF_GREEN)
            pdf.EndPage()

            pdf.Append()
               pdf.ChangeFont(f)
               pdf.WriteText(50, 50, "Bookmark destination type dtXY_Zoom")
               pdf.WriteText(50, 70, "Zoom factor not defined (unchanged), Top position 50 (TopDown coordinates)")
               bmk = pdf.AddBookmark("DestType: dtXY_Zoom, zoom factor unchanged", root, 4, False)
               pdf.SetBookmarkDest(bmk, TDestType.dtXY_Zoom, 50, 50, 0, 0)
               pdf.SetBookmarkStyle(bmk, TFStyle.fsRegular, CPDF.PDF_BLUE)
            pdf.EndPage()

            pdf.Append()
               pdf.ChangeFont(f)
               pdf.WriteText(50, 50, "Bookmark destination type dtFitH_Top")
               pdf.WriteText(50, 70, "Top position 50 (TopDown coordinates)")
               bmk = pdf.AddBookmark("DestType: dtFitH_Top (50)", root, 5, False)
               pdf.SetBookmarkDest(bmk, TDestType.dtFitH_Top, 50, 0, 0, 0)
               pdf.SetBookmarkStyle(bmk, TFStyle.fsRegular, &HFF8080)
               pdf.WriteText(50, 200, "Bookmark destination type dtFitH_Top")
               pdf.WriteText(50, 220, "Top position 200 (TopDown coordinates)")
               bmk = pdf.AddBookmark("DestType dtFitH_Top (200)", root, 5, False)
               pdf.SetBookmarkDest(bmk, TDestType.dtFitH_Top, 200, 0, 0, 0)
               pdf.SetBookmarkStyle(bmk, TFStyle.fsRegular, &HC08080)
            pdf.EndPage()

            pdf.Append()
               pdf.ChangeFont(f)
               pdf.WriteText(200, 50, "Bookmark destination type dtFitV_Left")
               pdf.WriteText(200, 70, "Left position 200. FitV has no effect if the width of the page")
               pdf.WriteText(200, 90, "is not greater as the height.")
               bmk = pdf.AddBookmark("DestType: dtFitV_Left (200)", root, 6, False)
               pdf.SetBookmarkDest(bmk, TDestType.dtFitV_Left, 200, 0, 0, 0)
               pdf.SetBookmarkStyle(bmk, TFStyle.fsRegular, &H808FFF)
            pdf.EndPage()

            pdf.Append()
               pdf.ChangeFont(f)
               pdf.WriteText(50, 50, "Bookmark destination type dtFit_Rect")
               x = (pdf.GetPageWidth() - 90.0) / 2.0
               y = (pdf.GetPageHeight() - 65.0) / 2.0

               pdf.WriteFTextEx(x, y, 90.0, -1, TTextAlign.taCenter, "We zoom into the rectangle")
               pdf.Rectangle(x, y, 90.0, 65.0, TPathFillMode.fmStroke)

               ' We place a page link with a GoTo action on this position. The link zooms into the rectangle in the same way as the bookmark.
               pdf.SetLinkHighlightMode(THighlightMode.hmInvert)
               lnk = pdf.PageLink(x, y, 90, 65, 7)
               act = pdf.CreateGoToAction(TDestType.dtFit_Rect, 7, x - 5.0, y - 5.0, x + 100.0, y + 70.0)
               pdf.AddActionToObj(TObjType.otPageLink, TObjEvent.oeOnMouseUp, act, lnk)

               bmk = pdf.AddBookmark("DestType: dtFit_Rect", -1, 7, False)
               ' The page link uses the same destination as the bookmark should use. So we add the action to the bookmark instead
               ' of a bookmark destination. This saves just a little bit disk space.
               pdf.AddActionToObj(TObjType.otBookmark, TObjEvent.oeOnMouseUp, act, bmk)
               pdf.SetBookmarkStyle(bmk, TFStyle.fsRegular, &H80FF)
            pdf.EndPage()

            pdf.SetPageFormat(TPageFormat.pfDIN_A4)
            pdf.Append()
               pdf.ChangeFont(f)
               pdf.WriteFTextEx(50.0, 50.0, pdf.GetPageWidth() - 100.0, -1.0, TTextAlign.taLeft, "Destination type dtFit. This variant scales the page so that both sides fit into the viewer window.")
            pdf.EndPage()

            root = pdf.AddBookmark("DestType dtFit", -1, 8, False)
            pdf.SetBookmarkDest(root, TDestType.dtFit, 0, 0, 0, 0)

            bmk = pdf.AddBookmark("DestType: dtXY_Zoom, zoom factor 3", root, 2, False)
            pdf.SetBookmarkDest(bmk, TDestType.dtXY_Zoom, 50, 50, 3, 0)
            pdf.SetBookmarkStyle(bmk, TFStyle.fsBold, CPDF.PDF_MAROON)


            pdf.SetPageLayout(TPageLayout.plOneColumn)

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
