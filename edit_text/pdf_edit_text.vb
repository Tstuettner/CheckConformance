Option Strict Off

Imports System.Runtime.InteropServices
Imports edit_text.DynaPDF

Friend Class CPDFEditText

   'This class demonstrates how texts can be found and replaced in a PDF file. The
   'algorithm handles rotated text as well as text lines which consist of multiple
   'text records correctly.

   'This algorithm assumes that the text is ordered from left to right and from top
   'to bottom. If this is not the case the result is maybe incorrect or the search
   'text cannot be found.

   'The remaing text in a line is realigned to avoid overlaping text or holes
   'between text redords. This realignment is not a real text re-flow because it is
   'restricted to the current text line. No line break occurs if the new text is too
   'large to fit into the page or text block. It is possible to develop a real text
   're-flow but such an algorithm is much more complex than this one.

   'A page must always be parsed twice, the first time to find the text records which
   'make up the search string, and a second time to delete or replace the text.

   '----------------------------------------------------------------------------------

   'A note about underlined text:
   'The underline of a text is not a font attribute. It is a separate vector graphic
   'which occurs outside of the text object, e.g. a rectanlge or a line drawn with a
   'moveto/lineto pair. DynaPDF contains no algorithms to identify such underlines so
   'that they can be deleted with the text. If you delete an underlined text the
   'underline will be left unchanged and only the text is deleted!

   'Calculation of the space width:

   'Space characters are often not stored in PDF files, instead, kerning space
   'is applied which produces the same visual appearance. Due to the missing space
   'characters we need an algorithm that is able to detect word boundaries.

   'The algorithm here compares just the distance between two text records and the
   'the displacement in kerning records. If it is larger than the half space width
   'we assume that a space character was emulated at this position. Note that the
   'displacement in a kerning record is measured in text space while the distance
   'between two text records is measured in user space!

   'The full space width is often too large to determine whether normal kerning
   'space was applied or whether a space character must be added at a given position.
   'The half space width procudes better results in most cases especially if the
   'document uses condensed fonts. The fonts of documents which emulate space
   'characters contain often no space character. In this case a standard space width
   'is set because the real value is not known.

   'Notice:
   'VB .Net is a rather slow programming language. The same code developed in C#
   'is about 10 times faster. If you want to work with the .Net framework it is
   'usually better to use C# instead. BTW - This class is delivered in C# too!

   Public Structure TTextRec
      Dim TmplHandle As Integer
      Dim First As Integer
      Dim Last As Integer
      Dim KernRecord As Integer
      Dim StrPos As Integer
      Dim NewLine As Boolean
   End Structure

   'Allowed error to determine whether a text record must be assigned to the current line.
   Protected Const MAX_LINE_ERROR As Double = 2.0

   'List to store found text records
   Public Class CTextRecords
      Public Sub Add(ByVal TmplHandle As Integer, ByVal First As Integer, ByVal Last As Integer, ByVal KernRecord As Integer, ByVal StrPos As Integer, ByVal NewLine As Boolean)
         If m_Count + 1 > m_Capacity Then
            ReDim m_Items(m_Capacity + 4196)
            m_Capacity += 4196
         End If
         m_Items(m_Count).TmplHandle = TmplHandle
         m_Items(m_Count).First = First
         m_Items(m_Count).Last = Last
         m_Items(m_Count).KernRecord = KernRecord
         m_Items(m_Count).StrPos = StrPos
         m_Items(m_Count).NewLine = NewLine
         m_Count += 1
      End Sub

      Public Sub Clear()
         m_Count = 0
      End Sub

      Public Function Count() As Integer
         Return m_Count
      End Function

      Public Function GetItem(ByVal Index As Integer) As TTextRec
         Return m_Items(Index)
      End Function

      Protected m_Capacity As Integer
      Protected m_Count As Integer
      Protected m_Items As TTextRec()
   End Class
   'List to store template handles
   Public Class CIntList
      Public Sub Add(ByVal Value As Integer)
         If m_Count + 1 > m_Capacity Then
            ReDim m_Items(m_Capacity + 1024)
            m_Capacity += 1024
         End If
         m_Items(m_Count) = Value
         m_Count += 1
      End Sub

      Public Sub Clear()
         m_Count = 0
      End Sub

      Public Function Count() As Integer
         Return m_Count
      End Function

      Public Function GetItem(ByVal Index As Integer) As Integer
         Return m_Items(Index)
      End Function

      Protected m_Capacity As Integer
      Protected m_Count As Integer
      Protected m_Items As Integer()
   End Class

   Public Sub New(ByVal PDFInst As CPDF)
      m_Matrix4.a = 1.0
      m_Matrix4.d = 1.0
      m_PDFInst = PDFInst
      m_Records = New CTextRecords()
      m_Templates = New CIntList()
   End Sub

   Protected Sub AddKernSpace(ByVal Advance As Single, ByVal SpaceWidth As Single, ByRef Matrix As TCTM)
      If Advance < SpaceWidth Then
         ' Hey, is an operator like AndAlso really required 
         If m_SearchPos < m_SearchText.Length AndAlso m_SearchText.Chars(m_SearchPos) = Chr(32) Then
            m_SearchPos += 1
            If m_SearchPos = m_SearchText.Length Then m_SearchPos = 0
         Else
            m_Matrix4.x = -Advance
            Matrix = MulMatrix(Matrix, m_Matrix4)
         End If
      Else
         m_Matrix4.x = -Advance
         Matrix = MulMatrix(Matrix, m_Matrix4)
      End If
   End Sub

   Protected Sub AddKernSpaceEx(ByVal Advance As Single, ByVal SpaceWidth As Single, ByRef Matrix As TCTM)
      If Advance < SpaceWidth Then
         If m_SearchPos < m_SearchText.Length AndAlso m_SearchText.Chars(m_SearchPos) = Chr(32) Then
            m_SearchPos += 1
         Else
            m_Matrix4.x = -Advance
            Matrix = MulMatrix(Matrix, m_Matrix4)
         End If
      Else
         m_Matrix4.x = -Advance
         Matrix = MulMatrix(Matrix, m_Matrix4)
      End If
   End Sub

   Protected Sub AddRecord(ByVal KernRecord As Integer, ByVal StrPos As Integer)
      m_Records.Add(m_CurrTmpl, m_First, m_RecordNumber, KernRecord, StrPos, m_NewLine)
      m_KernRecord = -1
      m_NewLine = False
      m_SearchPos = 0
      m_StrPos = -1
   End Sub

   Protected Sub AddSpace(ByVal x As Double, ByVal y As Double, ByVal SpaceWidth As Double)
      Dim distX As Double
      If m_Alpha = 0 Then
         distX = x - m_LastX
      Else
         distX = CalcDistance(m_LastX, m_LastY, x, y)
      End If
      If distX > SpaceWidth Then
         If m_SearchPos < m_SearchText.Length AndAlso m_SearchText.Chars(m_SearchPos) = Chr(32) Then
            m_SearchPos += 1
         End If
      End If
   End Sub

   Protected Sub AddSpace(ByVal DistX As Double, ByVal SpaceWidth As Double, ByRef Matrix As TCTM)
      If DistX > SpaceWidth Then
         If m_SearchPos < m_SearchText.Length AndAlso m_SearchText.Chars(m_SearchPos) = Chr(32) Then
            m_SearchPos += 1
         Else
            m_Matrix4.x = DistX
            Matrix = MulMatrix(Matrix, m_Matrix4)
         End If
      Else
         m_Matrix4.x = DistX
         Matrix = MulMatrix(Matrix, m_Matrix4)
      End If
   End Sub

   Protected Function BildFamilyNameAndStyle(ByVal FontName As String, ByRef FamilyName As String, ByVal Style As TFStyle) As Boolean
      Dim first As Integer = 0, p As Integer
      If FontName.Length > 7 AndAlso FontName.Chars(6) = "+" Then
         first = 7
      End If
      p = FontName.IndexOf(",", first)
      If p > -1 Then
         p += 1
         Dim fstyle As String = FontName.Substring(p)
         If fstyle.IndexOf("BoldItalic") > -1 Then
            Style = Style Or TFStyle.fsBold Or TFStyle.fsItalic
         ElseIf fstyle.IndexOf("Italic") > -1 Then
            Style = Style Or TFStyle.fsItalic
         ElseIf fstyle.IndexOf("Bold") > -1 Then
            Style = Style Or TFStyle.fsBold
         End If
         p -= 1
         FamilyName = FontName.Substring(first, p)
         Return True
      Else
         FamilyName = FontName.Substring(first)
         If FamilyName.IndexOf(" ", 0) > -1 Then Return True
         Return False
      End If
   End Function

   Protected Sub CalcAlpha(ByRef M As TCTM)
      Dim x1 As Double = 0.0
      Dim y1 As Double = 0.0
      Dim x2 As Double = 1.0
      Dim y2 As Double = 0.0
      Transform(M, x1, y1)
      Transform(M, x2, y2)
      'Get rid of rounding errors...
      m_Alpha = Convert.ToInt32(Math.Atan2(y2 - y1, x2 - x1) / 0.017453292519943295 * 256.0)
   End Sub

   Protected Sub CalcDistance(ByRef M1 As TCTM, ByRef M2 As TCTM, ByRef M3 As TCTM, ByRef DistX As Double, ByRef DistY As Double, ByVal x As Double, ByVal y As Double)
      If m_Alpha = 0 Then
         DistX = x - m_LastX
         DistY = Math.Abs(y - m_LastY)
      Else
         M3 = MulMatrix(M1, M2)
         'Notice: The distance is always a positive value. If the text is not ordered from left
         'to right the result can be incorrect!

         'To do:
         'Check whether the text occurs behind the previous one. If not, insert the text at the
         'beginning of the line. This is recommended if bidirectional text should be processed.
         DistX = CalcDistance(m_LastX, m_LastY, x, y)
         DistY = Math.Abs(M3.y)
      End If
   End Sub

   Protected Function CalcDistance(ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double) As Double
      Dim dx As Double = x2 - x1
      Dim dy As Double = y2 - y1
      Return Math.Sqrt(dx * dx + dy * dy)
   End Function

   Protected Sub CalcStrPos(ByRef M As TCTM, ByRef x As Double, ByRef y As Double)
      x = 0.0
      y = 0.0
      M = MulMatrix(m_Stack.ctm, m_Stack.tm)
      Transform(M, x, y)
   End Sub

   Protected Sub FindAll()
      Dim m As TCTM
      Dim alpha As Integer
      Dim x As Double, y As Double, spaceWidth As Double, distX As Double, distY As Double
      m_LastX = 0.0
      m_LastY = 0.0
      m_RecordNumber = 0
      m_NewLine = True
      m_SearchPos = 0
      'Get the first text record if any
      m_HaveMore = m_PDFInst.GetPageText(m_Stack)
      'No text found?
      If m_Stack.TextLen = 0 Then Exit Sub

      'Calculate the text position and orientation angle
      CalcStrPos(m_Matrix1, x, y)

      FindPattern()

      spaceWidth = m_Stack.SpaceWidth * GetScaleFactor(m_Matrix1) * 0.5

      'Calculate the end offset and invert the matrix.
      m_LastX = m_Stack.TextWidth
      Transform(m_Matrix1, m_LastX, m_LastY)
      Invert(m_Matrix1)

      m_RecordNumber += 1

      If Not m_HaveMore Then Exit Sub

      alpha = m_Alpha
      While m_PDFInst.GetPageText(m_Stack)
         CalcStrPos(m_Matrix2, x, y)
         If alpha = m_Alpha Then
            CalcDistance(m_Matrix1, m_Matrix2, m, distX, distY, x, y)
            If (distY > MAX_LINE_ERROR) Then
               'This algorithm does not support line breaks!
               m_SearchPos = 0
               m_Matrix1 = m_Matrix2
               Invert(m_Matrix1)
               m_NewLine = True
            ElseIf distX > spaceWidth Then
               If distX > 6.0 * spaceWidth Then
                  'The distance is too large. We assume that the text should not be considered
                  'as part of the current text line.
                  m_SearchPos = 0
                  m_Matrix1 = m_Matrix2
                  Invert(m_Matrix1)
                  m_NewLine = True
               ElseIf m_SearchText.Chars(m_SearchPos) = Chr(32) Then
                  If m_SearchPos = 0 Then
                     m_First = m_RecordNumber
                     m_KernRecord = -1
                     m_StrPos = -1
                  End If
                  m_SearchPos += 1
                  If m_SearchPos = m_SearchText.Length Then
                     AddRecord(m_KernRecord, m_StrPos)
                  End If
               Else
                  m_SearchPos = 0
                  If m_SearchText.Chars(m_SearchPos) = Chr(32) Then
                     m_First = m_RecordNumber
                     m_SearchPos += 1
                     If m_SearchPos = m_SearchText.Length Then
                        AddRecord(-1, -1)
                     End If
                  End If
               End If
            ElseIf distX < -spaceWidth Then ' Wrong direction?
               m_SearchPos = 0
               m_Matrix1 = m_Matrix2
               Invert(m_Matrix1)
               m_NewLine = True
            End If
         Else
            m_SearchPos = 0
            m_Matrix1 = m_Matrix2
            Invert(m_Matrix1)
            m_NewLine = True
         End If
         FindPattern()
         m_LastY = 0.0
         m_LastX = m_Stack.TextWidth
         alpha = m_Alpha
         spaceWidth = m_Stack.SpaceWidth * GetScaleFactor(m_Matrix2) * 0.5
         Transform(m_Matrix2, m_LastX, m_LastY)
         m_RecordNumber += 1
      End While
   End Sub

   Protected Function FindEndPattern(ByVal Text As String, ByVal Start As Integer) As Integer
      Dim len As Integer = Text.Length - Start
      While len > 0
         len -= 1
         If m_SearchPos = m_SearchText.Length Then Exit While
         If Text.Chars(Start) <> m_SearchText.Chars(m_SearchPos) Then Exit While
         m_SearchPos += 1
         Start += 1
      End While
      Return Start
   End Function

   Protected Sub FindPattern()
      Dim i As Integer, j As Integer
      Dim c1 As Char, c2 As Char
      Dim ptr As Long
      Dim text As String
      Dim rec As TTextRecordW
      Dim spw As Single = -m_Stack.SpaceWidth / 2.0F
      If m_SearchPos = 0 Then
         m_First = m_RecordNumber
         m_KernRecord = -1
         m_StrPos = -1
      End If
      'Handle spaces
      ptr = m_Stack.Kerning.ToInt64()
      For i = 0 To m_Stack.KerningCount - 1
         '.Net languages are too stupid to handle arrays! We must copy each record manually.
         CPDF.CopyKernRecord(New IntPtr(ptr), rec, Marshal.SizeOf(rec))
         text = Marshal.PtrToStringUni(rec.Text, rec.Length)
         ptr += Marshal.SizeOf(rec)
         If rec.Advance < spw Then
            If m_SearchText.Chars(m_SearchPos) = Chr(32) Then
               If m_SearchPos = 0 AndAlso m_KernRecord < 0 Then
                  m_KernRecord = i
                  m_StrPos = -1
               End If
               m_SearchPos += 1
               If m_SearchPos = m_SearchText.Length Then AddRecord(m_KernRecord, m_StrPos)
            Else
               m_SearchPos = 0
               m_StrPos = -1
               If m_SearchText.Chars(m_SearchPos) = Chr(32) Then
                  m_SearchPos += 1
                  If m_KernRecord < 0 Then m_KernRecord = i
                  If m_SearchPos = m_SearchText.Length Then AddRecord(m_KernRecord, m_StrPos)
               End If
            End If
         End If
         'This is a case-sensitive search.
         For j = 0 To text.Length - 1
            c1 = text.Chars(j)
            c2 = m_SearchText.Chars(m_SearchPos)
            If c1 <> c2 Then
               If c1 = Chr(160) Then  'non-break space
                  If c2 = Chr(32) Then
                     If m_SearchPos = 0 AndAlso m_KernRecord < 0 Then
                        m_KernRecord = i
                        m_StrPos = j
                     End If
                     m_SearchPos += 1
                  End If
               ElseIf c1 = Chr(173) Then 'non-break hyphen
                  If c2 = Chr(45) Then
                     If m_SearchPos = 0 AndAlso m_KernRecord < 0 Then
                        m_KernRecord = i
                        m_StrPos = j
                     End If
                     m_SearchPos += 1
                  End If
               End If
               m_KernRecord = -1
               m_SearchPos = 0
               m_StrPos = -1
            Else
               If m_SearchPos = 0 AndAlso m_KernRecord < 0 Then
                  m_KernRecord = i
                  m_StrPos = j
               End If
               m_SearchPos += 1
            End If
            If m_SearchPos = m_SearchText.Length Then AddRecord(m_KernRecord, m_StrPos)
         Next j
      Next i
   End Sub

   Public Function FindPattern(ByVal Text As String) As Integer
      If Text.Length = 0 Then Throw New Exception("The search text cannot be an empty string!")
      m_SearchText = Text
      m_Records.Clear()
      If Not m_PDFInst.InitStack(m_Stack) Then Throw New Exception("-1")
      m_CurrTmpl = -1
      FindAll()
      ParseTemplates()
      Return m_Records.Count()
   End Function

   Protected Function GetScaleFactor(ByRef M As TCTM) As Double
      Dim x As Double = 0.70710678118654757 * M.a + 0.70710678118654757 * M.c
      Dim y As Double = 0.70710678118654757 * M.b + 0.70710678118654757 * M.d
      Return Math.Sqrt(x * x + y * y)
   End Function

   Protected Sub Invert(ByRef M As TCTM)
      Dim d As Double = 1.0 / (M.a * M.d - M.b * M.c)
      Dim a As Double = M.d * d
      M.d = M.a * d
      M.b = -M.b * d
      M.c = -M.c * d
      d = -M.x * a - M.y * M.c
      M.y = -M.x * M.b - M.y * M.d
      M.a = a
      M.x = d
   End Sub

   Protected Function MulMatrix(ByRef M1 As TCTM, ByRef M2 As TCTM) As TCTM
      Dim retval As TCTM
      retval.a = M2.a * M1.a + M2.b * M1.c
      retval.b = M2.a * M1.b + M2.b * M1.d
      retval.c = M2.c * M1.a + M2.d * M1.c
      retval.d = M2.c * M1.b + M2.d * M1.d
      retval.x = M2.x * M1.a + M2.y * M1.c + M1.x
      retval.y = M2.x * M1.b + M2.y * M1.d + M1.y
      Return retval
   End Function

   Protected Sub ParseTemplates()
      Dim found As Boolean = False
      Dim e As Integer, i As Integer, j As Integer, tmplCount As Integer, tmplCount2 As Integer
      tmplCount = m_PDFInst.GetTemplCount()
      For i = 0 To tmplCount - 1
         If Not m_PDFInst.EditTemplate(i) Then Throw New Exception("-1")
         m_CurrTmpl = m_PDFInst.GetTemplHandle()
         'We must make sure that we don't parse a template twice
         j = 0
         e = m_Templates.Count() - 1
         While j <= e
            If m_Templates.GetItem(j) = m_CurrTmpl Then
               found = True
               Exit While
            End If
            If m_Templates.GetItem(e) = m_CurrTmpl Then
               found = True
               Exit While
            End If
            j += 1
            e -= 1
         End While
         If found Then
            m_PDFInst.EndTemplate()
         Else
            'Add the template handle to the list of templates
            m_Templates.Add(m_CurrTmpl)

            If Not m_PDFInst.InitStack(m_Stack) Then Throw New Exception("-1")

            FindAll()

            tmplCount2 = m_PDFInst.GetTemplCount()
            For j = 0 To tmplCount2 - 1
               ParseTemplates()
            Next j
            m_PDFInst.EndTemplate()
         End If
      Next i
   End Sub

   Public Sub ReplacePattern(ByVal NewText As String)
      Dim i As Integer = 0, j As Integer, lastTmpl As Integer = -1
      Dim ptr As Long
      Dim rec As TTextRec
      Dim src As TTextRecordW
      If Not m_PDFInst.InitStack(m_Stack) Then Throw New Exception("-1")
      m_Matrix3.a = 1.0
      m_Matrix3.b = 0.0
      m_Matrix3.c = 0.0
      m_Matrix3.d = 1.0
      m_Matrix3.x = 0.0
      m_Matrix3.y = 0.0
      m_RecordNumber = 0
      m_LastFont = IntPtr.Zero
      While i < m_Records.Count()
         rec = m_Records.GetItem(i)
         If rec.TmplHandle <> lastTmpl Then
            If Not m_PDFInst.FlushPageContent(m_Stack) Then Throw New Exception("-1")
            If lastTmpl > -1 Then m_PDFInst.EndTemplate()
            If Not m_PDFInst.EditTemplate2(rec.TmplHandle) Then Throw New Exception("-1")
            If Not m_PDFInst.InitStack(m_Stack) Then Throw New Exception("-1")
            m_LastFont = IntPtr.Zero
            m_RecordNumber = 0
         End If
         While m_RecordNumber < rec.First
            m_PDFInst.GetPageText(m_Stack)
            m_RecordNumber += 1
         End While
         If rec.NewLine Then
            m_Matrix4.x = m_Stack.TextWidth
            m_Matrix1 = MulMatrix(m_Stack.tm, m_Matrix4)
            m_Matrix3 = MulMatrix(m_Stack.ctm, m_Matrix1)
         End If
         m_HaveMore = m_PDFInst.GetPageText(m_Stack)
         If rec.KernRecord > 0 Then
            'Delete the string but preserve the kerning records before the string occurred.
            m_Stack.DeleteKerningAt = rec.KernRecord
            m_PDFInst.ReplacePageTextA(Nothing, m_Stack)
            m_Matrix4.x = 0.0
            ptr = m_Stack.Kerning.ToInt64()
            For j = 0 To rec.KernRecord - 1
               CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
               m_Matrix4.x -= src.Advance
               m_Matrix4.x += src.Width
               ptr += Marshal.SizeOf(src)
            Next j
            CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
            'If StrPos == -1 the first character is a space character which is emulated with kerning space.
            If rec.StrPos > -1 Then m_Matrix4.x -= src.Advance
            'Compute the string position
            m_Matrix1 = MulMatrix(m_Stack.tm, m_Matrix4)
            m_Matrix2 = MulMatrix(m_Stack.ctm, m_Matrix1)
            i = WriteLine(i, rec, NewText)
         Else
            m_PDFInst.ReplacePageTextA(Nothing, m_Stack)
            'Compute the string position
            m_Matrix2 = MulMatrix(m_Stack.ctm, m_Stack.tm)
            i = WriteLine(i, rec, NewText)
         End If
         lastTmpl = rec.TmplHandle
      End While
      If Not m_PDFInst.FlushPageContent(m_Stack) Then Throw New Exception("-1")
      If lastTmpl > -1 Then m_PDFInst.EndTemplate()
   End Sub

   Protected Sub SetFont()
      If Not m_LastFont.Equals(m_Stack.IFont) Then
         Dim font As TPDFFontObj = New TPDFFontObj
         'Not that this function does not use the DynaPDF exception handling!
         'The only reason why this function can fail is out of memory.
         If Not m_PDFInst.GetFont(m_Stack.IFont, font) Then Throw New Exception("Out of memory!")
         Dim familyName As String = Nothing
         Dim style As TFStyle = TFStyle.fsNone
         If (font.ItalicAngle <> 0.0F) OrElse (font.Flags And &H40) <> 0 Then
            style = style Or TFStyle.fsItalic
         End If
         'The force bold flag must be ignored if the font type is not Type1.
         If (font.FontType = TFontType.ftType1) AndAlso (font.Flags And &H40000) <> 0 Then
            style = style Or TFStyle.fsBold
         End If
         BildFamilyNameAndStyle(font.FontName, familyName, style)
         If m_PDFInst.SetFontExW(familyName, style, 10.0, True, TCodepage.cpUnicode) < 0 Then
            If (font.Flags And 1) <> 0 Then 'Fixed pitch
               If m_PDFInst.SetFontW("Courier New", style, 10.0, True, TCodepage.cpUnicode) < 0 Then
                  Throw New Exception("-1")
               End If
            ElseIf m_PDFInst.SetFontW("Arial", style, 10.0, True, TCodepage.cpUnicode) < 0 Then
               Throw New Exception("-1")
            End If
         End If
         m_LastFont = m_Stack.IFont
      End If
   End Sub

   Protected Sub Transform(ByRef M As TCTM, ByRef x As Double, ByRef y As Double)
      Dim ox As Double = x
      x = ox * M.a + y * M.c + M.x
      y = ox * M.b + y * M.d + M.y
   End Sub

   Protected Function WriteLine(ByVal Index As Integer, ByRef Record As TTextRec, ByVal NewText As String) As Integer
      'Process the remaining text in the kerning array
      Index += 1
      Dim rec As TTextRec
      Dim src As TTextRecordW
      Dim ptr As Long
      Dim text As String
      Dim i As Integer, j As Integer, p As Integer, count As Integer = 0
      Dim spaceWidth As Single = -m_Stack.SpaceWidth * 0.5F
      While Index < m_Records.Count()
         rec = m_Records.GetItem(Index)
         If rec.First <> Record.First OrElse rec.TmplHandle <> Record.TmplHandle Then Exit While
         Index += 1
         count += 1
      End While
      'We must always handle the text before and after the search text was found.
      'Handling space characters correctly is the most important part if text
      'should be replaced or deleted.
      If count > 0 Then
         'The text was found multiple times in the text record
         ptr = m_Stack.Kerning.ToInt64()
         ptr += (Record.KernRecord * Marshal.SizeOf(src))
         CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
         'The old text was already deleted, write the new one
         WriteRecord(m_Matrix2, Record, src, NewText, NewText.Length)
         Index -= count
         While count > 0
            count -= 1
            rec = m_Records.GetItem(Index)
            For j = Record.KernRecord + 1 To rec.KernRecord - 1
               ptr += Marshal.SizeOf(src)
               CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
               AddKernSpace(src.Advance, spaceWidth, m_Matrix2)
               text = Marshal.PtrToStringUni(src.Text, src.Length)
               WriteText(m_Matrix2, text, text.Length)
            Next j
            ptr += Marshal.SizeOf(src)
            CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
            If rec.StrPos > -1 AndAlso m_SearchPos < m_SearchText.Length AndAlso m_SearchText.Chars(m_SearchPos) <> Chr(32) Then
               m_Matrix4.x = -src.Advance
               m_Matrix2 = MulMatrix(m_Matrix2, m_Matrix4)
            End If
            WriteRecord(m_Matrix2, rec, src, NewText, NewText.Length)
            Record = rec
            Index += 1
         End While
         'Handle the remaining records in the kerning array
         For i = Record.KernRecord + 1 To m_Stack.KerningCount - 1
            ptr += Marshal.SizeOf(src)
            CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
            AddKernSpace(src.Advance, spaceWidth, m_Matrix2)
            If (src.Length = 0) Then Exit For
            text = Marshal.PtrToStringUni(src.Text, src.Length)
            WriteText(m_Matrix2, text, text.Length)
         Next i
      ElseIf Record.KernRecord > -1 Then
         ptr = m_Stack.Kerning.ToInt64()
         ptr += (Record.KernRecord * Marshal.SizeOf(src))
         CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
         WriteRecord(m_Matrix2, Record, src, NewText, NewText.Length)
         For i = Record.KernRecord + 1 To m_Stack.KerningCount - 1
            ptr += Marshal.SizeOf(src)
            CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
            text = Marshal.PtrToStringUni(src.Text, src.Length)
            AddKernSpaceEx(src.Advance, spaceWidth, m_Matrix2)
            If m_SearchPos < m_SearchText.Length Then
               p = FindEndPattern(text, 0)
               WriteTextEx(m_Matrix2, text, p)
            Else
               WriteText(m_Matrix2, text, text.Length)
            End If
         Next i
      Else
         'The text begins at the end of the previous text record.
         ptr = m_Stack.Kerning.ToInt64()
         CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
         WriteRecord(m_Matrix3, Record, src, NewText, NewText.Length)
         For i = 1 To m_Stack.KerningCount - 1
            ptr += Marshal.SizeOf(src)
            CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
            text = Marshal.PtrToStringUni(src.Text, src.Length)
            AddKernSpace(src.Advance, spaceWidth, m_Matrix3)
            WriteText(m_Matrix3, text, text.Length)
         Next i
         m_Matrix2 = m_Matrix3
      End If
      Return WriteRemaining(Index, Record, NewText)
   End Function

   Protected Sub WriteRecord(ByRef M As TCTM, ByRef Record As TTextRec, ByRef Source As TTextRecordW, ByVal NewText As String, ByVal sLen As Integer)
      Dim p As Integer
      m_SearchPos = 0
      Dim text As String = Marshal.PtrToStringUni(Source.Text, Source.Length)
      If Record.StrPos < 0 Then
         m_SearchPos += 1
         p = FindEndPattern(text, 0)
         'Write the new text if any
         If sLen > 0 Then
            WriteText(M, NewText, NewText.Length)
         End If
         If p < Source.Length Then
            WriteTextEx(M, text, p)
         End If
      Else
         p = Record.StrPos
         If p > 0 Then
            WriteText(M, text, p)
            'Write the new text if any
            If sLen > 0 Then
               WriteText(M, NewText, sLen)
            End If
            p = FindEndPattern(text, p)
            If p < text.Length Then
               WriteTextEx(M, text, p)
            End If
         Else
            p = FindEndPattern(text, 0)
            'Write the new text if any
            If sLen > 0 Then
               WriteText(M, NewText, sLen)
            End If
            If p < text.Length Then
               WriteTextEx(M, text, p)
            End If
         End If
      End If
   End Sub

   Protected Function WriteRemaining(ByVal Index As Integer, ByRef Record As TTextRec, ByVal NewText As String) As Integer
      'We handle here the rest of the text line if it is stored in separate text records.
      'This is rather complex especially if the search text was found multiple times in the
      'line and if the line consists in turn of multiple text records. We handle
      'this case recursivley because this is much easier in comparison to a non-recursive
      'version.
      Dim m1 As TCTM, m2 As TCTM
      Dim rec As TTextRec
      Dim src As TTextRecordW
      Dim spaceWidth1 As Single
      Dim ptr As Long
      Dim text As String
      Dim nextIndex As Integer = -1
      Dim x As Double = 0.0, y As Double = 0.0, spaceWidth2 As Double, distX As Double = 0.0, distY As Double = 0.0
      Dim i As Integer, j As Integer, p As Integer, lastRecord As Integer, lastAlpha As Integer
      If Not m_HaveMore Then
         m_RecordNumber += 1
         Return Index
      End If
      If Index >= m_Records.Count() Then
         lastRecord = &H7FFFFFFF
      Else
         rec = m_Records.GetItem(Index)
         lastRecord = rec.First
         If Not rec.NewLine Then nextIndex = Index
      End If
      m_LastX = 0.0
      m_LastY = 0.0
      m_Matrix3 = m_Matrix2
      'Get the end offset of the last text record. The current position of the
      'original text is different because the new text was already written to
      'the file.
      m_Matrix4.x = m_Stack.TextWidth
      m_Matrix2 = MulMatrix(m_Stack.tm, m_Matrix4)
      m_Matrix2 = MulMatrix(m_Stack.ctm, m_Matrix2)
      Transform(m_Matrix2, m_LastX, m_LastY)
      CalcAlpha(m_Matrix2)
      lastAlpha = m_Alpha
      spaceWidth2 = m_Stack.SpaceWidth * GetScaleFactor(m_Matrix2) * 0.5
      m_RecordNumber += 1
      'Was the search string found in multiple records?
      While m_RecordNumber < Record.Last
         m_HaveMore = m_PDFInst.GetPageText(m_Stack)
         If Not m_HaveMore Then Return Index
         m_RecordNumber += 1
         spaceWidth1 = -m_Stack.SpaceWidth / 2.0F
         m_Matrix2 = MulMatrix(m_Stack.ctm, m_Stack.tm)
         x = 0.0
         y = 0.0
         Transform(m_Matrix2, x, y)
         AddSpace(x, y, spaceWidth2)
         m_PDFInst.ReplacePageTextA(Nothing, m_Stack)
         ptr = m_Stack.Kerning.ToInt64()
         For i = 0 To m_Stack.KerningCount - 1
            CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
            text = Marshal.PtrToStringUni(src.Text, src.Length)
            AddKernSpaceEx(src.Advance, spaceWidth1, m_Matrix3)
            If m_SearchPos < m_SearchText.Length Then
               p = FindEndPattern(text, 0)
               If p < src.Length Then
                  'We are now at the end of the already replaced text. Only the remaining text behind the
                  'search text must be written to the file.
                  WriteTextEx(m_Matrix3, text, p)
               End If
            Else
               WriteText(m_Matrix3, text, text.Length)
            End If
            ptr += Marshal.SizeOf(src)
         Next i
         spaceWidth2 = m_Stack.SpaceWidth * GetScaleFactor(m_Matrix2) * 0.5
         m_LastY = 0.0
         m_LastX = m_Stack.TextWidth
         Transform(m_Matrix2, m_LastX, m_LastY)
      End While
      'Now we come to the most complicated part: we must find all records which lie on the same
      'text line and move the text behind the replaced one to avoid overlaping text or holes
      'between text records. We must consider the spacing between records to preserve the original
      'layout. Note that we deal with two text positions here: m_Matrix2 contains the coordinates
      'of the original text while m_Matrix3 represents the position of the new text.
      m1 = m_Matrix2
      Invert(m1)
      CalcAlpha(m_Matrix2)
      lastAlpha = m_Alpha
      While m_HaveMore AndAlso m_RecordNumber < lastRecord
         m_HaveMore = m_PDFInst.GetPageText(m_Stack)
         If Not m_HaveMore Then Return Index
         m_RecordNumber += 1
         spaceWidth1 = -m_Stack.SpaceWidth / 2.0F
         CalcStrPos(m_Matrix2, x, y)
         If m_Alpha <> lastAlpha Then
            nextIndex = -1
            Exit While
         End If
         CalcDistance(m1, m_Matrix2, m2, distX, distY, x, y)
         If distY > MAX_LINE_ERROR Then
            'We are on a new text line...
            nextIndex = -1
            Exit While
         ElseIf distX < -spaceWidth2 Or distX > 6.0 * spaceWidth2 Then
            'The distance is too large. We assume that the text should not be considered
            'as part of the current text line.
            nextIndex = -1
            Exit While
         End If
         m_PDFInst.ReplacePageTextA(Nothing, m_Stack)
         AddSpace(distX, spaceWidth2, m_Matrix3)
         ptr = m_Stack.Kerning.ToInt64()
         For i = 0 To m_Stack.KerningCount - 1
            CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
            text = Marshal.PtrToStringUni(src.Text, src.Length)
            AddKernSpaceEx(src.Advance, spaceWidth1, m_Matrix3)
            If m_SearchPos < m_SearchText.Length Then
               p = FindEndPattern(text, 0)
               If p < src.Length Then
                  'We are now at the end of the already replaced text. Only the remaining text behind the
                  'search text must be written to the file.
                  WriteTextEx(m_Matrix3, text, p)
               End If
            Else
               WriteText(m_Matrix3, text, text.Length)
            End If
            ptr += Marshal.SizeOf(src)
         Next i
         lastAlpha = m_Alpha
         spaceWidth2 = m_Stack.SpaceWidth * GetScaleFactor(m_Matrix2) * 0.5
         m_LastY = 0.0
         m_LastX = m_Stack.TextWidth
         Transform(m_Matrix2, m_LastX, m_LastY)
      End While
      If nextIndex > -1 Then
         rec = m_Records.GetItem(nextIndex)
         If rec.NewLine Then Return Index
         Index += 1
         m_HaveMore = m_PDFInst.GetPageText(m_Stack)
         'We are still in the same text line. We must check whether a space character must be deleted.
         'The rest of the line is now processed recursively.
         CalcStrPos(m_Matrix2, x, y)
         If m_Alpha = 0 Then
            distX = x - m_LastX
         Else
            distX = CalcDistance(m_LastX, m_LastY, x, y)
         End If
         AddSpace(distX, spaceWidth2, m_Matrix3)
         If rec.KernRecord > 0 Then
            'Delete the string but preserve the kerning records before the string occurred.
            m_Stack.DeleteKerningAt = rec.KernRecord
            m_PDFInst.ReplacePageTextA(Nothing, m_Stack)
            m_Matrix4.x = 0.0
            ptr = m_Stack.Kerning.ToInt64()
            For j = 0 To rec.KernRecord - 1
               CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
               m_Matrix4.x -= src.Advance
               m_Matrix4.x += src.Width
               ptr += Marshal.SizeOf(src)
            Next j
            CPDF.CopyKernRecord(New IntPtr(ptr), src, Marshal.SizeOf(src))
            'If StrPos == -1 the first character is a space character which is emulated with kerning space.
            If rec.StrPos > -1 Then m_Matrix4.x -= src.Advance
            'Compute the string position
            m_Matrix2 = MulMatrix(m_Matrix3, m_Matrix4)
            Index = WriteLine(Index, rec, NewText)
         Else
            m_PDFInst.ReplacePageTextA(Nothing, m_Stack)
            m_Matrix2 = m_Matrix3
            Index = WriteLine(Index, rec, NewText)
         End If
      End If
      Return Index
   End Function

   Protected Sub WriteText(ByRef M As TCTM, ByVal Text As String, ByVal sLen As Integer)
      'Initialize the graphics state
      SetFont()
      'The new text is written in red color so that you can better find it
      m_PDFInst.SetColorSpace(TPDFColorSpace.csDeviceRGB)
      m_PDFInst.SetFillColor(CPDF.PDF_RED)
      m_PDFInst.SetStrokeColor(CPDF.PDF_RED)
      m_PDFInst.SetTextDrawMode(m_Stack.DrawMode)

      'This is the normal initalization
      'm_PDFInst.SetFillColorSpace(m_Stack.FillCS)
      'm_PDFInst.SetFillColor(m_Stack.FillColor)
      'm_PDFInst.SetStrokeColorSpace(m_Stack.StrokeCS)
      'm_PDFInst.SetStrokeColor(m_Stack.StrokeColor)
      'm_PDFInst.SetTextDrawMode(m_Stack.DrawMode)

      m_PDFInst.ChangeFontSize(m_Stack.FontSize)
      m_PDFInst.WriteTextMatrixExW(M, Text, sLen)
      ' Compute the end offset of the text
      Dim tmp As TCTM
      tmp.a = 1.0
      tmp.b = 0.0
      tmp.c = 0.0
      tmp.d = 1.0
      tmp.x = m_PDFInst.GetTextWidthExW(Text, sLen)
      tmp.y = 0.0
      M = MulMatrix(M, tmp)
   End Sub

   Protected Sub WriteTextEx(ByRef M As TCTM, ByVal Text As String, ByVal First As Integer)
      'Initialize the graphics state
      SetFont()
      'The new text is written in red color so that you can better find it
      m_PDFInst.SetColorSpace(TPDFColorSpace.csDeviceRGB)
      m_PDFInst.SetFillColor(CPDF.PDF_RED)
      m_PDFInst.SetStrokeColor(CPDF.PDF_RED)
      m_PDFInst.SetTextDrawMode(m_Stack.DrawMode)

      'This is the normal initalization
      'm_PDFInst.SetFillColorSpace(m_Stack.FillCS)
      'm_PDFInst.SetFillColor(m_Stack.FillColor)
      'm_PDFInst.SetStrokeColorSpace(m_Stack.StrokeCS)
      'm_PDFInst.SetStrokeColor(m_Stack.StrokeColor)
      'm_PDFInst.SetTextDrawMode(m_Stack.DrawMode)

      m_PDFInst.ChangeFontSize(m_Stack.FontSize)
      ' Get the substring
      Dim txt As String = Text.Substring(First)
      m_PDFInst.WriteTextMatrixExW(M, txt, txt.Length)
      ' Compute the end offset of the text
      Dim tmp As TCTM
      tmp.a = 1.0
      tmp.b = 0.0
      tmp.c = 0.0
      tmp.d = 1.0
      tmp.x = m_PDFInst.GetTextWidthExW(txt, txt.Length)
      tmp.y = 0.0
      M = MulMatrix(M, tmp)
   End Sub

   Protected m_Alpha As Integer
   Protected m_CurrTmpl As Integer
   Protected m_First As Integer
   Protected m_HaveMore As Boolean
   Protected m_KernRecord As Integer
   Protected m_LastFont As IntPtr
   Protected m_LastX As Double
   Protected m_LastY As Double
   Protected m_Matrix1 As TCTM
   Protected m_Matrix2 As TCTM
   Protected m_Matrix3 As TCTM
   Protected m_Matrix4 As TCTM
   Protected m_NewLine As Boolean
   Protected m_PDFInst As CPDF
   Protected m_RecordNumber As Integer
   Protected m_Records As CTextRecords
   Protected m_SearchPos As Integer
   Protected m_SearchText As String
   Protected m_Stack As TPDFStack
   Protected m_StrPos As Integer
   Protected m_Templates As CIntList
End Class
