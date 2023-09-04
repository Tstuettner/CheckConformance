Option Strict On
Option Explicit On 
Imports System.Runtime.InteropServices
Imports System.Text
Imports text_extraction.DynaPDF

Friend Class CTextExtraction

   Private Const MAX_LINE_ERROR As Double = 4.0 ' This must be the square of the allowed error (2 * 2 in this case).

   Public Sub New(ByVal PDFInst As CPDF)
      m_LastTextDir = TTextDir.tfNotInitialized
      m_PDF = PDFInst
      m_Templates = New System.Collections.Generic.List(Of Integer)
   End Sub

   Protected Function CalcDistance(ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double) As Double
      Dim dx As Double = x2 - x1
      Dim dy As Double = y2 - y1
      Return Math.Sqrt(dx * dx + dy * dy)
   End Function

   Private Function AddText() As Integer
      Try
         Dim i As Integer
         Dim textDir As TTextDir
         Dim x1 As Double = 0.0
         Dim y1 As Double = 0.0
         Dim x2 As Double = 0.0
         Dim y2 As Double = m_Stack.FontSize
         ' Transform the text matrix to user space
         Dim m As TCTM = MulMatrix(m_Stack.ctm, m_Stack.tm)
         Transform(m, x1, y1) ' Start point of the text record
         'The second point to determine the text direction can also be used to
         'calculate the visible font size measured in user space:
         '  Dim realFontSize as Double = CalcDistance(x1, y1, x2, y2)
         Transform(m, x2, y2) ' Second point to determine the text direction
         ' Determine the text direction
         If y1 = y2 Then
            textDir = CType((System.Convert.ToInt32(x1 > x2) + 1) * 2, TTextDir)
         Else
            textDir = CType(System.Convert.ToInt32(y1 > y2), TTextDir)
         End If

         If textDir <> m_LastTextDir OrElse Not IsPointOnLine(x1, y1, m_LastTextEndX, m_LastTextEndY, m_LastTextInfX, m_LastTextInfY) Then
            ' Extend the x-coordinate to an infinite point
            m_LastTextInfX = 1000000.0
            m_LastTextInfY = 0.0
            Transform(m, m_LastTextInfX, m_LastTextInfY)
            If m_LastTextDir <> TTextDir.tfNotInitialized Then
               ' Add a new line to the output file
               m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(Chr(13) + Chr(10)))
            End If
         Else
            'The space width is measured in text space but the distance between two text
            'records is measured in user space! We must transform the space width to user
            'space before we can compare the values.
            Dim distance As Double, spaceWidth As Double
            ' Note that we use the full space width here because the end position of the last record
            ' was set to the record width minus the half space width.
            Dim x3 As Double = m_Stack.SpaceWidth
            Dim y3 As Double = 0.0
            Transform(m, x3, y3)
            spaceWidth = CalcDistance(x1, y1, x3, y3)
            distance = CalcDistance(m_LastTextEndX, m_LastTextEndY, x1, y1)
            If distance > spaceWidth Then
               'Add a space to the output file
               m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(" "))
            End If
         End If
         ' We use the half space width to determine whether a space must be inserted at
         ' a specific position. This produces better results in most cases.
         Dim ptr As Long = m_Stack.Kerning.ToInt64
         Dim spw As Single = -m_Stack.SpaceWidth * 0.5F
         Dim rec As TTextRecordW
         If m_Stack.FontSize < 0.0 Then spw = -spw
         For i = 0 To m_Stack.KerningCount - 1
            CPDF.CopyKernRecord(New IntPtr(ptr), rec, Marshal.SizeOf(rec))
            If rec.Advance < spw Then
               ' Add a space to the output file
               m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(" "))
            End If
            ptr += Marshal.SizeOf(rec)
            m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(Marshal.PtrToStringUni(rec.Text, rec.Length)))
         Next i
         ' We don't set the cursor to the real end of the string because applications like MS Word
         ' add often a space to the end of a text record and this space can slightly overlap the next
         ' record. IsPointOnLine() would return false in this case.
         m_LastTextEndX = m_Stack.TextWidth + spw ' spw is a negative value!
         m_LastTextEndY = 0.0
         m_LastTextDir = textDir
         ' Calculate the end coordinate of the text record
         Transform(m, m_LastTextEndX, m_LastTextEndY)
         Return 0
      Catch
         Return -1
      End Try
   End Function

   Public Sub Close()
      m_File.Flush()
      m_File.Close()
      m_File = Nothing
      m_Stream = Nothing
   End Sub

   Private Function IsPointOnLine(ByVal x As Double, ByVal y As Double, ByVal x0 As Double, ByVal y0 As Double, ByVal x1 As Double, ByVal y1 As Double) As Boolean
      Dim dx As Double, dy As Double, di As Double
      x -= x0
      y -= y0
      dx = x1 - x0
      dy = y1 - y0
      di = (x * dx + y * dy) / (dx * dx + dy * dy)
      If di < 0.0 Then
         di = 0.0
      ElseIf di > 1.0 Then
         di = 1.0
      End If
      dx = x - di * dx
      dy = y - di * dy
      di = dx * dx + dy * dy
      Return (di < MAX_LINE_ERROR)
   End Function

   Protected Function MulMatrix(ByVal M1 As TCTM, ByVal M2 As TCTM) As TCTM
      Dim retval As TCTM
      retval.a = M2.a * M1.a + M2.b * M1.c
      retval.b = M2.a * M1.b + M2.b * M1.d
      retval.c = M2.c * M1.a + M2.d * M1.c
      retval.d = M2.c * M1.b + M2.d * M1.d
      retval.x = M2.x * M1.a + M2.y * M1.c + M1.x
      retval.y = M2.x * M1.b + M2.y * M1.d + M1.y
      Return retval
   End Function

   Public Sub Open(ByVal FileName As String)
      m_Stream = New System.IO.FileStream(FileName, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite)
      m_File = New System.IO.BinaryWriter(m_Stream, System.Text.Encoding.Unicode)
      ' Write a Little Endian marker to the file (byte order mark)
      m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(ChrW(&HFEFF)))
   End Sub

   Public Sub ParsePage()
      m_Templates.Clear()
      If Not m_PDF.InitStack(m_Stack) Then Return
      m_LastTextEndX = 0.0
      m_LastTextEndY = 0.0
      m_LastTextDir = TTextDir.tfNotInitialized
      m_LastTextInfX = 0.0
      m_LastTextInfY = 0.0

      ParseText()
      ParseTemplates()
  End Sub

   ' Templates are parsed recursively.
   Private Sub ParseTemplates()
      Dim i As Integer, j As Integer, tmpl As Integer
      For i = 0 To m_PDF.GetTemplCount() - 1
         If Not m_PDF.EditTemplate(i) Then Return
         tmpl = m_PDF.GetTemplHandle()
         If m_Templates.IndexOf(tmpl) < 0 Then
            m_Templates.Add(tmpl)
            If Not m_PDF.InitStack(m_Stack) Then Return

            ParseText()

            For j = 0 To m_PDF.GetTemplCount() - 1
               ParseTemplates()
            Next j
            m_PDF.EndTemplate()
         Else
            m_PDF.EndTemplate()
         End If
      Next i
   End Sub

   Private Sub ParseText()
      Dim haveMore As Boolean
      ' Get the first text record if any
      haveMore = m_PDF.GetPageText(m_Stack)
      ' No text found?
      If Not haveMore And (m_Stack.TextLen = 0) Then Return
      AddText()
      If haveMore Then
         Do While m_PDF.GetPageText(m_Stack)
            AddText()
         Loop
      End If
   End Sub

   Protected Sub Transform(ByRef M As TCTM, ByRef x As Double, ByRef y As Double)
      Dim tx As Double
      tx = x
      x = tx * M.a + y * M.c + M.x
      y = tx * M.b + y * M.d + M.y
   End Sub

   Public Sub WritePageIdentifier(ByVal PageNum As Integer)
      If PageNum > 1 Then
         m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(Chr(13) + Chr(10)))
      End If
      m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(String.Format("%----------------------- Page {0} -----------------------------" + Chr(13) + Chr(10), PageNum)))
   End Sub

   Protected Enum TTextDir
      tfLeftToRight = 0
      tfRightToLeft = 1
      tfTopToBottom = 2
      tfBottomToTop = 4
      tfNotInitialized = -1
   End Enum

   Protected m_File As System.IO.BinaryWriter
   Protected m_HavePos As Boolean
   Protected m_LastTextDir As TTextDir
   Protected m_LastTextEndX As Double
   Protected m_LastTextEndY As Double
   Protected m_LastTextInfX As Double
   Protected m_LastTextInfY As Double
   Protected m_PDF As CPDF
   Protected m_Stack As TPDFStack
   Protected m_Stream As System.IO.FileStream
   Protected m_Templates As System.Collections.Generic.List(Of Integer)
End Class
