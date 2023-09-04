Option Strict On
Option Explicit On 
Imports System.Runtime.InteropServices
Imports System.Text
Imports text_search.DynaPDF

Friend Class CTextSearch

   Private Const MAX_LINE_ERROR As Double = 4.0 ' This must be the square of the allowed error (2 * 2 in this case).

   Public Structure TGState
      Dim ActiveFont As IntPtr
      Dim CharSpacing As Single
      Dim FontSize As Double
      Dim FontType As TFontType
      Dim Matrix As TCTM
      Dim SpaceWidth As Single
      Dim TextDrawMode As TDrawMode
      Dim TextScale As Single
      Dim WordSpacing As Single
   End Structure

   Protected Class CStack
      Protected Overrides Sub Finalize()
         m_Items = Nothing
      End Sub

      Public Function Restore(ByRef F As TGState) As Boolean
         If m_Count > 0 Then
            m_Count -= 1
            F = m_Items(m_Count)
            Return True
         End If
         Return False
      End Function

      Public Function Save(ByRef F As TGState) As Integer
         If m_Count = m_Capacity Then
            m_Capacity += 16
            Try
               ReDim m_Items(m_Capacity)
            Catch
               m_Capacity -= 16
               Return -1
            End Try
         End If
         m_Items(m_Count) = F
         m_Count += 1
         Return 0
      End Function
      Private m_Capacity As Integer
      Private m_Count As Integer
      Private m_Items As TGState()
   End Class

   Public Sub New(ByVal PDFInst As CPDF)
      m_GState.ActiveFont = IntPtr.Zero
      m_GState.CharSpacing = 0.0F
      m_GState.FontSize = 1.0
      m_GState.FontType = TFontType.ftType1
      m_GState.Matrix.a = 1.0
      m_GState.Matrix.b = 0.0
      m_GState.Matrix.c = 0.0
      m_GState.Matrix.d = 1.0
      m_GState.Matrix.x = 0.0
      m_GState.Matrix.y = 0.0
      m_GState.SpaceWidth = 0.0F
      m_GState.TextDrawMode = TDrawMode.dmNormal
      m_GState.TextScale = 100.0F
      m_GState.WordSpacing = 0.0F
      m_LastTextDir = TTextDir.tfNotInitialized
      m_PDF = PDFInst
      m_Stack = New CStack()
   End Sub

   Public Function BeginTemplate(ByVal BBox As TPDFRect, ByVal Matrix As IntPtr) As Integer
      Reset()
      If Not IntPtr.Zero.Equals(Matrix) Then
         Dim m As TCTM
         m = CType(Marshal.PtrToStructure(Matrix, GetType(TCTM)), TCTM)
         m_GState.Matrix = MulMatrix(m_GState.Matrix, m)
      End If
      Return 0
   End Function

   Protected Function CalcDistance(ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double) As Double
      Dim dx As Double = x2 - x1
      Dim dy As Double = y2 - y1
      Return Math.Sqrt(dx * dx + dy * dy)
   End Function

   Private Function Compare() As Boolean
      Dim i As Integer
      ' m_OutBuf contains an Unicode string. TranslateRawCode() uses a byte array because
      ' we found no other way to marshal the string.
      Dim txt As String = System.Text.UnicodeEncoding.Unicode.GetString(m_OutBuf, 0, m_OutLen * 2)
      Do While i < m_OutLen
         If m_SearchText.Chars(m_SearchPos) <> txt.Chars(i) Then
            m_HavePos = False
            m_SearchPos = 0
            Return False
         End If
         i += 1
         m_SearchPos += 1
         If m_SearchPos = m_SearchText.Length Then
            m_SearchPos = 0
            Return (i = m_OutLen)
         End If
      Loop
      Return True
   End Function

   Private Function DrawRect(ByVal Matrix As TCTM, ByVal EndX As Double) As Boolean
      ' Note that the start and end coordinate can use different transformation matrices
      Dim x2 As Double = EndX
      Dim y2 As Double = 0.0
      Dim x3 As Double = EndX
      Dim y3 As Double = m_GState.FontSize
      Transform(Matrix, x2, y2)
      Transform(Matrix, x3, y3)
      Return DrawRectEx(x2, y2, x3, y3)
   End Function

   Private Function DrawRectEx(ByVal x2 As Double, ByVal y2 As Double, ByVal x3 As Double, ByVal y3 As Double) As Boolean
      m_PDF.MoveTo(m_x1, m_y1)
      m_PDF.LineTo(x2, y2)
      m_PDF.LineTo(x3, y3)
      m_PDF.LineTo(m_x4, m_y4)
      m_HavePos = False
      m_SelCount += 1
      Return m_PDF.ClosePath(TPathFillMode.fmFill)
   End Function

   Public Function GetSelCount() As Integer
      Return m_SelCount
   End Function

   Private Function MarkSubString(ByRef x As Double, ByVal Matrix As TCTM, ByVal Source As TTextRecordA) As Boolean
      Dim i As Integer, decoded As Integer
      Dim spaceWidth2 As Single = -m_GState.SpaceWidth * 6.0F
      Dim width As Double
      Dim max As Integer = Source.Length
      Dim src As Long = Source.Text.ToInt64
      If Source.Advance < -m_GState.SpaceWidth Then
         ' If the distance is too large then we assume that no space was emulated at this position.
         If Source.Advance > spaceWidth2 AndAlso m_SearchText.Chars(m_SearchPos) = Chr(32) Then
            If Not m_HavePos Then
               SetStartCoord(Matrix, x)
               m_SearchPos += 1
               If m_SearchPos = m_SearchText.Length Then
                  If Not DrawRect(Matrix, x - Source.Advance) Then Return False
                  Reset()
               End If
            ElseIf m_SearchPos = m_SearchText.Length Then
               If Not DrawRect(Matrix, 0.0) Then Return False
               Reset()
            Else
               m_SearchPos += 1
            End If
         Else
            Reset()
         End If
      End If
      x -= Source.Advance
      Do While i < max
         i += m_PDF.TranslateRawCode(m_GState.ActiveFont, New IntPtr(src + i), max - i, width, m_OutBuf, m_OutLen, decoded, m_GState.CharSpacing, m_GState.WordSpacing, m_GState.TextScale)
         ' We skip the text record if the text cannot be converted to Unicode. The return value must be true,
         ' otherwise we would break processing.
         If decoded = 0 Then Return True
         ' m_OutLen is always greater zero if decoded is true!
         If Compare() Then
            If Not m_HavePos Then
               SetStartCoord(Matrix, x)
            End If
            x += width
            If m_SearchPos = 0 Then
               If Not DrawRect(Matrix, x) Then Return False
            End If
         Else
            x += width
         End If
      Loop
      Return True
   End Function

   Public Function MarkText(ByVal Matrix As TCTM, ByVal Source() As TTextRecordA, ByVal Count As Integer, ByVal Width As Double) As Integer
      Try
         'Note that we write rectangles to the page while we parsed it. This is critical because the parser
         'doesn't notice when a fatal error occurs, e.g. out of memory. We must make sure that processing
         'breaks immediately in such a case. To archive this the return value of ClosePath() is checked each
         'time a rectangle is drawn to the page. The function can only fail if a fatal error occurred.
         Dim i As Integer
         Dim textDir As TTextDir
         Dim x As Double
         Dim x1 As Double = 0.0
         Dim y1 As Double = 0.0
         Dim x2 As Double = 0.0
         Dim y2 As Double = m_GState.FontSize
         ' Transform the text matrix to user space
         Dim m As TCTM = MulMatrix(m_GState.Matrix, Matrix)
         Transform(m, x1, y1) ' Start point of the text record
         Transform(m, x2, y2) ' Second point to determine the text direction
         ' Determine the text direction
         If y1 = y2 Then
            textDir = CType((System.Convert.ToInt32(x1 > x2) + 1) * 2, TTextDir)
         Else
            textDir = CType(System.Convert.ToInt32(y1 > y2), TTextDir)
         End If

         If textDir <> m_LastTextDir OrElse Not IsPointOnLine(x1, y1, m_LastTextEndX1, m_LastTextEndY1, m_LastTextInfX, m_LastTextInfY) Then
            ' Extend the x-coordinate to an infinite point
            m_LastTextInfX = 1000000.0
            m_LastTextInfY = 0.0
            Transform(m, m_LastTextInfX, m_LastTextInfY)
            Reset()
         Else
            Dim distance As Double, spaceWidth As Double
            Dim x3 As Double = m_GState.SpaceWidth
            Dim y3 As Double = 0.0
            Transform(m, x3, y3)
            spaceWidth = CalcDistance(x1, y1, x3, y3)
            distance = CalcDistance(m_LastTextEndX1, m_LastTextEndY1, x1, y1)
            If distance > spaceWidth Then
               ' If the distance is too large then we assume that no space was emulated at this position.
               If distance < spaceWidth * 6.0 AndAlso m_SearchText.Chars(m_SearchPos) = Chr(32) Then
                  If Not m_HavePos Then
                     ' The start coordinate is the end coordinate of the last text record.
                     m_HavePos = True
                     m_SearchPos += 1
                     If m_SearchPos = m_SearchText.Length Then
                        m_x1 = m_LastTextEndX1
                        m_y1 = m_LastTextEndY1
                        m_x4 = m_LastTextEndX4
                        m_y4 = m_LastTextEndY4
                        If Not DrawRectEx(x1, y1, x2, y2) Then Return -1
                        Reset()
                     End If
                  ElseIf m_SearchPos = m_SearchText.Length Then
                     If Not DrawRectEx(x1, y1, x2, y2) Then Return -1
                     Reset()
                  Else
                     m_SearchPos += 1
                  End If
               Else
                  Reset()
               End If
            End If
         End If
         x = 0.0
         For i = 0 To Count - 1
            If Not MarkSubString(x, m, Source(i)) Then Return -1
         Next i
         m_LastTextDir = textDir
         m_LastTextEndX1 = Width
         m_LastTextEndY1 = 0.0
         m_LastTextEndX4 = 0.0
         m_LastTextEndY4 = m_GState.FontSize
         Transform(m, m_LastTextEndX1, m_LastTextEndY1)
         Transform(m, m_LastTextEndX4, m_LastTextEndY4)
         Return 0
      Catch
         Return -1
      End Try
   End Function

   Public Sub Init()
      InitGState()
      Reset()
      m_SelCount = 0
   End Sub

   Protected Sub InitGState()
      Do While RestoreGState()
      Loop
      m_GState.ActiveFont = IntPtr.Zero
      m_GState.CharSpacing = 0.0F
      m_GState.FontSize = 1.0
      m_GState.Matrix.a = 1.0
      m_GState.Matrix.b = 0.0
      m_GState.Matrix.c = 0.0
      m_GState.Matrix.d = 1.0
      m_GState.Matrix.x = 0.0
      m_GState.Matrix.y = 0.0
      m_GState.SpaceWidth = 0.0F
      m_GState.TextDrawMode = TDrawMode.dmNormal
      m_GState.TextScale = 100.0F
      m_GState.WordSpacing = 0.0F
      m_LastTextDir = TTextDir.tfNotInitialized
      m_LastTextInfX = 0.0
      m_LastTextInfY = 0.0
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

   Public Sub MulMatrix(ByVal M As TCTM)
      m_GState.Matrix = MulMatrix(m_GState.Matrix, M)
   End Sub

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

   Private Sub Reset()
      m_HavePos = False
      m_SearchPos = 0
   End Sub

   Public Function RestoreGState() As Boolean
      Return m_Stack.Restore(m_GState)
   End Function

   Public Function SaveGState() As Integer
      Return m_Stack.Save(m_GState)
   End Function

   Public Sub SetCharSpacing(ByVal Value As Double)
      m_GState.CharSpacing = CType(Value, Single)
   End Sub

   Public Sub SetFont(ByVal FontSize As Double, ByVal Type As TFontType, ByVal Font As IntPtr)
      m_GState.ActiveFont = Font
      m_GState.FontSize = FontSize
      m_GState.FontType = Type
      m_GState.SpaceWidth = CType(m_PDF.GetSpaceWidth(Font, FontSize) * 0.5, Single)
   End Sub

   Public Sub SetSearchText(ByVal Text As String)
      m_SearchText = Text
      m_SearchPos = 0
   End Sub

   Private Sub SetStartCoord(ByVal Matrix As TCTM, ByVal x As Double)
      m_x1 = x
      m_y1 = 0.0
      m_x4 = x
      m_y4 = m_GState.FontSize
      Transform(Matrix, m_x1, m_y1)
      Transform(Matrix, m_x4, m_y4)
      m_HavePos = True
   End Sub

   Public Sub SetTextDrawMode(ByVal Mode As TDrawMode)
      m_GState.TextDrawMode = Mode
   End Sub

   Public Sub SetTextScale(ByVal Value As Double)
      m_GState.TextScale = CType(Value, Single)
   End Sub

   Public Sub SetWordSpacing(ByVal Value As Double)
      m_GState.WordSpacing = CType(Value, Single)
   End Sub

   Protected Sub Transform(ByRef M As TCTM, ByRef x As Double, ByRef y As Double)
      Dim tx As Double
      tx = x
      x = tx * M.a + y * M.c + M.x
      y = tx * M.b + y * M.d + M.y
   End Sub

   Protected Enum TTextDir
      tfLeftToRight = 0
      tfRightToLeft = 1
      tfTopToBottom = 2
      tfBottomToTop = 4
      tfNotInitialized = -1
   End Enum

   Protected m_GState As TGState
   Protected m_HavePos As Boolean
   Protected m_LastTextDir As TTextDir
   Protected m_LastTextEndX1 As Double
   Protected m_LastTextEndY1 As Double
   Protected m_LastTextEndX4 As Double
   Protected m_LastTextEndY4 As Double
   Protected m_LastTextInfX As Double
   Protected m_LastTextInfY As Double
   Protected m_OutBuf(63) As Byte
   Protected m_OutLen As Integer
   Protected m_PDF As CPDF
   Protected m_SearchPos As Integer
   Protected m_SearchText As String
   Protected m_SelCount As Integer
   Protected m_Stack As CStack
   Protected m_x1 As Double
   Protected m_x4 As Double
   Protected m_y1 As Double
   Protected m_y4 As Double
End Class
